using Loadlibrayy.Logger;
using Loadlibrayy.Natives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Loadlibrayy.Helpers;
using System.Runtime.CompilerServices;

namespace Loadlibrayy.Extensions
{
    // THIS CLASS IS A WRAPPER FOR THE MANAGED PROCESS/PROCESSTHREAD CLASS
    public unsafe static class WhatMicrosoftShouldHaveDone
    {
        #region Remote Code Execution
        public static void InjectShellcode(this Process process, byte[] shellcodeBuffer)
        {
            // WRITE SHELLCODE TO TARGET MEMORY
            var shellcodeRemoteCall = process.AllocateAndWrite(shellcodeBuffer, NT.AllocationType.Commit | NT.AllocationType.Reserve, NT.MemoryProtection.ExecuteReadWrite);
            
            // CALL THE SHELLCODE TO CALL OUR SHELLCODE
            var shellcodeThread = process.CreateThread(shellcodeRemoteCall, 0, out ulong threadId);

            // WAIT FOR THE THREAD TO FINISH
            NTM.WaitForThread(shellcodeThread);

            // FREE THE SHELLCODE
            process.FreeMemory(shellcodeRemoteCall);
        }
        #endregion

        #region Information
        public static ulong GetPebAddress(this Process process)
        {
            NT.PROCESS_BASIC_INFORMATION pbi = new NT.PROCESS_BASIC_INFORMATION();
            NT.NtQueryInformationProcess(process.Handle, 0, &pbi, pbi.Size, IntPtr.Zero);

            return pbi.PebBaseAddress;
        }
        public static NT._PEB_LDR_DATA GetLoaderData(this Process process)
        {
            var peb = process.Read<NT._PEB>(process.GetPebAddress());
            return process.Read<NT._PEB_LDR_DATA>(peb.Ldr);
        }
        public static void WriteLoaderData(this Process process, NT._PEB_LDR_DATA ldrData)
        {
            var peb = process.Read<NT._PEB>(process.GetPebAddress());
            process.Write(ldrData, peb.Ldr);
        }
        #endregion

        #region Memory
        public static ulong AllocateMemory(this Process process, uint length, NT.AllocationType allocationType, NT.MemoryProtection memoryProtection) =>
            NT.VirtualAllocEx(process.Handle, 0, length, allocationType, memoryProtection);

        public static bool FreeMemory(this Process process, ulong memoryPointer) =>
            NT.VirtualFreeEx(process.Handle, memoryPointer, 0, NT.AllocationType.Release);

        public static NT.MEMORY_BASIC_INFORMATION VirtualQuery(this Process process, ulong memoryPointer)
        {
            var structSize = (uint)Marshal.SizeOf(typeof(NT.MEMORY_BASIC_INFORMATION));
            NT.VirtualQueryEx(process.Handle, memoryPointer, out NT.MEMORY_BASIC_INFORMATION mem, structSize);
            return mem;
        }
        public static NT.MemoryProtection VirtualProtect(this Process process, ulong memoryPointer, int size, NT.MemoryProtection newProtect)
        {
            if (!NT.VirtualProtectEx(process.Handle, memoryPointer, size, newProtect, out NT.MemoryProtection oldProtect))
                throw new Exception($"VirtualProtect - VirtualProtectEx() failed - {Marshal.GetLastWin32Error().ToString("x2")}");

            return oldProtect;
        }
        
        public static ulong AllocateAndWrite(this Process process, byte[] buffer, NT.AllocationType allocationType, NT.MemoryProtection memoryProtection)
        {
            ulong allocatedMemory = process.AllocateMemory((uint)buffer.Length, allocationType, memoryProtection);

            process.WriteRawMemory(buffer, allocatedMemory);

            return allocatedMemory;
        }

        public static void WriteRawMemory(this Process process, byte[] buffer, ulong memoryPointer)
        {
            //NT.MemoryProtection oldProtect = process.VirtualProtect(memoryPointer, buffer.Length, NT.MemoryProtection.ExecuteReadWrite);

            if (!NT.WriteProcessMemory(process.Handle, memoryPointer, buffer, (uint)buffer.Length, 0))
                throw new Exception($"WriteBuffer - WriteProcessMemory() failed - {Marshal.GetLastWin32Error().ToString("x2")}");

            //process.VirtualProtect(memoryPointer, buffer.Length, oldProtect);
        }
        public static byte[] ReadRawMemory(this Process process, ulong memoryPointer, int size)
        {
            byte[] buffer = new byte[size];
            if (!NT.ReadProcessMemory(process.Handle, memoryPointer, buffer, buffer.Length, 0))
                throw new Exception($"ReadRawMemory - ReadProcessMemory() failed - {Marshal.GetLastWin32Error().ToString("x2")}");
            return buffer;
        }

        public static void Write<T>(this Process process, T value, ulong memoryPointer) where T : struct
        {
            byte[] buffer = Tools.GetBytes(value);
            process.WriteRawMemory(buffer, memoryPointer);
        }
        public static T Read<T>(this Process process, ulong memoryPointer) where T : struct
        {
            byte[] buffer = process.ReadRawMemory(memoryPointer, Unsafe.SizeOf<T>());

            return Tools.GetStructure<T>(buffer);
        }

        public static void NukeMemoryPage(this Process process, ulong memoryPointer, int size = 0)
        {
            // GENERATE BYTE ARRAY OF TOTAL SIZE OF PAGE
            byte[] headerBuffer = new byte[size == 0 ? (int)process.VirtualQuery(memoryPointer).RegionSize : size];

            // FILL OUT WITH RANDOM BYTES
            NTM.RandomEngine.NextBytes(headerBuffer);

            // LMAO BYE
            process.WriteRawMemory(headerBuffer, memoryPointer);
        }
        public static ulong CreateSection(this Process process, NT.MemoryProtection memoryProtection, long size)
        {
            var result = NT.NtCreateSection(out ulong sectionHandle, NT.ACCESS_MASK.GENERIC_ALL, 0, out size, memoryProtection, 0x8000000 /*SEC_COMMIT*/, 0);

            if (result != 0)
                throw new Exception($"CreateSection - NtCreateSection() failed - {result.ToString("x2")}");

            return sectionHandle;
        }
        public static ulong MapSection(this Process process, ulong sectionHandle, NT.MemoryProtection memoryProtection)
        {
            ulong memoryPointer = 0;
            var result = NT.NtMapViewOfSection(sectionHandle, process.Handle, ref memoryPointer, 0, 0, 0, out uint viewSize, 2, 0, memoryProtection);
            if (result != 0)
                throw new Exception($"MapSection - NtMapViewOfSection() failed - {result.ToString("x2")}");

            return memoryPointer;
        }
        public static uint UnmapSection(this Process process, ulong baseAddress) => NT.NtUnmapViewOfSection(process.Handle, baseAddress);
        #endregion

        #region Modules
        public static ulong GetModuleByName(this Process process, string moduleName)
        {
            ulong[] moduleHandleArray = new ulong[1000];

            fixed (ulong* hMods = moduleHandleArray)
            {
                if (NT.EnumProcessModules(process.Handle, (ulong)hMods, (uint)(sizeof(ulong) * moduleHandleArray.Length), out uint cbNeeded) > 0)
                {
                    for (int moduleIndex = 0; moduleIndex < cbNeeded / sizeof(ulong); moduleIndex++)
                    {
                        string name = NTM.GetModuleBaseName(process.Handle, moduleHandleArray[moduleIndex]);

                        if (String.Equals(name, moduleName, StringComparison.InvariantCultureIgnoreCase))
                            return moduleHandleArray[moduleIndex];
                    }
                }
            }

            return 0;
        }
        public static Dictionary<string, ulong> GetModules(this Process process)
        {
            var result = new Dictionary<string, ulong>();

            ulong[] moduleHandleArray = new ulong[1000];

            fixed (ulong* hMods = moduleHandleArray)
            {
                if (NT.EnumProcessModules(process.Handle, (ulong)hMods, (uint)(sizeof(ulong) * moduleHandleArray.Length), out uint cbNeeded) > 0)
                {
                    for (int moduleIndex = 0; moduleIndex < cbNeeded / sizeof(ulong); moduleIndex++)
                    {
                        string name = NTM.GetModuleBaseName(process.Handle, moduleHandleArray[moduleIndex]);

                        result[name.ToLower()] = moduleHandleArray[moduleIndex];

                        //if (String.Equals(name, moduleName, StringComparison.InvariantCultureIgnoreCase))
                        //    return moduleHandleArray[moduleIndex];
                    }
                }
            }

            return result;
        }
        #endregion

        #region Threads
        public static ulong CreateThread(this Process process, ulong startAddress, ulong argumentAddress, out ulong threadId)
        {
            return NT.CreateRemoteThread(process.Handle, 0, 0, startAddress, argumentAddress, 0, out threadId);
        }
        public static uint CreateAndWaitForThread(this Process process, ulong startAddress, ulong argumentAddress, out ulong threadHandle)
        {
            threadHandle = NT.CreateRemoteThread(process.Handle, 0, 0, startAddress, argumentAddress, 0, out ulong threadId);
            NTM.WaitForThread(threadHandle);
            return NTM.GetThreadExitCode(threadHandle);
        }

        public static ulong GetNativeHandle(this ProcessThread thread, NT.ThreadAccess accessRights) => NTM.OpenThread(accessRights, thread.Id);
        #endregion
    }
}

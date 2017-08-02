using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cpuz141
{
    public unsafe class CPUZ
    {
        private const string CPUZ_DISPLAY_NAME = "cpuz141";
        private const string CPUZ_FILE_NAME = "C:\\Windows\\System32\\drivers\\cpuz141.sys";
        private const string CPUZ_DEVICE_NAME = "\\Device\\cpuz141";

        private IntPtr g_hService;
        private IntPtr g_hDevice;

        #region Memory Structs
        [StructLayout(LayoutKind.Sequential)]
        struct InputReadStruct
        {
            public uint AddressHigh;
            public uint AddressLow;
            public uint Length;
            public uint BufferHigh;
            public uint BufferLow;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct InputWriteStruct
        {
            public uint AddressHigh;
            public uint AddressLow;
            public uint Value;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct OutputStruct
        {
            public uint Operation;
            public uint BufferLow;
        };
        #endregion

        // DRIVER FUNCTIONS
        public bool Load()
        {
            IntPtr hService;
            if (ServiceHelper.OpenService(out hService, CPUZ_DISPLAY_NAME, 0x0020/*SERVICE_STOP*/ | 0x00010000/*DELETE*/))
            {
                Console.WriteLine($"[!] Service already running");
                
                if (!ServiceHelper.StopService(hService))
                    Console.WriteLine($"[!] Couldn't stop service");

                if (!ServiceHelper.DeleteService(hService))
                    Console.WriteLine($"[!] Couldn't delete service");

                ServiceHelper.CloseServiceHandle(hService);
                return Load();
            }

            File.WriteAllBytes(CPUZ_FILE_NAME, CPUZSHELLCODE.Shellcode);
            
            Console.WriteLine($"[+] Loading...");

            if (!ServiceHelper.CreateService(
                ref g_hService,
                CPUZ_DISPLAY_NAME, CPUZ_DISPLAY_NAME,
                CPUZ_FILE_NAME, 
                (uint)Win.SERVICE_ACCESS.SERVICE_ALL_ACCESS, 1/*SERVICE_KERNEL_DRIVER*/,
                (uint)Win.SERVICE_START.SERVICE_DEMAND_START, 1/*SERVICE_ERROR_NORMAL*/))
            {
                Console.WriteLine($"[!] Failed to create service - {Marshal.GetLastWin32Error():X}");
                return false;
            }

            if (!ServiceHelper.StartService(g_hService))
            {
                Console.WriteLine($"[!] Failed to start service - {Marshal.GetLastWin32Error():X}");
                ServiceHelper.DeleteService(g_hService);
                return false;
            }

            Console.WriteLine($"[+] Getting Device Handle");

            Win.OBJECT_ATTRIBUTES ObjectAttributes = new Win.OBJECT_ATTRIBUTES();
            Win.UNICODE_STRING DeviceName = new Win.UNICODE_STRING(CPUZ_DEVICE_NAME);
            Win.IO_STATUS_BLOCK IoStatus;
            ObjectAttributes.Length = Marshal.SizeOf(typeof(Win.OBJECT_ATTRIBUTES));
            ObjectAttributes.ObjectName = new IntPtr(&DeviceName);

            uint Status = 0;
            IntPtr hDevice;

            do
            {
                Status = Win.NtOpenFile(
                    &hDevice,
                    (uint)(Win.ACCESS_MASK.GENERIC_READ | Win.ACCESS_MASK.GENERIC_WRITE | Win.ACCESS_MASK.SYNCHRONIZE),
                    &ObjectAttributes, &IoStatus, 0, 3/*OPEN_EXISTING*/);
                
                if (Status != 0/*NT_SUCCESS*/)
                {
                    Console.WriteLine($"[!] NtOpenFile failed! - {Status:X}");
                    Thread.Sleep(250);
                }

            } while (Status != 0/*NT_SUCCESS*/);
            
            g_hDevice = hDevice;

            Console.WriteLine($"[+] hService: {g_hService:X}");
            Console.WriteLine($"[+] hDevice: {g_hDevice:X}");

            return true;
        }
        public bool Unload()
        {
            if (!ServiceHelper.StopService(g_hService))
            {
                Console.WriteLine($"[!] Couldn't stop service");
                return false;
            }

            if (!ServiceHelper.DeleteService(g_hService))
            {
                Console.WriteLine($"[!] Couldn't delete service");
                return false;
            }

            ServiceHelper.CloseServiceHandle(g_hService);
            Win.CloseHandle(g_hDevice);

            Console.WriteLine($"[+] Unloaded service");

            return true;
        }

        // HELPERS
        private ulong LODWORD(ulong l) => (l & 0xffffffff);
        private ulong HIDWORD(ulong l) => ((l >> 32) & 0xffffffff);
        public ulong TranslateLinearAddress(ulong directoryTableBase, ulong virtualAddress)
        {
            ushort PML4 = (ushort)((virtualAddress >> 39) & 0x1FF); //<! PML4 Entry Index
            ushort DirectoryPtr = (ushort)((virtualAddress >> 30) & 0x1FF); //<! Page-Directory-Pointer Table Index
            ushort Directory = (ushort)((virtualAddress >> 21) & 0x1FF); //<! Page Directory Table Index
            ushort Table = (ushort)((virtualAddress >> 12) & 0x1FF); //<! Page Table Index


            // Read the PML4 Entry. DirectoryTableBase has the base address of the table.
            // It can be read from the CR3 register or from the kernel process object.
            ulong PML4E = ReadPhysicalAddress<ulong>(directoryTableBase + (ulong)PML4 * sizeof(ulong));

            if (PML4E == 0)
                return 0;

            // The PML4E that we read is the base address of the next table on the chain,
            // the Page-Directory-Pointer Table.
            ulong PDPTE = ReadPhysicalAddress<ulong>((PML4E & 0xFFFF1FFFFFF000) + (ulong)DirectoryPtr * sizeof(ulong));

            if (PDPTE == 0)
                return 0;

            //Check the PS bit
            if ((PDPTE & (1 << 7)) != 0)
            {
                // If the PDPTE’s PS flag is 1, the PDPTE maps a 1-GByte page. The
                // final physical address is computed as follows:
                // — Bits 51:30 are from the PDPTE.
                // — Bits 29:0 are from the original va address.
                return (PDPTE & 0xFFFFFC0000000) + (virtualAddress & 0x3FFFFFFF);
            }

            // PS bit was 0. That means that the PDPTE references the next table
            // on the chain, the Page Directory Table. Read it.
            ulong PDE = ReadPhysicalAddress<ulong>((PDPTE & 0xFFFFFFFFFF000) + (ulong)Directory * sizeof(ulong));

            if (PDE == 0)
                return 0;

            if ((PDE & (1 << 7)) != 0)
            {
                // If the PDE’s PS flag is 1, the PDE maps a 2-MByte page. The
                // final physical address is computed as follows:
                // — Bits 51:21 are from the PDE.
                // — Bits 20:0 are from the original va address.
                return (PDE & 0xFFFFFFFE00000) + (virtualAddress & 0x1FFFFF);
            }

            // PS bit was 0. That means that the PDE references a Page Table.
            ulong PTE = ReadPhysicalAddress<ulong>((PDE & 0xFFFFFFFFFF000) + (ulong)Table * sizeof(ulong));

            if (PTE == 0)
                return 0;

            // The PTE maps a 4-KByte page. The
            // final physical address is computed as follows:
            // — Bits 51:12 are from the PTE.
            // — Bits 11:0 are from the original va address.
            return (PTE & 0xFFFFFFFFFF000) + (virtualAddress & 0xFFF);
        }

        // READ CONTROL REGISTERS
        public ulong ReadControlRegister(uint nControlRegister)
        {
            const uint ioctl = 0x9C402428;
            ulong io = 0;
            
            ulong value = 0;

            if (!Win.DeviceIoControl(g_hDevice, ioctl, &nControlRegister, sizeof(uint), &value, sizeof(ulong), &io, 0))
                throw new Exception("DeviceIonControl failed! - 0x9C402428");

            return value;
        }
        
        public bool ReadPhysicalAddress(ulong lpAddress, ulong lpBuffer, ulong lLength)
        {
            const uint ioctl = 0x9C402420;
            ulong io = 0ul;

            InputReadStruct input = new InputReadStruct();
            OutputStruct output = new OutputStruct();

            if (lpAddress == 0 || lpBuffer == 0)
                return false;

            input.AddressHigh = (uint)HIDWORD(lpAddress);
            input.AddressLow = (uint)LODWORD(lpAddress);
            input.Length = (uint)lLength;
            input.BufferHigh = (uint)HIDWORD(lpBuffer);
            input.BufferLow = (uint)LODWORD(lpBuffer);

            return Win.DeviceIoControl(g_hDevice, ioctl, &input, (uint)Marshal.SizeOf(typeof(InputReadStruct)), &output, (uint)Marshal.SizeOf(typeof(OutputStruct)), &io, 0);
        }
        public bool WritePhysicalAddress(ulong address, ulong buf, ulong len)
        {
            if (len % 4 != 0 || len == 0)
                throw new Exception("The CPU-Z driver can only write lengths that are aligned to 4 bytes (4, 8, 12, 16, etc)");

            const uint ioctl = 0x9C402430;
            ulong io = 0ul;

            InputWriteStruct in_mem = new InputWriteStruct();
            OutputStruct out_mem = new OutputStruct();

            if (address == 0 || buf == 0)
                return false;

            if (len == 4)
            {
                in_mem.AddressHigh = (uint)HIDWORD(address);
                in_mem.AddressLow = (uint)LODWORD(address);
                in_mem.Value = *(uint*)buf;

                return !!Win.DeviceIoControl(g_hDevice, ioctl, &in_mem, (uint)Marshal.SizeOf(typeof(InputWriteStruct)), &out_mem, (uint)Marshal.SizeOf(typeof(OutputStruct)), &io, 0);
            }
            else
            {
                for (uint i = 0; i < len / 4; i++)
                {
                    in_mem.AddressHigh = (uint)HIDWORD(address + 4 * i);
                    in_mem.AddressLow = (uint)LODWORD(address + 4 * i);
                    in_mem.Value = ((uint*)buf)[i];
                    if (!Win.DeviceIoControl(g_hDevice, ioctl, &in_mem, (uint)Marshal.SizeOf(typeof(InputWriteStruct)), &out_mem, (uint)Marshal.SizeOf(typeof(OutputStruct)), &io, 0))
                        return false;
                }
                return true;
            }
        }
        public bool ReadSystemAddress(ulong address, ulong buf, ulong len)
        {
            ulong dirbase = ReadControlRegister(3);
            
            ulong phys = TranslateLinearAddress(dirbase, address);
            

            if (phys == 0)
                return false;

            return ReadPhysicalAddress(phys, buf, len);
        }
        public bool WriteSystemAddress(ulong address, ulong buf, ulong len)
        {
            ulong dirbase = ReadControlRegister(3);
            ulong phys = TranslateLinearAddress(dirbase, address);

            if (phys == 0)
                return false;

            return WritePhysicalAddress(phys, buf, len);
        }

        // GENERIC FUNCTIONS
        public T ReadPhysicalAddress<T>(ulong address) where T : struct
        {
            var buf = (ulong*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

            if (!ReadPhysicalAddress(address, (ulong)buf, (ulong)Marshal.SizeOf(typeof(T))))
                 throw new Exception("Read failed");
            
            T result = (T)Marshal.PtrToStructure((IntPtr)buf, typeof(T));

            Marshal.FreeHGlobal((IntPtr)buf);

            return result;

        }
        public T ReadSystemAddress<T>(ulong address) where T : struct
        {
            var buf = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            
            if (!ReadSystemAddress(address, (ulong)buf, (ulong)Marshal.SizeOf(typeof(T))))
                throw new Exception("Read failed");

            T result = (T)Marshal.PtrToStructure(buf, typeof(T));

            Marshal.FreeHGlobal(buf);

            return result;
        }
        public bool WritePhysicalAddress<T>(ulong address, T value) where T : struct
        {
            var buf = (ulong)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(value, (IntPtr)buf, false);

            bool success = WritePhysicalAddress(address, buf, (ulong)Marshal.SizeOf(typeof(T)));

            Marshal.FreeHGlobal((IntPtr)buf);

            return success;
        }
        public bool WriteSystemAddress<T>(ulong address, T value) where T : struct
        {
            var buf = (ulong)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
            Marshal.StructureToPtr(value, (IntPtr)buf, false);

            bool success = WriteSystemAddress(address, buf, (ulong)Marshal.SizeOf(typeof(T)));

            Marshal.FreeHGlobal((IntPtr)buf);

            return success;
        }

        private static class Win
        {
            #region Function
            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool DeviceIoControl(
                IntPtr hDevice, 
                uint dwIoControlCode,
                void* lpInBuffer, 
                uint nInBufferSize,
                void* lpOutBuffer, 
                uint nOutBufferSize,
                ulong* lpBytesReturned, 
                uint lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
            public static extern uint NtOpenFile(IntPtr* FileHandle, uint DesiredAccess, OBJECT_ATTRIBUTES* ObjectAttributes, IO_STATUS_BLOCK* IoStatusBlock, uint ShareAccess, uint OpenOptions);
            #endregion
            #region Structs
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct OBJECT_ATTRIBUTES
            {
                public Int32 Length;
                public IntPtr RootDirectory;
                public IntPtr ObjectName;
                public uint Attributes;
                public IntPtr SecurityDescriptor;
                public IntPtr SecurityQualityOfService;

            }
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct IO_STATUS_BLOCK
            {
                public uint status;
                public IntPtr information;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct UNICODE_STRING : IDisposable
            {
                public ushort Length;
                public ushort MaximumLength;
                private IntPtr buffer;

                public UNICODE_STRING(string s)
                {
                    Length = (ushort)(s.Length * 2);
                    MaximumLength = (ushort)(Length + 2);
                    buffer = Marshal.StringToHGlobalUni(s);
                }

                public void Dispose()
                {
                    Marshal.FreeHGlobal(buffer);
                    buffer = IntPtr.Zero;
                }

                public override string ToString()
                {
                    return Marshal.PtrToStringUni(buffer);
                }
            }
            #endregion
            #region Flags
            [Flags]
            public enum SERVICE_ACCESS : uint
            {
                /// <summary>
                /// Required to call the QueryServiceConfig and 
                /// QueryServiceConfig2 functions to query the service configuration.
                /// </summary>
                SERVICE_QUERY_CONFIG = 0x00001,

                /// <summary>
                /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function 
                /// to change the service configuration. Because this grants the caller 
                /// the right to change the executable file that the system runs, 
                /// it should be granted only to administrators.
                /// </summary>
                SERVICE_CHANGE_CONFIG = 0x00002,

                /// <summary>
                /// Required to call the QueryServiceStatusEx function to ask the service 
                /// control manager about the status of the service.
                /// </summary>
                SERVICE_QUERY_STATUS = 0x00004,

                /// <summary>
                /// Required to call the EnumDependentServices function to enumerate all 
                /// the services dependent on the service.
                /// </summary>
                SERVICE_ENUMERATE_DEPENDENTS = 0x00008,

                /// <summary>
                /// Required to call the StartService function to start the service.
                /// </summary>
                SERVICE_START = 0x00010,

                /// <summary>
                ///     Required to call the ControlService function to stop the service.
                /// </summary>
                SERVICE_STOP = 0x00020,

                /// <summary>
                /// Required to call the ControlService function to pause or continue 
                /// the service.
                /// </summary>
                SERVICE_PAUSE_CONTINUE = 0x00040,

                /// <summary>
                /// Required to call the EnumDependentServices function to enumerate all
                /// the services dependent on the service.
                /// </summary>
                SERVICE_INTERROGATE = 0x00080,

                /// <summary>
                /// Required to call the ControlService function to specify a user-defined
                /// control code.
                /// </summary>
                SERVICE_USER_DEFINED_CONTROL = 0x00100,

                /// <summary>
                /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table.
                /// </summary>
                SERVICE_ALL_ACCESS = (ACCESS_MASK.STANDARD_RIGHTS_REQUIRED |
                    SERVICE_QUERY_CONFIG |
                    SERVICE_CHANGE_CONFIG |
                    SERVICE_QUERY_STATUS |
                    SERVICE_ENUMERATE_DEPENDENTS |
                    SERVICE_START |
                    SERVICE_STOP |
                    SERVICE_PAUSE_CONTINUE |
                    SERVICE_INTERROGATE |
                    SERVICE_USER_DEFINED_CONTROL),

                GENERIC_READ = ACCESS_MASK.STANDARD_RIGHTS_READ |
                    SERVICE_QUERY_CONFIG |
                    SERVICE_QUERY_STATUS |
                    SERVICE_INTERROGATE |
                    SERVICE_ENUMERATE_DEPENDENTS,

                GENERIC_WRITE = ACCESS_MASK.STANDARD_RIGHTS_WRITE |
                    SERVICE_CHANGE_CONFIG,

                GENERIC_EXECUTE = ACCESS_MASK.STANDARD_RIGHTS_EXECUTE |
                    SERVICE_START |
                    SERVICE_STOP |
                    SERVICE_PAUSE_CONTINUE |
                    SERVICE_USER_DEFINED_CONTROL,

                /// <summary>
                /// Required to call the QueryServiceObjectSecurity or 
                /// SetServiceObjectSecurity function to access the SACL. The proper
                /// way to obtain this access is to enable the SE_SECURITY_NAME 
                /// privilege in the caller's current access token, open the handle 
                /// for ACCESS_SYSTEM_SECURITY access, and then disable the privilege.
                /// </summary>
                ACCESS_SYSTEM_SECURITY = ACCESS_MASK.ACCESS_SYSTEM_SECURITY,

                /// <summary>
                /// Required to call the DeleteService function to delete the service.
                /// </summary>
                DELETE = ACCESS_MASK.DELETE,

                /// <summary>
                /// Required to call the QueryServiceObjectSecurity function to query
                /// the security descriptor of the service object.
                /// </summary>
                READ_CONTROL = ACCESS_MASK.READ_CONTROL,

                /// <summary>
                /// Required to call the SetServiceObjectSecurity function to modify
                /// the Dacl member of the service object's security descriptor.
                /// </summary>
                WRITE_DAC = ACCESS_MASK.WRITE_DAC,

                /// <summary>
                /// Required to call the SetServiceObjectSecurity function to modify 
                /// the Owner and Group members of the service object's security 
                /// descriptor.
                /// </summary>
                WRITE_OWNER = ACCESS_MASK.WRITE_OWNER,
            }

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

            /// <summary>
            /// Service start options
            /// </summary>
            public enum SERVICE_START : uint
            {
                /// <summary>
                /// A device driver started by the system loader. This value is valid
                /// only for driver services.
                /// </summary>
                SERVICE_BOOT_START = 0x00000000,

                /// <summary>
                /// A device driver started by the IoInitSystem function. This value 
                /// is valid only for driver services.
                /// </summary>
                SERVICE_SYSTEM_START = 0x00000001,

                /// <summary>
                /// A service started automatically by the service control manager 
                /// during system startup. For more information, see Automatically 
                /// Starting Services.
                /// </summary>         
                SERVICE_AUTO_START = 0x00000002,

                /// <summary>
                /// A service started by the service control manager when a process 
                /// calls the StartService function. For more information, see 
                /// Starting Services on Demand.
                /// </summary>
                SERVICE_DEMAND_START = 0x00000003,

                /// <summary>
                /// A service that cannot be started. Attempts to start the service
                /// result in the error code ERROR_SERVICE_DISABLED.
                /// </summary>
                SERVICE_DISABLED = 0x00000004,
            }

            #endregion
        }
    }

    public static class ServiceHelper
    {
        public static bool CreateService(
            ref IntPtr hService, 
            string ServiceName, 
            string DisplayName, 
            string BinPath, 
            uint DesiredAccess, 
            uint ServiceType, 
            uint StartType, 
            uint ErrorControl)
        {
            IntPtr hSCManager = Win.OpenSCManager(0, 0, 0x0002/*SC_MANAGER_CREATE_SERVICE*/);

            if (hSCManager == IntPtr.Zero)
                return false;

            hService = Win.CreateServiceW(
                hSCManager,
                ServiceName, DisplayName,
                DesiredAccess,
                ServiceType, StartType,
                ErrorControl, BinPath,
                0, 0, 0, 0, 0, 0);

            Win.CloseServiceHandle(hSCManager);

            return hService != IntPtr.Zero;
        }
        public static bool OpenService(out IntPtr hService, string szServiceName, uint DesiredAccess)
        {
            IntPtr hSCManager = Win.OpenSCManager(0, 0, DesiredAccess);
            hService = Win.OpenService(hSCManager, szServiceName, DesiredAccess);
            Win.CloseServiceHandle(hSCManager);
            return hService != IntPtr.Zero;
        }
        public static bool StopService(IntPtr hService)
        {
            Win.SERVICE_STATUS ServiceStatus = new Win.SERVICE_STATUS();
            return Win.ControlService(hService, Win.SERVICE_CONTROL.STOP, ref ServiceStatus);
        }

        public static bool StartService(IntPtr hService) => Win.StartService(hService, 0, null);
        public static bool DeleteService(IntPtr hService) => Win.DeleteService(hService);
        public static void CloseServiceHandle(IntPtr hService) => Win.CloseServiceHandle(hService);

        private static class Win
        {
            [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr OpenSCManager(uint machineName, uint databaseName, uint dwAccess);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseServiceHandle(IntPtr hSCObject);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ControlService(IntPtr hService, SERVICE_CONTROL dwControl, ref SERVICE_STATUS lpServiceStatus);

            [DllImport("advapi32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool StartService(
                IntPtr hService,
            int dwNumServiceArgs,
            string[] lpServiceArgVectors
            );

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteService(IntPtr hService);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateServiceW(
                IntPtr hSCManager,
                string lpServiceName,
                string lpDisplayName,
                uint dwDesiredAccess,
                uint dwServiceType,
                uint dwStartType,
                uint dwErrorControl,
                string lpBinaryPathName,
                uint lpLoadOrderGroup,
                uint lpdwTagId,
                uint lpdwTagId1,
                uint lpDependencies,
                uint lpServiceStartName,
                uint lpPassword);

            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct SERVICE_STATUS
            {
                public SERVICE_TYPE dwServiceType;
                public SERVICE_STATE dwCurrentState;
                public uint dwControlsAccepted;
                public uint dwWin32ExitCode;
                public uint dwServiceSpecificExitCode;
                public uint dwCheckPoint;
                public uint dwWaitHint;
            }
            [Flags]
            internal enum SERVICE_TYPE : int
            {
                SERVICE_KERNEL_DRIVER = 0x00000001,
                SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,
                SERVICE_WIN32_OWN_PROCESS = 0x00000010,
                SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
                SERVICE_INTERACTIVE_PROCESS = 0x00000100
            }
            [Flags]
            public enum SERVICE_CONTROL : uint
            {
                STOP = 0x00000001,
                PAUSE = 0x00000002,
                CONTINUE = 0x00000003,
                INTERROGATE = 0x00000004,
                SHUTDOWN = 0x00000005,
                PARAMCHANGE = 0x00000006,
                NETBINDADD = 0x00000007,
                NETBINDREMOVE = 0x00000008,
                NETBINDENABLE = 0x00000009,
                NETBINDDISABLE = 0x0000000A,
                DEVICEEVENT = 0x0000000B,
                HARDWAREPROFILECHANGE = 0x0000000C,
                POWEREVENT = 0x0000000D,
                SESSIONCHANGE = 0x0000000E
            }
            public enum SERVICE_STATE : uint
            {
                SERVICE_STOPPED = 0x00000001,
                SERVICE_START_PENDING = 0x00000002,
                SERVICE_STOP_PENDING = 0x00000003,
                SERVICE_RUNNING = 0x00000004,
                SERVICE_CONTINUE_PENDING = 0x00000005,
                SERVICE_PAUSE_PENDING = 0x00000006,
                SERVICE_PAUSED = 0x00000007
            }

            [Flags]
            public enum SERVICE_ACCEPT : uint
            {
                STOP = 0x00000001,
                PAUSE_CONTINUE = 0x00000002,
                SHUTDOWN = 0x00000004,
                PARAMCHANGE = 0x00000008,
                NETBINDCHANGE = 0x00000010,
                HARDWAREPROFILECHANGE = 0x00000020,
                POWEREVENT = 0x00000040,
                SESSIONCHANGE = 0x00000080,
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadlibrayy.Helpers
{
    // SMALL CLASS THAT GENERATES VARIOUS
    // SHELLCODES USED BY THIS INJECTOR
    // THIS IS MERELY FOR EYE CANDY
    public static unsafe class ShellcodeGenerator
    {
        public static byte[] CallLoadLibrary(ulong allocatedImagePath, ulong loadLibraryPointer)
        {
            // threadhijack_loadlibrary_x64.asm
            byte[] shellcode = new byte[]
            {
                        0x9C, 0x50, 0x53, 0x51, 0x52, 0x41, 0x50, 0x41, 0x51, 0x41, 0x52, 0x41, 0x53, // push     REGISTERS
                        0x48, 0x83, 0xEC, 0x28,                                                       // sub      RSP, 0x28
                        0x48, 0xB9, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                   // movabs   RCX, 0x0000000000000000 ; Image path
                        0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                   // movabs   RAX, 0x0000000000000000 ; Pointer to LoadLibrary
                        0xFF, 0xD0,                                                                   // call     RAX
                        0x48, 0x83, 0xC4, 0x28,                                                       // add      RSP, 0x28
                        0x41, 0x5B, 0x41, 0x5A, 0x41, 0x59, 0x41, 0x58, 0x5A, 0x59, 0x5B, 0x58, 0x9D, // pop      REGISTER
                        0xC3                                                                          // ret
            };

            // WRITE POINTERS TO SHELLCODE
            fixed (byte* shellcodePointer = shellcode)
            {
                *(ulong*)(shellcodePointer + 19) = allocatedImagePath;
                *(ulong*)(shellcodePointer + 29) = loadLibraryPointer;
            }

            return shellcode;
        }

        public static byte[] CallDllMain(ulong remoteImage, ulong entrypointPointer, bool hijackSafe)
        {
            byte[] shellcode;

            if (hijackSafe)
            {
                // threadhijack_dllmain_x64.asm
                shellcode = new byte[]
                {
                        0x9C, 0x50, 0x53, 0x51, 0x52, 0x41, 0x50, 0x41, 0x51, 0x41, 0x52, 0x41, 0x53,   // push     REGISTERS
                        0x48, 0x83, 0xEC, 0x28,                                                         // sub      RSP, 0x28
                        0x48, 0xB9, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                     // movabs   RCX, 0x0000000000000000 
                        0x48, 0xC7, 0xC2, 0x01, 0x00, 0x00, 0x00,                                       // mov      rdx, 0x1
                        0x4D, 0x31, 0xC0,                                                               // xor      r8, r8
                        0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,                     // movabs   RAX, 0x0000000000000000
                        0xFF, 0xD0,                                                                     // call     RAX
                        0x48, 0x83, 0xC4, 0x28,                                                         // add      RSP, 0x28
                        0x41, 0x5B, 0x41, 0x5A, 0x41, 0x59, 0x41, 0x58, 0x5A, 0x59, 0x5B, 0x58, 0x9D,   // pop      REGISTERS
                        0xC3
                };

                // WRITE POINTERS TO SHELLCODE
                fixed (byte* shellcodePointer = shellcode)
                {
                    *(ulong*)(shellcodePointer + 19) = remoteImage;
                    *(ulong*)(shellcodePointer + 39) = entrypointPointer;
                }
            }
            else
            {
                // call_dllmain_x64.asm
                shellcode = new byte[]
                {
                        0x48, 0x83, 0xEC, 0x28,                                         // sub      RSP, 0x28
                        0x48, 0xB9, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,     // movabs   RCX, 0x0000000000000000
                        0x48, 0xC7, 0xC2, 0x01, 0x00, 0x00, 0x00,                       // mov      rdx, 0x1
                        0x4D, 0x31, 0xC0,                                               // xor      r8, r8
                        0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,     // movabs   RAX, 0x0000000000000000
                        0xFF, 0xD0,                                                     // call     RAX
                        0x48, 0x83, 0xC4, 0x28,                                         // add      RSP, 0x28
                        0xC3                                                            // ret
                    };

                // WRITE POINTERS TO SHELLCODE
                fixed (byte* shellcodePointer = shellcode)
                {
                    *(ulong*)(shellcodePointer + 6) = remoteImage;
                    *(ulong*)(shellcodePointer + 26) = entrypointPointer;
                }
            }

            

            return shellcode;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Loadlibrayy.Natives;

namespace Loadlibrayy.Helpers
{
    public static unsafe class Tools
    {
        // I FUCKING HATE TYPING THIS SHIT
        public static int Size<T>() where T : struct => Marshal.SizeOf(typeof(T));
        
        // THIS AS WELL
        public static ushort GetRelocationData(void* baseRelocation, int index) => 
            *(ushort*)((long)baseRelocation + Tools.Size<NT.IMAGE_BASE_RELOCATION>() + sizeof(ushort) * index);
        
        public static NT.IMAGE_SECTION_HEADER* GetFirstSection(ulong localImage, NT.IMAGE_DOS_HEADER* dosHeader) =>
            (NT.IMAGE_SECTION_HEADER*)(localImage + (uint)dosHeader->e_lfanew/*START OF NTHEADER*/ + (uint)Tools.Size<NT.IMAGE_NT_HEADERS>());


        public static string FindDll(string imageName)
        {
            // https://msdn.microsoft.com/en-us/library/7d83bc18.aspx?f=255&MSPPError=-2147217396
            // The Windows system directory. The GetSystemDirectory function retrieves the path of this directory.
            // The Windows directory. The GetWindowsDirectory function retrieves the path of this directory.
            
            return 
                SearchDirectoryForImage(Environment.GetFolderPath(Environment.SpecialFolder.Windows)) ?? 
                SearchDirectoryForImage(Environment.GetFolderPath(Environment.SpecialFolder.System));
                
            // HELPER FUNCTION TO FIND IMAGES
            string SearchDirectoryForImage(string directoryPath)
            {
                foreach (string imagePath in Directory.GetFiles(directoryPath, "*.dll"))
                    if (String.Equals(Path.GetFileName(imagePath), imageName, StringComparison.InvariantCultureIgnoreCase))
                        return imagePath;

                return null;
            }
        }
    }
}

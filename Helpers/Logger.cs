using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loadlibrayy.Logger
{
    public static class Log
    {
        public static void LogGeneral(string general) =>
            Console.WriteLine($"[+] {general}");

        public static void LogInfo(string information) =>
            Console.WriteLine($"[?] {information}");

        public static void LogVariable<T>(string variableName, T variable) =>
            Console.WriteLine($"[?] {variableName} - {variable}");

        public static void LogError(string error) =>
            Console.WriteLine($"[!] {error}");

        public static void ShowWarning(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void ShowInformation(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ShowError(string message, string title) =>
           MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

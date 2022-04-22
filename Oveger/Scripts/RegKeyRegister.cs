using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace Oveger.XAMLS
{
    public static class RegKeyRegister
    {
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static void SetStartup(bool CheckStartup)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (CheckStartup)
                rk.SetValue(AppName, Assembly.GetExecutingAssembly().Location);
            else
                rk.DeleteValue(AppName, false);
        }
    }
}

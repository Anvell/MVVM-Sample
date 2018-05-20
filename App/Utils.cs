using System;
using System.Windows;
using static System.IO.Path;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace MVVMSample
{

    public static class Utils {

        public static string GetVersion() {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static bool FindInStrings(this string s, params string[] instr) {
            var searched = s.ToLowerInvariant();
            foreach (var p in instr) {
                if(p.ToLowerInvariant().Contains(searched)) return true;
            }
            return false;
        }

        public static bool FindInStrings(this Regex re, params string[] instr) {
            foreach (var p in instr) {
                if (re.IsMatch(p)) return true;
            }
            return false;
        }

        public static string GetFileByDialog<T>(string fn = "") where T: FileDialog, new() {
            var dlg = new T();
            dlg.FileName = GetFileName(fn);
            dlg.Filter = Strings.file_filter;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (dlg.ShowDialog() == true) {
                return dlg.FileName;
            }
            return string.Empty;
        }

        public static void ShowAlert(string msg, MessageBoxImage icon = MessageBoxImage.Exclamation) {
            MessageBox.Show(msg, Strings.label_error, MessageBoxButton.OK, icon);
        }
    }
}
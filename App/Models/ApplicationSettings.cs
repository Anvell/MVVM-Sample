using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using MugenMvvmToolkit.Models;
using PropertyChanged;

namespace MVVMSample.Models {

    public class ApplicationSettings : NotifyPropertyChangedBase {

        public string lastFileName { get; set; } = string.Empty;
        public double top { get; set; } = 0;
        public double left { get; set; } = 0;
        public double width { get; set; } = 1400;
        public double height { get; set; } = 820;

        [XmlIgnore]
        public WindowState windowState { get; set; } = WindowState.Normal;

        [DoNotNotify]
        [XmlElement("windowState")]
        public int windowStateInt32 {
            get { return (int) windowState; }
            set { windowState = (WindowState) value; }
        }

        public ApplicationSettings() {
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            left = (screenWidth / 2) - (width > screenWidth ? screenWidth : width) / 2;
            top = (screenHeight / 2) - (height > screenHeight ? screenHeight : height) / 2;
        }

        public static string GetSettingsFullPath(string fileName) {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appdata, Strings.app_title, fileName);
        }

        public static string GetSettingsFolder() => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Strings.app_title);

        public static ApplicationSettings FromFile(string fileName = Strings.app_settings_file) {
            ApplicationSettings settings = null;

            var fullPath = GetSettingsFullPath(fileName);

            if (!File.Exists(fullPath)) return new ApplicationSettings();

            var serializerObj = new XmlSerializer(typeof(ApplicationSettings));

            using (var file = new StreamReader(fullPath)) {
                try {
                    settings = serializerObj.Deserialize(file) as ApplicationSettings;
                }
                catch { /* not needed */ }
            }

            return settings?? new ApplicationSettings();
        }

        public void SaveToFile(string fileName = Strings.app_settings_file) {
            var serializerObj = new XmlSerializer(typeof(ApplicationSettings));

            var dir = GetSettingsFolder();
            if (!Directory.Exists(dir)) {

                try {
                    Directory.CreateDirectory(dir);
                } catch {
                    return;
                }
            }

            using (var file = new StreamWriter(GetSettingsFullPath(fileName))) {
                serializerObj.Serialize(file, this);
            }
        }
    }
}

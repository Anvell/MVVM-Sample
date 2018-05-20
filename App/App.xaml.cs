using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using MugenMvvmToolkit.WPF.Infrastructure;
using MVVMSample.ViewModels;
using MugenMvvmToolkit;
using System.Runtime;
using System.IO;
using MugenMvvmToolkit.Binding;

namespace MVVMSample {
    
    public partial class App : Application
    {
        public App() {

            var settingsDir = Models.ApplicationSettings.GetSettingsFolder();
            if (!Directory.Exists(settingsDir)) {

                try {
                    Directory.CreateDirectory(settingsDir);
                }
                catch { settingsDir = null; }
            }

            if(settingsDir != null) {
                ProfileOptimization.SetProfileRoot(settingsDir);
                ProfileOptimization.StartProfile("Startup.Profile");
            }
            
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), 
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            new Bootstrapper<MainViewModel>(this, new MugenContainer());
        }
    }
}

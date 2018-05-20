using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.EventArg;
using MugenMvvmToolkit.ViewModels;
using MVVMSample.Data;
using MVVMSample.Input;
using MVVMSample.Models;
using MVVMSample.Singleton;
using MVVMSample.Views;

namespace MVVMSample.ViewModels {

    class MainViewModel : MultiViewModel<TableViewModel> {

        #region Properties

        public string fileName { get; private set; }
        public ApplicationSettings applicationSettings { get; set; }

        public string windowTitle {
            get {
                var suffix = string.IsNullOrEmpty(fileName) ? string.Empty : Strings.app_title_separator 
                                                                             + Path.GetFileName(fileName);
                return $"{Strings.app_title} {Utils.GetVersion()}{suffix}";
            }
        }

        public int currentIndex {
            get { return ItemsSource.IndexOf(SelectedItem); }
            set { if(value < ItemsSource?.Count && value >= 0) SelectedItem = ItemsSource[value]; }
        }

        public DataCollection dataCollection { get; set; }

        public ICommand newFileCmd { get; private set; }
        public ICommand newItemCmd { get; private set; }
        public ICommand openFileCmd { get; private set; }
        public ICommand saveFileCmd { get; private set; }
        public ICommand saveFileAsCmd { get; private set; }
        public ICommand undoCmd { get; private set; }
        public ICommand pasteCmd { get; private set; }

        #endregion

        #region Contructor

        public MainViewModel() {
            applicationSettings = ApplicationSettings.FromFile();
            dataCollection = new DataCollection();

            newFileCmd = new ActionCommand(() => OnNewFileCommand());
            newItemCmd = new ActionCommand(() => SelectedItem.OnNewCommand());
            openFileCmd = new ActionCommand(() => OnOpenFile());
            saveFileCmd = new ActionCommand(() => OnSaveFile());
            saveFileAsCmd = new ActionCommand(() => OnSaveFileWithDialog());
            undoCmd = new ActionCommand(() => OnUndoCommand());
            pasteCmd = new ActionCommand(() => SelectedItem.OnPasteCommand());
        }

        #endregion

        #region Methods

        protected override void OnInitialized() {
            base.OnInitialized();
            
            var tableOneViewModel = GetViewModel<TableOneViewModel>();
            tableOneViewModel.Initialize(new TableOneView(), Strings.label_table_one, dataCollection);
            AddViewModel(tableOneViewModel, false);

            var tableTwoViewModel = GetViewModel<TableTwoViewModel>();
            tableTwoViewModel.Initialize(new TableTwoView(), Strings.label_table_two, dataCollection);
            AddViewModel(tableTwoViewModel, false);

            this.AddClosingHandler(OnClosing);

            SelectedItem = ItemsSource[0];

            if (applicationSettings.lastFileName != string.Empty)
                FileProviderAction(false, applicationSettings.lastFileName);
        }

        private async void FileProviderAction(bool saveFile, string filePath, bool overwrite = false) {
            BeginBusy();
            var success = false;

            try {
                if(saveFile)
                    await FileProvider.SaveFile(filePath, dataCollection, overwrite);
                else
                    await FileProvider.LoadFile(filePath, dataCollection);
                success = true;
            }
            catch (FileNotFoundException) {
                Utils.ShowAlert($"{Path.GetFileName(filePath)}\n{Strings.label_error_file_not_found}", MessageBoxImage.Information);
            }
            catch (OperationCanceledException) {
                // canceled by user
            }
            catch (IOException) {
                Utils.ShowAlert($"{Path.GetFileName(filePath)}\n{Strings.label_error_database_corrupted}", MessageBoxImage.Error);
            }
            finally {
                await Task.Delay(500);
                this.ClearBusy();
            }

            if (success) {
                Chronos.Get.ClearUndo();
                fileName = filePath;
            }
        }

        private bool SavingCheck() {

            if (!Chronos.Get.IsEmpty()) {
                var path = string.IsNullOrWhiteSpace(fileName) ? "Untitled" : Path.GetFileName(fileName);
                var result = MessageBox.Show($"Do you want to save \"{fileName}\"?", "Question", 
                                             MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result) {
                    case MessageBoxResult.Yes:
                        if (string.IsNullOrWhiteSpace(fileName)) {
                            OnSaveFileWithDialog();
                        } else {
                            FileProviderAction(false, fileName);
                        }
                        break;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            return true;
        }

        #endregion

        #region Events

        public void OnHandleWindowDrop(DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0) {
                    fileName = new Uri(files[0]).LocalPath;
                    FileProviderAction(false, fileName);
                }
            }
        }

        public void OnUndoCommand() {
            Chronos.Get.InSleep(() => Chronos.Get.Revert());
        }

        public void OnNewFileCommand() {
            fileName = string.Empty;
            applicationSettings.lastFileName = string.Empty;
            Chronos.Get.InSleep(() => {
                dataCollection.tableOne.Clear();
                dataCollection.tableTwo.Clear();
            });
        }

        public void OnCancelFileSave() => FileProvider.Cancel();

        public void OnOpenFile() {
            var userFileName = Utils.GetFileByDialog<OpenFileDialog>();
            if (userFileName != string.Empty) {
                FileProviderAction(false, userFileName);
            }
        }

        public void OnSaveFile() {
            if(string.IsNullOrWhiteSpace(fileName)) {
                OnSaveFileWithDialog();
            } else {
                FileProviderAction(true, fileName);
            }
        }

        public void OnSaveFileWithDialog() {
            var userFileName = Utils.GetFileByDialog<SaveFileDialog>(fileName);
            if (userFileName != string.Empty) {
                FileProviderAction(true, userFileName, true);
            }
        }

        private void OnClosing(IViewModel sender, ViewModelClosingEventArgs args) {
            if (SavingCheck()) {
                applicationSettings.lastFileName = fileName;
                applicationSettings.SaveToFile();
            } else {
                args.Cancel = true;
            }
        }

        public void OnClose() => Application.Current.Shutdown();

        public void OnAboutMessage() {
            MessageBox.Show($"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name} " +
                            $"{Utils.GetVersion()}\n\n" + Strings.third_party_libraries_usage, "About", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}

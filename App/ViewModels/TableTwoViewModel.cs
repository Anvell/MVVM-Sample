using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using MVVMSample.Models;
using PropertyChanged;
using System.Windows.Controls;
using System.Windows.Input;
using MVVMSample.Input;

namespace MVVMSample.ViewModels
{

    class TableTwoViewModel : TableViewModel {
        
        #region Properties

        public ICommand copyRowCmd { get; private set; }

        public ICollectionView tableTwoDataView { get; private set; }

        DataModelTwo selectedItem { get; set; }

        [DoNotNotify]
        List<DataModelTwo> selectedItems { get; set; }

        #endregion

        #region Methods

        public override void Initialize(UserControl userContent, string userTitle, DataCollection dataCollection) {
            base.Initialize(userContent, userTitle, dataCollection);

            selectedItems = new List<DataModelTwo>();
            tableTwoDataView = CollectionViewSource.GetDefaultView(dataCollection.tableTwo);

            copyRowCmd = new ActionCommand(() => OnCopyCommand());
        }

        private void RefreshTable() => tableTwoDataView?.Refresh();

        public void ClearColumnSort() => tableTwoDataView?.SortDescriptions.Clear();

        #endregion

        #region Commands

        public override void OnNewCommand() {
            var r = DataModelTwo.New();
            dataCollection.tableTwo.Add(r);
        }

        public override void OnCopyCommand() {

        }

        public override void OnPasteCommand() {

        }

        #endregion

        #region Events

        public void OnTableSelectionChanged(SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count > 0) {
                foreach (var item in e.RemovedItems) {
                    var i = item as DataModelTwo;
                    if (i != null) selectedItems.Remove(i);
                }
            }

            if (e.AddedItems.Count > 0) {
                foreach (var item in e.AddedItems) {
                    var i = item as DataModelTwo;
                    if (i != null) selectedItems.Add(i);
                }
            }
        }

        public void OnTablePreviewKeyDown(KeyEventArgs e) {

            if (Key.C == e.Key && Keyboard.IsKeyDown(Key.LeftCtrl)) {
                e.Handled = true;
                OnCopyCommand();
            }
        }

        #endregion

    }
}

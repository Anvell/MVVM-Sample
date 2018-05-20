using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using MVVMSample.Models;
using PropertyChanged;
using System.Windows.Controls;
using System.Windows.Input;
using MVVMSample.Input;
using System;
using System.Reflection;

namespace MVVMSample.ViewModels {

    class TableOneViewModel : TableViewModel {

        #region Properties

        public ICommand copyRowCmd { get; private set; }
        public OptionType optionFilter { get; set; } = 0;

        public ICollectionView tableOneDataView { get; private set; }
        private Regex filterStringRegEx { get; set; } = null;

        DataModelOne selectedItem { get; set; }

        [DoNotNotify]
        List<DataModelOne> selectedItems { get; set; }

        [DoNotNotify]
        private string _searchText;

        [DoNotNotify]
        public string searchText {
            get { return _searchText; }
            set {
                _searchText = value;

                if (_searchText.StartsWith(":")) {
                    try {
                        filterStringRegEx = new Regex(_searchText.Substring(1), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                    }
                    catch { filterStringRegEx = null; }
                }
                OnPropertyChanged(nameof(searchText));
                RefreshTable();
            }
        }

        #endregion

        #region Methods

        public override void Initialize(UserControl userContent, string userTitle, DataCollection dataCollection) {
            base.Initialize(userContent, userTitle, dataCollection);

            selectedItems = new List<DataModelOne>();
            tableOneDataView = CollectionViewSource.GetDefaultView(dataCollection.tableOne);
            tableOneDataView.Filter = CustomFilter;

            searchText = string.Empty;

            copyRowCmd = new ActionCommand(() => OnCopyCommand());
        }

        private void RefreshTable() => tableOneDataView?.Refresh();

        public void ClearColumnSort() => tableOneDataView?.SortDescriptions.Clear();

        private bool CustomFilter(object item) {
            bool result = true;
            var r = item as DataModelOne;
            if (r == null || searchText == null) return false;

            if ((optionFilter & OptionType.One) != 0 && !r.optionOne) return false;
            if ((optionFilter & OptionType.Two) != 0 && !r.optionTwo) return false;

            if (!string.IsNullOrWhiteSpace(searchText)) {
                if (searchText.StartsWith(":") && filterStringRegEx != null) {

                    if (!filterStringRegEx.FindInStrings(r.name, r.company, r.comment))
                        return false;
                }
                else {
                    if (!searchText.FindInStrings(r.name, r.company, r.comment))
                        return false;
                }
            }
            
            return result;
        }

        #endregion

        #region Commands

        public override void OnNewCommand() {
            var r = DataModelOne.New();
            dataCollection.tableOne.Add(r);
        }

        public override void OnCopyCommand() {
            var clipboardText = string.Empty;

            foreach (var item in selectedItems) {
                foreach (var property in typeof(DataModelOne).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                    if(property.Name != "id")
                        clipboardText += $"{property.GetValue(item).ToString()}\t";
                }
                clipboardText += "\n";
            }
            Clipboard.SetText(clipboardText);
        }

        public override void OnPasteCommand() {
           
        }

        #endregion

        #region Events

        public void OnOptionFilter(bool isSelected, int filter) {
            optionFilter = isSelected? optionFilter | (OptionType)filter 
                                     : optionFilter & ~(OptionType)filter;
            RefreshTable();
        }

        public void OnTableSelectionChanged(SelectionChangedEventArgs e) {
            if (e.RemovedItems.Count > 0) {
                foreach (var item in e.RemovedItems) {
                    var i = item as DataModelOne;
                    if (i != null) selectedItems.Remove(i);
                }
            }

            if (e.AddedItems.Count > 0) {
                foreach (var item in e.AddedItems) {
                    var i = item as DataModelOne;
                    if(i != null) selectedItems.Add(i);
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

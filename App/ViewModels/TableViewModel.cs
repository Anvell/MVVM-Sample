using MugenMvvmToolkit.ViewModels;
using System.Windows.Controls;
using MVVMSample.Models;

namespace MVVMSample.ViewModels
{

    abstract class TableViewModel : ViewModelBase {

        #region Properties

        public UserControl content { get; protected set; }
        public string title { get; set; }

        public DataCollection dataCollection { get; set; }

        #endregion

        public virtual void Initialize(UserControl userContent, string userTitle, DataCollection dataCollection) {
            content = userContent;
            title = userTitle;
            content.DataContext = this;
            this.dataCollection = dataCollection;
        }

        #region Commands

        public abstract void OnNewCommand();

        public abstract void OnCopyCommand();

        public abstract void OnPasteCommand();

        #endregion
    }
}

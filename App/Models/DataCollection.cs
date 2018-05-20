using MugenMvvmToolkit.Collections;
using MugenMvvmToolkit.Models;
using MVVMSample.Singleton;
using System.Collections.Specialized;

namespace MVVMSample.Models {

    public class DataCollection : NotifyPropertyChangedBase {

        public SynchronizedNotifiableCollection<DataModelOne> tableOne { get; set; }
        public SynchronizedNotifiableCollection<DataModelTwo> tableTwo { get; set; }

        public DataCollection() {
            tableOne = new SynchronizedNotifiableCollection<DataModelOne>();
            tableOne.CollectionChanged += (s, e) => BaseCollectionChanged<DataModelOne>(s, e);
            tableTwo = new SynchronizedNotifiableCollection<DataModelTwo>();
            tableTwo.CollectionChanged += (s, e) => BaseCollectionChanged<DataModelTwo>(s, e);
        }

        private void BaseCollectionChanged<T>(object sender, NotifyCollectionChangedEventArgs e) where T : BaseModel {
            if(e.OldItems != null) {
                for (int i = 0; i < e.OldItems.Count; i++) {
                    var r = e.OldItems[i] as T;
                    r?.SetAction(PendingAction.Delete);
                    Chronos.Get.Add(tableOne, r);
                }
            }

            if (e.NewItems != null) {
                for (int i = 0; i < e.NewItems.Count; i++) {
                    var r = e.NewItems[i] as T;
                    Chronos.Get.Add(tableOne, r);
                }
            }
        }
    }
}

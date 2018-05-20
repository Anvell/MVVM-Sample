using System;
using System.ComponentModel;
using System.Reflection;
using LiteDB;
using PropertyChanged;
using MVVMSample.Interfaces;
using MVVMSample.Singleton;

namespace MVVMSample.Models {
    public class BaseModel : INotifyPropertyChanged, ISetProperty {

        [BsonId]
        [DoNotNotify]
        public ObjectId id { get; set; }

        [BsonIgnore]
        [DoNotNotify]
        protected PendingAction pendingAction { get; set; } = PendingAction.None;

        [BsonIgnore]
        [DoNotNotify]
        protected bool isNotificationsSuspended { get; set; } = false;

        [BsonIgnore]
        public event PropertyChangedEventHandler PropertyChanged;

        #region Methods

        public void OnPropertyChanged(string propertyName, object before, object after) {
            if (PropertyChanged != null && !isNotificationsSuspended) {
                Chronos.Get.Add(this, propertyName, before);
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetProperty(string propertyName, object value) {
            Chronos.Get.Sleep();
            var property = GetType().GetProperty(propertyName);
            property.SetValue(this, value);
            Chronos.Get.Awake();
        }

        public void ClearActionStatus() {
            pendingAction = PendingAction.None;
        }

        public void SetAction(PendingAction action) {
            pendingAction = action;
        }

        public PendingAction GetAction() => pendingAction;

        public bool IsDeleted() => pendingAction == PendingAction.Delete;

        public void MarkAsUpdated() {
            if (pendingAction != PendingAction.Delete)
                pendingAction = PendingAction.Update;
        }

        public ObjectId GetId() => id;

        protected void CopyValuesFrom<T>(T fromObject) {
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                property.SetValue(this, property.GetValue(fromObject));
            }

            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                field.SetValue(this, field.GetValue(fromObject));
            }
        }

        protected void SuspendNotifications() => isNotificationsSuspended = true;
        protected void RestoreNotifications() => isNotificationsSuspended = false;

        #endregion
    }
}

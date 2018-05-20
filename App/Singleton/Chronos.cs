using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MVVMSample.Interfaces;
using MVVMSample.Models;

namespace MVVMSample.Singleton
{

    public sealed class Chronos {

        #region Singleton

        private static volatile Chronos instance;
        private static object syncRoot = new object();

        public static Chronos Get {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new Chronos();
                    }
                }
                return instance;
            }
        }

        #endregion

        private bool isSuspended = false;
        private Stack<IRecord> undoStack { get; set; }

        private Chronos() {
            undoStack = new Stack<IRecord>();
        }
        
        public void Add(BaseModel node, string propertyName, object previous) {
            if(!isSuspended)
                undoStack.Push(new Record(node, propertyName, previous));
        }

        public void Add(IList sourceCollection, BaseModel item) {
            if (!isSuspended)
                undoStack.Push(new RecordCollection(sourceCollection, item));
        }

        public List<BaseModel> GetItems() {
            return undoStack.Select(x => x.GetItem()).ToList();
        }

        public void Revert() {
            if(undoStack.Count > 0)
                undoStack.Pop().Revert();
        }

        public void InSleep(Action act) {
            Sleep();
            act();
            Awake();
        }

        public bool IsEmpty() => undoStack.Count < 1;
        public void ClearUndo() => undoStack.Clear();
        public void Sleep() => isSuspended = true;
        public void Awake() => isSuspended = false;

        private class RecordCollection : IRecord {
            IList sourceCollection;
            BaseModel item;

            public RecordCollection(IList sourceCollection, BaseModel item) {
                this.sourceCollection = sourceCollection;
                this.item = item;
            }

            public void Revert() {
                if(sourceCollection.Contains(item)) {
                    sourceCollection.Remove(item);
                } else {
                    sourceCollection.Add(item);
                }
            }

            public BaseModel GetItem() => item;
        }

        private class Record : IRecord {
            BaseModel node;
            string propertyName;
            object previousValue;

            public Record(BaseModel node, string propertyName, object previous)  {
                this.node = node;
                this.propertyName = propertyName;
                previousValue = previous;
            }

            public void Revert() {
                node.SetProperty(propertyName, previousValue);
            }

            public BaseModel GetItem() => node;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace MVVMSample.Data {

    class DatabaseHelper : IDisposable {
        LiteDatabase dbHandle;

        public DatabaseHelper(string dbPath) {
            dbHandle = new LiteDatabase(dbPath);
        }

        public List<T> LoadFromDB<T>(string CollectionName) {
            var col = dbHandle.GetCollection<T>(CollectionName);
            return col.FindAll().ToList();
        }

        public void SetVersion(ushort version) {
            dbHandle.Engine.UserVersion = version;
        }

        public ushort GetVersion() => dbHandle.Engine.UserVersion;

        public T LoadFirstEntry<T>(string CollectionName) {
            T dbData;

            var col = dbHandle.GetCollection<T>(CollectionName);
            if (col != null) {
                dbData = col.FindOne(Query.All());
                return dbData;
            }
            else return default(T);
        }

        public bool SaveToDB<T>(string CollectionName, T data) {
            var col = dbHandle.GetCollection<T>(CollectionName);
            if (col != null && data != null) {
                col.Upsert(data);
                return true;
            }
            return false;
        }

        public bool SaveToDB<T>(string CollectionName, List<T> dbData) {
            var col = dbHandle.GetCollection<T>(CollectionName);
            if (col != null && dbData.Count > 0) {
                col.Upsert(dbData);
                return true;
            }
            return false;
        }

        public bool RemoveFromDB<T>(string CollectionName, List<ObjectId> dbEntries) {
            var col = dbHandle.GetCollection<T>(CollectionName);
            if (col != null && dbEntries.Count > 0) {
                foreach (var id in dbEntries) {
                    col.Delete(id);
                }
                return true;
            }
            return false;
        }

        protected virtual void _dispose(bool disposing) {
            if (disposing) {
                dbHandle.Dispose();
            }
        }

        public void Dispose() {
            _dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
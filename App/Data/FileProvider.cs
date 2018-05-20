using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVVMSample.Models;
using MVVMSample.Singleton;

namespace MVVMSample.Data
{

    static class FileProvider {

        private const ushort DATABASE_VERSION = 1;

        static FileAccessLock fileLock = new FileAccessLock();

        public static void Cancel() => fileLock.Cancel();

        public static async Task LoadFile(string fileName, DataCollection dataCollection) {

            if (!File.Exists(fileName)) {
                throw new FileNotFoundException();
            }

            try {
                await fileLock.SetLock(fileName);
            }
            catch (OperationCanceledException) {
                throw;
            }

            DatabaseHelper db = null;
            List<DataModelOne> tableOne = null;
            List<DataModelTwo> tableTwo = null;

            try {
                db = new DatabaseHelper(fileName);
                tableOne = db.LoadFromDB<DataModelOne>(Strings.app_db_name_one);
                tableTwo = db.LoadFromDB<DataModelTwo>(Strings.app_db_name_two);
            }
            catch {
                throw new IOException();
            }
            finally {
                db.Dispose();
                fileLock.ReleaseLock();
            }

            if (tableOne == null || tableTwo == null) {
                throw new IOException();
            };

            dataCollection.tableOne.Clear();
            dataCollection.tableOne.AddRange(tableOne);

            dataCollection.tableTwo.Clear();
            dataCollection.tableTwo.AddRange(tableTwo);
        }

        public static async Task SaveFile(string fileName, DataCollection dataCollection, bool overwrite = false) {

            try {
                await fileLock.SetLock(fileName);
            }
            catch (OperationCanceledException) {
                throw new OperationCanceledException();
            }

            if (overwrite) File.Delete(fileName);

            using (var db = new DatabaseHelper(fileName)) {

                if (!File.Exists(fileName)) {
                    db.SaveToDB(Strings.app_db_name_one, dataCollection.tableOne.ToList());
                    db.SaveToDB(Strings.app_db_name_two, dataCollection.tableTwo.ToList());
                }
                else {

                    var deletedItems = Chronos.Get.GetItems()
                                      .Where(i => i.IsDeleted())  
                                      .Select(x => x.GetId())
                                      .ToList();
                                        
                    if (deletedItems?.Count > 0) {
                        db.RemoveFromDB<DataModelOne>(Strings.app_db_name_one, deletedItems);
                        db.RemoveFromDB<DataModelTwo>(Strings.app_db_name_two, deletedItems);
                    }
                    
                    db.SaveToDB(Strings.app_db_name_one, dataCollection.tableOne
                                .Where(x => x.GetAction() == PendingAction.Update).ToList());

                    db.SaveToDB(Strings.app_db_name_two, dataCollection.tableTwo
                                .Where(x => x.GetAction() == PendingAction.Update).ToList());

                }
                db.SetVersion(DATABASE_VERSION);
            }
            fileLock.ReleaseLock();
        }
    }
}

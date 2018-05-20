using System;
using LiteDB;

namespace MVVMSample.Models
{
    public class DataModelTwo : BaseModel {

        #region Properties

        public string name { get; set; } = string.Empty;
        public string project { get; set; } = string.Empty;
        public DateTime deliveryDate { get; set; }

        #endregion

        #region Contructor

        public static DataModelTwo New() {
            return new DataModelTwo() {
                id = ObjectId.NewObjectId(),
                pendingAction = PendingAction.Update
            };
        }

        public DataModelTwo() {
        }

        public DataModelTwo(DataModelTwo fromObject, bool createNewId = true) {
            CopyValuesFrom(fromObject);
            if (createNewId) id = ObjectId.NewObjectId();
            SetAction(fromObject.pendingAction);
        }

        #endregion

        #region Methods

        #endregion
    }
}

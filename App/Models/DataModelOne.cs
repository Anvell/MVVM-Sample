using LiteDB;
using PropertyChanged;

namespace MVVMSample.Models {

    public class DataModelOne : BaseModel {

        #region Properties

        public string name { get; set; } = string.Empty;
        public string company { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public bool optionOne { get; set; }
        public bool optionTwo { get; set; }
        public string comment { get; set; } = string.Empty;

        #endregion

        #region Contructor

        public static DataModelOne New() {
            return new DataModelOne() {
                id = ObjectId.NewObjectId(),
                pendingAction = PendingAction.Update
            };
        }

        public DataModelOne() {

        }

        public DataModelOne(DataModelOne fromObject, bool createNewId = true) {
            CopyValuesFrom(fromObject);
            if (createNewId) id = ObjectId.NewObjectId();
            SetAction(fromObject.pendingAction);
        }

        #endregion

        #region Methods

        #endregion

    }
}

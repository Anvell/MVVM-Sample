using MVVMSample.Models;

namespace MVVMSample.Interfaces
{
    interface IRecord {
        void Revert();
        BaseModel GetItem();
    }
}

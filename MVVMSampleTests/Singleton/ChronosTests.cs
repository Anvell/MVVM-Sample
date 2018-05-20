using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVMSample.Models;

namespace MVVMSample.Singleton.Tests
{

    [TestClass()]
    public class ChronosTests {

        DataModelOne data;
        string prev1 = "None";
        string prev2 = "Test Name";

        [TestInitialize()]
        public void Initialize() {
            data = DataModelOne.New();
            Chronos.Get.Add(data, nameof(data.name), prev1);
            Chronos.Get.Add(data, nameof(data.name), prev2);
        }

        [TestCleanup()]
        public void Cleanup() {
            Chronos.Get.ClearUndo();
        }

        [TestMethod()]
        public void AddTest() {
            Assert.IsFalse(Chronos.Get.IsEmpty());
        }

        [TestMethod()]
        public void GetItemsTest() {
            var items = Chronos.Get.GetItems();

            Assert.AreEqual(2, items.Count);
        }

        [TestMethod()]
        public void RevertTest() {
            Chronos.Get.Revert();
            Assert.AreEqual("Test Name", data.name);
        }
    }
}
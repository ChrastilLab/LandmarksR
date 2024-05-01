using LandmarksR.Scripts.Experiment.Data;
using NUnit.Framework;
namespace LandmarksR.Scripts.Tests.Data
{

    [TestFixture]
    public class ColumnNameMapTests
    {
        private ColumnNameMap _columnNameMap;

        [SetUp]
        public void Setup()
        {
            _columnNameMap = new ColumnNameMap();
        }

        [Test]
        public void TryAdd_ShouldAddCorrectly()
        {
            var result = _columnNameMap.TryAdd("Column1", 1);
            Assert.IsTrue(result, "Should be able to add new column.");

            int index;
            var getIndexResult = _columnNameMap.TryGetIndex("Column1", out index);
            Assert.IsTrue(getIndexResult, "Should retrieve index.");
            Assert.AreEqual(1, index, "Index should be 1.");
        }

        [Test]
        public void TryAdd_DuplicateName_ShouldReturnFalse()
        {
            _columnNameMap.TryAdd("Column1", 1);
            var result = _columnNameMap.TryAdd("Column1", 2);
            Assert.IsFalse(result, "Should not add duplicate name.");
        }

        [Test]
        public void TryAdd_DuplicateIndex_ShouldReturnFalse()
        {
            _columnNameMap.TryAdd("Column1", 1);
            var result = _columnNameMap.TryAdd("Column2", 1);
            Assert.IsFalse(result, "Should not add duplicate index.");
        }
    }

}

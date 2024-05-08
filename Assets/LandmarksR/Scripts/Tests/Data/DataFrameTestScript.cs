#nullable enable
using NUnit.Framework;
using LandmarksR.Scripts.Experiment.Data;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarksR.Scripts.Tests.Data
{
    [TestFixture]
    public class DataFrameTests
    {
        [Test]
        public void Constructor_InitializesEmptyDataFrame()
        {
            var df = new DataFrame();
            Assert.AreEqual(0, df.RowCount);
            Assert.AreEqual(0, df.ColumnCount);
        }
        [Test]
        public void Constructor_Copy()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.SetColumnNames(new List<string> { "A", "B", "C" });
            var df2 = new DataFrame(df);
            Assert.AreEqual(1, df2.RowCount);
            Assert.AreEqual(3, df2.ColumnCount);
            Assert.AreEqual(1, df2[0, 0]);
            Assert.AreEqual(2, df2[0, 1]);
            Assert.AreEqual(3, df2[0, 2]);
        }

        [Test]
        public void Constructor_Empty_WithColumnNames()
        {

            var df = new DataFrame(new List<string> { "A", "B", "C"});
            Debug.Log(df);
            Assert.AreEqual(0, df.RowCount);
            Assert.AreEqual(3, df.ColumnCount);

            df.AppendRow(new List<object?> { 1, 2, 3 });
            Debug.Log(df);
            Assert.AreEqual(1, df.RowCount);
            Assert.AreEqual(3, df.ColumnCount);
        }

        [Test]
        public void AppendRow_AddsRowToEmptyDataFrame()
        {
            var df = new DataFrame();
            var row = new List<object?> { 1, "test", null };
            df.AppendRow(row);
            Assert.AreEqual(1, df.RowCount);
            Assert.AreEqual(row.Count, df.ColumnCount);
        }

        [Test]
        public void AppendRow_ThrowsExceptionIfColumnMismatch()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2 });
            Assert.Throws<System.ArgumentException>(() => df.AppendRow(new List<object?> { 1 }));
        }

        [Test]
        public void AppendColumn_AddsColumnToEmptyDataFrame()
        {
            var df = new DataFrame();
            var column = new List<object?> { 1, 2, 3 };
            df.AppendColumn(column, "NewColumn");
            Assert.AreEqual(3, df.RowCount);
            Assert.AreEqual(1, df.ColumnCount);
        }

        [Test]
        public void AppendColumn_ThrowsExceptionIfRowCountMismatch()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            var column = new List<object?> { 1, 2 };
            Assert.Throws<System.ArgumentException>(() => df.AppendColumn(column, "NewColumn"));
        }

        [Test]
        public void GetRow_ReturnsCorrectRow()
        {
            var df = new DataFrame();
            var row1 = new List<object?> { 1, 2, 3 };
            var row2 = new List<object?> { 4, 5, 6 };
            df.AppendRow(row1);
            df.AppendRow(row2);
            var retrievedRow = df.GetRow(1);
            Assert.AreEqual(row2, retrievedRow.Rows[0]);
        }

        [Test]
        public void GetRow_ThrowsExceptionIfIndexOutOfRange()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            Assert.Throws<System.IndexOutOfRangeException>(() => df.GetRow(2));
        }

        [Test]
        public void GetColumn_ReturnsCorrectColumn()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.AppendRow(new List<object?> { 4, 5, 6 });
            var column = df.GetColumn(1);
            Assert.AreEqual(2, column.Rows[0][0]);
            Assert.AreEqual(5, column.Rows[1][0]);
        }

        [Test]
        public void GetColumn_ThrowsExceptionIfIndexOutOfRange()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            Assert.Throws<System.IndexOutOfRangeException>(() => df.GetColumn(3));
        }

        [Test]
        public void Indexer_ByIndex_ReturnsCorrectValue()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { "a", "b", "c" });
            Assert.AreEqual("b", df[0, 1]);
        }

        [Test]
        public void Indexer_ByIndex_ThrowsExceptionIfOutOfRange()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { "a", "b", "c" });
            Assert.Throws<System.IndexOutOfRangeException>(() => _ = df[0, 3]);
        }

        [Test]
        public void Indexer_ByHeader_ReturnsCorrectValue()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { "a", "b", "c" });
            df.SetColumnNames(new List<string> { "Col1", "Col2", "Col3" });
            Assert.AreEqual("b", df[0, "Col2"]);
        }

        [Test]
        public void Indexer_ByHeader_ThrowsExceptionIfHeaderNotFound()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { "a", "b", "c" });
            df.SetColumnNames(new List<string> { "Col1", "Col2", "Col3" });
            Assert.Throws<System.ArgumentException>(() => _ = df[0, "Col4"]);
        }

        [Test]
        public void GetFirstElement_ReturnsCorrectValue()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.SetColumnNames(new List<string> { "A", "B", "C" });
            Assert.AreEqual(1, df.GetFirstInColumn<int>("A"));

            var df2 = new DataFrame();
            df2.AppendRow(new List<object?> { 1, 2, 3 });
            Assert.AreEqual(2, df2.GetFirstInColumn<int>("X.1"));
        }

        [Test]
        public void GetColumnRange_ReturnsCorrectColumns()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.AppendRow(new List<object?> { 4, 5, 6 });
            var columns = df.GetColumnRange(1, 2);
            Assert.AreEqual(2, columns.ColumnCount);
            Assert.AreEqual(2, columns.RowCount);
            Assert.AreEqual(2, columns[0, 0]);
            Assert.AreEqual(5, columns[1, 0]);
            Assert.AreEqual(3, columns[0, 1]);
            Assert.AreEqual(6, columns[1, 1]);

        }

        [Test]
        public void SetValue_SetsCorrectValue()
        {
            var df = new DataFrame();
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.SetValue(0, 1, 5);
            Debug.Log(df);
            Assert.AreEqual(5, df[0, 1]);
        }

        [Test]
        public void SetValue_AtNullColumn_SetsCorrectValue()
        {
            var df = new DataFrame( new List<string> { "A", "B", "C" });
            df.AppendRow(new List<object?> { 1, 2, 3 });
            df.SetValue(0, "B", 5);
            Debug.Log(df);
            Assert.AreEqual(5, df[0, 1]);
        }

        [Test]
        public void Merge_CombinesTwoDataFramesHorizontally()
        {
            var df1 = new DataFrame();
            df1.AppendRow(new List<object?> { 1, 2 });
            df1.SetColumnNames(new List<string> { "A", "B" });

            var df2 = new DataFrame();
            df2.AppendRow(new List<object?> { 3 });
            df2.SetColumnNames(new List<string> { "C" });

            var merged = df1.Merge(df2, MergeType.Horizontal);
            Debug.Log(merged);
            Assert.AreEqual(3, merged.ColumnCount);
        }

        [Test]
        public void Merge_CombinesEmptyOrPartialDataFramesHorizontally()
        {
            var df1 = new DataFrame();
            var df2 = new DataFrame();
            var merged = df1.Merge(df2, MergeType.Horizontal);
            Debug.Log(merged);
            Assert.AreEqual(0, merged.ColumnCount);

            df1.AppendRow(new List<object?> { 1, 2 });
            var merged2 = df1.Merge(df2, MergeType.Horizontal);
            Debug.Log(merged2);
            Assert.AreEqual(2, merged2.ColumnCount);

            var merged3 = df2.Merge(df1, MergeType.Horizontal);
            Debug.Log(merged3);
            Assert.AreEqual(2, merged3.ColumnCount);
        }

        [Test]
        public void Merge_CombinesTwoDataFramesVertically()
        {
            var df1 = new DataFrame();
            df1.AppendRow(new List<object?> { 1, 2 });
            df1.SetColumnNames(new List<string> { "A", "B" });

            var df2 = new DataFrame();
            df2.AppendRow(new List<object?> { 3, 4 });
            df2.SetColumnNames(new List<string> { "C", "D" });

            var merged = df1.Merge(df2, MergeType.Vertical);
            Debug.Log(merged);
            Assert.AreEqual(2, merged.RowCount);
        }

        [Test]
        public void Merge_CombinesEmptyOrPartialDataFramesVertically()
        {
            var df1 = new DataFrame();
            var df2 = new DataFrame();
            var merged = df1.Merge(df2, MergeType.Vertical);
            Debug.Log(merged);
            Assert.AreEqual(0, merged.RowCount);

            df1.AppendColumn(new List<object?> { 1, 2 }, "X");
            var merged2 = df1.Merge(df2, MergeType.Vertical);
            Debug.Log(merged2);
            Assert.AreEqual(2, merged2.RowCount);

            var merged3 = df2.Merge(df1, MergeType.Vertical);
            Debug.Log(merged3);
            Assert.AreEqual(2, merged3.RowCount);
        }

        [Test]
        public void Merge_ThrowsExceptionIfRowCountMismatch()
        {
            var df1 = new DataFrame();
            df1.AppendRow(new List<object?> { 1, 2 });
            var df2 = new DataFrame();
            df2.AppendRow(new List<object?> { 3 });
            df2.AppendRow(new List<object?> { 4 });

            Assert.Throws<System.ArgumentException>(() => df1.Merge(df2, MergeType.Horizontal));
        }

    }
}

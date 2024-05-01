#nullable enable
using System;
using System.Collections.Generic;
using LandmarksR.Scripts.Experiment.Data;
using NUnit.Framework;

namespace LandmarksR.Scripts.Tests.Data
{

    [TestFixture]
    public class DataFrameBuilderTests
    {
            [Test]
    public void AddRow_AddsRowCorrectly()
    {
        // Arrange
        var builder = new DataFrameBuilder();

        // Act
        builder.AddRow(1, "John", 30);
        builder.AddRow(2, "Alice", 25);

        var dataFrame = builder.Build();

        // Assert
        Assert.AreEqual(2, dataFrame.RowCount);
        Assert.AreEqual(3, dataFrame.ColumnCount);
        Assert.AreEqual(1, dataFrame.GetValue<int>(0, 0));
        Assert.AreEqual("John", dataFrame.GetValue<string>(0, 1));
        Assert.AreEqual(30, dataFrame.GetValue<int>(0, 2));
        Assert.AreEqual(2, dataFrame.GetValue<int>(1, 0));
        Assert.AreEqual("Alice", dataFrame.GetValue<string>(1, 1));
        Assert.AreEqual(25, dataFrame.GetValue<int>(1, 2));
    }

    [Test]
    public void AddColumn_AddsColumnCorrectly()
    {
        // Arrange
        var builder = new DataFrameBuilder();

        // Act
        builder.AddColumn(1, 2);
        builder.AddColumn("John", "Alice");

        var dataFrame = builder.Build();

        // Assert
        Assert.AreEqual(2, dataFrame.RowCount);
        Assert.AreEqual(2, dataFrame.ColumnCount);
        Assert.AreEqual(1, dataFrame.GetValue<int>(0, 0));
        Assert.AreEqual(2, dataFrame.GetValue<int>(1, 0));
        Assert.AreEqual("John", dataFrame.GetValue<string>(0, 1));
        Assert.AreEqual("Alice", dataFrame.GetValue<string>(1, 1));
    }

    [Test]
    public void AddColumn_WithDifferentRowCount_ThrowsArgumentException()
    {
        // Arrange
        var builder = new DataFrameBuilder();

        // Act
        builder.AddRow(1, "John", 30);
        builder.AddRow(2, "Alice", 25);

        // Assert
        Assert.Throws<ArgumentException>(() => builder.AddColumn(1, 2, 3));
    }

    [Test]
    public void AddRow_WithDifferentColumnCount_ThrowsArgumentException()
    {
        // Arrange
        var builder = new DataFrameBuilder();

        // Act
        builder.AddColumn(1, 2, 3);
        builder.AddColumn("John", "Alice", 3);

        // Assert
        Assert.Throws<ArgumentException>(() => builder.AddRow(1, "John", 3));
    }

    [Test]
    public void AddColumn_WithHeader_AddsColumnWithHeaderCorrectly()
    {
        // Arrange
        var builder = new DataFrameBuilder();

        // Act
        builder.AddColumn("Name", new List<object?> { "John", "Alice" });

        var dataFrame = builder.Build();

        // Assert
        Assert.AreEqual(2, dataFrame.RowCount);
        Assert.AreEqual(1, dataFrame.ColumnCount);
        Assert.AreEqual("John", dataFrame.GetValue<string>(0, "Name"));
        Assert.AreEqual("Alice", dataFrame.GetValue<string>(1, "Name"));
    }

    [Test]
    public void CreateRow_CreatesDataFrameWithSingleRowCorrectly()
    {
        // Act
        var dataFrame = DataFrameBuilder.CreateRow(1, "John", 30);

        // Assert
        Assert.AreEqual(1, dataFrame.RowCount);
        Assert.AreEqual(3, dataFrame.ColumnCount);
        Assert.AreEqual(1, dataFrame.GetValue<int>(0, 0));
        Assert.AreEqual("John", dataFrame.GetValue<string>(0, 1));
        Assert.AreEqual(30, dataFrame.GetValue<int>(0, 2));
    }

    [Test]
    public void CreateColumn_CreatesDataFrameWithSingleColumnCorrectly()
    {
        // Act
        var dataFrame = new DataFrameBuilder().AddColumn("Age", new List<object?>() { 30, 25 }).Build();

        // Assert
        Assert.AreEqual(2, dataFrame.RowCount);
        Assert.AreEqual(1, dataFrame.ColumnCount);
        Assert.AreEqual(30, dataFrame.GetValue<int>(0, "Age"));
        Assert.AreEqual(25, dataFrame.GetValue<int>(1, "Age"));
    }

    [Test]
    public void FromDictionary_CreatesDataFrameFromDictionaryCorrectly()
    {
        // Arrange
        var data = new Dictionary<string, List<object?>>
        {
            { "ID", new List<object?> { 1, 2 } },
            { "Name", new List<object?> { "John", "Alice" } },
            { "Age", new List<object?> { 30, 25 } }
        };

        // Act
        var dataFrame = DataFrameBuilder.FromDictionary(data);

        // Assert
        Assert.AreEqual(2, dataFrame.RowCount);
        Assert.AreEqual(3, dataFrame.ColumnCount);
        Assert.AreEqual(1, dataFrame.GetValue<int>(0, "ID"));
        Assert.AreEqual("John", dataFrame.GetValue<string>(0, "Name"));
        Assert.AreEqual(30, dataFrame.GetValue<int>(0, "Age"));
        Assert.AreEqual(2, dataFrame.GetValue<int>(1, "ID"));
        Assert.AreEqual("Alice", dataFrame.GetValue<string>(1, "Name"));
        Assert.AreEqual(25, dataFrame.GetValue<int>(1, "Age"));
    }
    }
}

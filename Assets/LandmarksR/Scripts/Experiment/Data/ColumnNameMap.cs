using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Manages the mapping between column names and their indices.
    /// </summary>
    public class ColumnNameMap
    {
        private readonly Dictionary<string, int> nameToIndex = new();
        private readonly Dictionary<int, string> indexToName = new();

        /// <summary>
        /// Gets the count of columns in the map.
        /// </summary>
        public int Count => nameToIndex.Count;

        /// <summary>
        /// Initializes a new instance of the ColumnNameMap class.
        /// </summary>
        public ColumnNameMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ColumnNameMap class by copying another ColumnNameMap.
        /// </summary>
        /// <param name="columnNameMap">The ColumnNameMap to copy.</param>
        public ColumnNameMap(ColumnNameMap columnNameMap)
        {
            nameToIndex = new Dictionary<string, int>(columnNameMap.nameToIndex);
            indexToName = new Dictionary<int, string>(columnNameMap.indexToName);
        }

        /// <summary>
        /// Tries to add a new column name and index to the map.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="index">The column index.</param>
        /// <returns>True if the name and index were added; otherwise, false.</returns>
        public bool TryAdd(string name, int index)
        {
            if (nameToIndex.ContainsKey(name) || indexToName.ContainsKey(index))
                return false;

            nameToIndex.Add(name, index);
            indexToName.Add(index, name);

            return true;
        }

        /// <summary>
        /// Checks if the map contains a specific column name.
        /// </summary>
        /// <param name="name">The column name to check.</param>
        /// <returns>True if the map contains the name; otherwise, false.</returns>
        public bool ContainsName(string name)
        {
            return nameToIndex.ContainsKey(name);
        }

        /// <summary>
        /// Checks if the map contains a specific column index.
        /// </summary>
        /// <param name="index">The column index to check.</param>
        /// <returns>True if the map contains the index; otherwise, false.</returns>
        public bool ContainsIndex(int index)
        {
            return indexToName.ContainsKey(index);
        }

        /// <summary>
        /// Tries to get the index of a specific column name.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="index">The column index.</param>
        /// <returns>True if the index was retrieved; otherwise, false.</returns>
        public bool TryGetIndex(string name, out int index)
        {
            return nameToIndex.TryGetValue(name, out index);
        }

        /// <summary>
        /// Tries to get the name of a specific column index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <param name="name">The column name.</param>
        /// <returns>True if the name was retrieved; otherwise, false.</returns>
        public bool TryGetName(int index, out string name)
        {
            return indexToName.TryGetValue(index, out name);
        }

        /// <summary>
        /// Tries to remove a column by its index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <returns>True if the column was removed; otherwise, false.</returns>
        public bool TryRemoveByIndex(int index)
        {
            if (!indexToName.TryGetValue(index, out var name))
                return false;

            nameToIndex.Remove(name);
            indexToName.Remove(index);
            return true;
        }

        /// <summary>
        /// Tries to remove a column by its name.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns>True if the column was removed; otherwise, false.</returns>
        public bool TryRemoveByName(string name)
        {
            if (!nameToIndex.TryGetValue(name, out var index))
                return false;

            nameToIndex.Remove(name);
            indexToName.Remove(index);
            return true;
        }

        /// <summary>
        /// Removes all columns from the map.
        /// </summary>
        public void RemoveAll()
        {
            nameToIndex.Clear();
            indexToName.Clear();
        }

        /// <summary>
        /// Gets the column names in the order of their indices.
        /// </summary>
        /// <returns>A list of column names ordered by their indices.</returns>
        public List<string> GetOrderedNames()
        {
            return indexToName.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }
    }
}

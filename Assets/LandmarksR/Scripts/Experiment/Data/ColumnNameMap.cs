using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class ColumnNameMap
    {
        private readonly Dictionary<string, int> nameToIndex = new();
        private readonly Dictionary<int, string> indexToName = new();

        public int Count => nameToIndex.Count;

        public ColumnNameMap()
        {
        }

        public ColumnNameMap(ColumnNameMap columnNameMap)
        {
            nameToIndex = new Dictionary<string, int>(columnNameMap.nameToIndex);
            indexToName = new Dictionary<int, string>(columnNameMap.indexToName);
        }

        public bool TryAdd(string name, int index)
        {
            if (nameToIndex.ContainsKey(name) || indexToName.ContainsKey(index))
                return false;

            nameToIndex.Add(name, index);
            indexToName.Add(index, name);

            return true;
        }

        public bool ContainsName(string name)
        {
            return nameToIndex.ContainsKey(name);
        }

        public bool ContainsIndex(int index)
        {
            return indexToName.ContainsKey(index);
        }

        public bool TryGetIndex(string name, out int index)
        {
            return nameToIndex.TryGetValue(name, out index);
        }

        public bool TryGetName(int index, out string name)
        {
            return indexToName.TryGetValue(index, out name);
        }

        public bool TryRemoveByIndex(int index)
        {
            if (!indexToName.TryGetValue(index, out var name))
                return false;

            nameToIndex.Remove(name);
            indexToName.Remove(index);
            return true;
        }

        public bool TryRemoveByName(string name)
        {
            if (!nameToIndex.TryGetValue(name, out var index))
                return false;

            nameToIndex.Remove(name);
            indexToName.Remove(index);
            return true;
        }

        public void RemoveAll()
        {
            nameToIndex.Clear();
            indexToName.Clear();
        }

        public List<string> GetOrderedNames()
        {
            // Sort by index
            return indexToName.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }
    }


}

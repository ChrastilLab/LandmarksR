#nullable enable
using System;
using System.Collections.Generic;

namespace LandmarksR.Scripts.Experiment.Data
{
    public static class DataUtilities
    {

        public static bool CheckEqualLength(IReadOnlyList<List<object?>> values, out int length)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            length = -1;
            foreach (var sequence in values)
            {
                if (length == -1)
                    length = sequence.Count;
                else if (length != sequence.Count)
                    return false;
            }

            if (length == -1)
                throw new ArgumentException("Values cannot be empty.");

            return true;
        }

    }
}

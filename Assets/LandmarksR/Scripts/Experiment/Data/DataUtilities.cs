#nullable enable
using System;
using System.Collections.Generic;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Utility class for data-related operations.
    /// </summary>
    public static class DataUtilities
    {
        /// <summary>
        /// Checks if all sequences in the list have equal length.
        /// </summary>
        /// <param name="values">The list of sequences to check.</param>
        /// <param name="length">The length of the sequences if they are all equal, otherwise -1.</param>
        /// <returns>True if all sequences have equal length, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the values parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the values list is empty.</exception>
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

using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Enum representing the type of table.
    /// </summary>
    public enum TableType
    {
        Text,
        Joined
    }

    /// <summary>
    /// Abstract base class for tables used in the experiment.
    /// </summary>
    public abstract class Table : BaseTask
    {
        /// <summary>
        /// The data contained in the table.
        /// </summary>
        public DataFrame Data { get; protected set; }

        /// <summary>
        /// The count of rows in the table.
        /// </summary>
        public virtual int Count { get; protected set; }

        /// <summary>
        /// The enumerator for the data in the table.
        /// </summary>
        public DataEnumerator Enumerator { get; protected set; }

        /// <summary>
        /// Unity Start method. Initializes the DataFrame.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            Data = new DataFrame();
        }

        /// <summary>
        /// Prepares the table for use.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Functional);
            base.Prepare();
        }
    }
}

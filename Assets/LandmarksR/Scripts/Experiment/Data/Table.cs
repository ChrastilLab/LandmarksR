using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    public enum TableType
    {
        Text,
        Joined
    }
    public abstract class Table : BaseTask
    {
        public virtual int Count { get; protected set; }
        public IDataEnumerator Enumerator { get; protected set; }

        protected override void Prepare()
        {
            base.Prepare();
            isRunning = false;
        }
    }
}

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
        public DataFrame Data { get; protected set; }
        public virtual int Count { get; protected set; }
        public DataEnumerator Enumerator { get; protected set; }

        protected override void Start()
        {
            base.Start();
            Data = new DataFrame();
        }

        protected override void Prepare()
        {
            base.Prepare();
            isRunning = false;
        }
    }
}

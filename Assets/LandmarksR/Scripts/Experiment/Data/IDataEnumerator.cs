using System;

namespace LandmarksR.Scripts.Experiment.Data
{
    public interface IDataEnumerator
    {
        public bool MoveNext();
        public void Reset();
        public DataFrame GetCurrent();
    }
}

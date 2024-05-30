using System.Collections.Generic;
using LandmarksR.Scripts.Experiment.Tasks.Interactive;
using LandmarksR.Scripts.Experiment.Tasks.Structural;
using UnityEngine.Assertions;

namespace PerspectiveTransformation.Scripts
{
    public class ITI : InstructionTask
    {

        private RepeatTask repeatTask;
        protected override void Prepare()
        {
            repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, "ITI must be a child of Repeat Task");

            base.Prepare();
        }

        public override void Finish()
        {
            base.Finish();
            repeatTask.Context.TryAdd("ITI", $"{elapsedTime}");
        }
    }
}

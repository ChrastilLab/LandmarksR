using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class NavigationTask : BaseTask
    {
        private Hud _hud;
        private PlayerController _playerController;
        // private RepeatTask _parentRepeatTask;
        [NotEditable, SerializeField] private string target;


        protected override void Prepare()
        {
            base.Prepare();

            if (transform.parent.TryGetComponent<RepeatTask>(out var repeatTask))
                // _parentRepeatTask = repeatTask;
                target = repeatTask.table.GetValue("Target");
            else
                DebugLogger.Instance.I("task", "Parent task is not a repeat task.");


            _hud = Experiment.Instance.playerController.hud;
            _hud.SetTitle("Navigation Task")
                .SetContent($"You will have {timer} seconds to find the target: {target}")
                .ShowAll()
                .HideAllAfter(3f);


            _playerController = Experiment.Instance.playerController;
            _playerController.TryEnableDesktopInput(3f);
            _playerController.playerEventController.RegisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }

        private void HandlePlayerTriggerEnter(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("Target")) return;

            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            _playerController.DisableDesktopInput();
            _playerController.playerEventController.UnregisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }
    }
}

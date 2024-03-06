using LandmarksR.Scripts.Player;
using UnityEngine;
using LandmarksR.Scripts.Experiment;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class TestingTask : BaseTask
    {
        protected static Config Config => Config.Instance;
        protected static Experiment Experiment => Experiment.Instance;
        protected static PlayerController Player => Experiment.Instance.playerController;
        protected static Hud Hud => Experiment.Instance.playerController.hud;

        protected void Update()
        {
            if (!isRunning) return;
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                isRunning = false;
            }

            HandleAlphaNumbers();
        }

        // Handle Alpha0 - Alpha9 input
        private void HandleAlphaNumbers()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Alpha0();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Alpha1();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Alpha2();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Alpha3();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Alpha4();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Alpha5();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Alpha6();
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Alpha7();
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Alpha8();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Alpha9();
            }
        }

        // Alpha0 - 9 Handling Methods, needs to be override
        protected virtual void Alpha0()
        {
        }

        protected virtual void Alpha1()
        {
        }

        protected virtual void Alpha2()
        {
        }

        protected virtual void Alpha3()
        {
        }

        protected virtual void Alpha4()
        {
        }

        protected virtual void Alpha5()
        {
        }

        protected virtual void Alpha6()
        {
        }

        protected virtual void Alpha7()
        {
        }

        protected virtual void Alpha8()
        {
        }

        protected virtual void Alpha9()
        {
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace LandmarksR.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public SubController vrController;
        [SerializeField] public SubController desktopController;
        [SerializeField] public Canvas canvas;

        public void SwitchDisplayMode(DisplayMode displayMode)
        {
            switch (displayMode)
            {
                case DisplayMode.Desktop:
                    vrController.gameObject.SetActive(false);
                    desktopController.gameObject.SetActive(true);
                    canvas.worldCamera = desktopController.mainCamera;
                    break;
                case DisplayMode.VR:
                    desktopController.gameObject.SetActive(false);
                    vrController.gameObject.SetActive(true);
                    canvas.worldCamera = vrController.mainCamera;

                    // StartCoroutine(StartXR());
                    StartXR();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartXR()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }

            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("No XR Loader is active. Please ensure that your XR project is set up correctly.");
            }
            else
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        private void StopXR()
        {
            if (!XRGeneralSettings.Instance.Manager.isInitializationComplete) return;
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        private void OnApplicationQuit()
        {
            StopXR();
        }
    }
}

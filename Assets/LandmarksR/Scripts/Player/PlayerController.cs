using System;
using System.Collections;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment;
using UnityEngine;
using UnityEngine.XR.Management;

namespace LandmarksR.Scripts.Player
{

    public enum DisplayMode
    {
        Desktop,
        VR
    }
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public PlayerControllerReference vrPlayerControllerReference;
        [SerializeField] public PlayerControllerReference desktopPlayerControllerReference;
        [SerializeField] public Hud hud;
        [NotEditable, SerializeField] public PlayerEventController playerEventController;
        [NotEditable, SerializeField] private FirstPersonController firstPersonController;

        // [SerializeField] public Canvas canvas;

        private Config _config;

        public void Start()
        {
            _config = Config.Instance;
            SwitchDisplayMode(_config.DisplayMode);
        }

        public void Teleport(Vector3 position, Vector3 rotation)
        {
            if (firstPersonController)
            {
                firstPersonController.Teleport(position, rotation);
            }
        }

        public void SwitchDisplayMode(DisplayMode displayMode)
        {
            switch (displayMode)
            {
                case DisplayMode.Desktop:
                    vrPlayerControllerReference.gameObject.SetActive(false);
                    desktopPlayerControllerReference.gameObject.SetActive(true);
                    hud.SetCamera(desktopPlayerControllerReference.mainCamera);

                    firstPersonController = desktopPlayerControllerReference.GetComponent<FirstPersonController>();
                    playerEventController = desktopPlayerControllerReference.GetComponent<PlayerEventController>();

                    break;
                case DisplayMode.VR:
                    desktopPlayerControllerReference.gameObject.SetActive(false);
                    vrPlayerControllerReference.gameObject.SetActive(true);
                    hud.SetCamera(vrPlayerControllerReference.mainCamera);

                    firstPersonController = null;
                    playerEventController = vrPlayerControllerReference.GetComponent<PlayerEventController>();

                    StartXR();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void TryEnableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = true;
        }

        public void TryEnableDesktopInput(float delay)
        {
            StartCoroutine(TryEnableDesktopInputCoroutine(delay));
        }

        private IEnumerator TryEnableDesktopInputCoroutine(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            TryEnableDesktopInput();
        }

        public void DisableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = false;
        }

        private static void StartXR()
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

        private static void StopXR()
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

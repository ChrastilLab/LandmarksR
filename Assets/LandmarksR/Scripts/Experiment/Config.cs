using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{



    public class Config : MonoBehaviour
    {
        public static Config Instance => _instance ??= BuildConfig();
        private static Config _instance;

        private static Config BuildConfig()
        {
            return new GameObject("Config").AddComponent<Config>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        [SerializeField] private DisplayMode displayMode = DisplayMode.Desktop;
        [SerializeField] private HudMode hudMode = HudMode.Follow;

        public DisplayMode DisplayMode {get => displayMode; private set => displayMode = value;}
        public HudMode HudMode {get => hudMode; private set => hudMode = value;}

        public void UpdateDisplayMode(DisplayMode mode, PlayerController playerController)
        {
            DisplayMode = mode;
            playerController.SwitchDisplayMode(mode);
        }

        public void UpdateHudMode(HudMode mode, Hud hud)
        {
            HudMode = mode;
            hud.SwitchHudMode(mode);
        }


    }
}

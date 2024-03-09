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
        [SerializeField] private float hudDistance = 1.5f;
        [SerializeField] private Vector2 hudScreenSize = new Vector2(1920f, 1080f);

        public DisplayMode DisplayMode {get => displayMode; private set => displayMode = value;}
        public HudMode HudMode {get => hudMode; private set => hudMode = value;}

        public float HudDistance {get => hudDistance; set => hudDistance = value;}
        public Vector2 HudScreenSize {get => hudScreenSize; set => hudScreenSize = value;}

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

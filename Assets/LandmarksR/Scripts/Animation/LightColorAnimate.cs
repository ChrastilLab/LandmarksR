using UnityEngine;

namespace LandmarksR.Scripts.Animation
{
    public class LightColorAnimate : MonoBehaviour
    {
        public float speed = 2f;
        public Color color = Color.white;
        private Light _light;
        private float _time;

        private void Start()
        {
            _light = GetComponent<Light>();
        }

        private void Update()
        {
            _time += Time.deltaTime * speed * 0.05f;

            // rainbow color
            _light.color = Color.HSVToRGB(Mathf.PingPong(_time, 1), 1, 1);

            // _light.color = Color.Lerp(Color.white, color, Mathf.PingPong(_time, 1));

        }
    }
}

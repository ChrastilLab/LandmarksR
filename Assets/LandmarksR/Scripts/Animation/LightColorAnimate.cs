using UnityEngine;

namespace LandmarksR.Scripts.Animation
{
    /// <summary>
    /// Animates the color of a light component through a rainbow spectrum.
    /// </summary>
    public class LightColorAnimate : MonoBehaviour
    {
        /// <summary>
        /// The speed of the color change animation.
        /// </summary>
        public float speed = 2f;

        private Light _light;
        private float _time;

        /// <summary>
        /// Initializes the light component.
        /// </summary>
        private void Start()
        {
            _light = GetComponent<Light>();
        }

        /// <summary>
        /// Updates the light color each frame.
        /// </summary>
        private void Update()
        {
            _time += Time.deltaTime * speed * 0.05f;

            // Update the light color to a rainbow color.
            _light.color = Color.HSVToRGB(Mathf.PingPong(_time, 1), 1, 1);
        }
    }
}

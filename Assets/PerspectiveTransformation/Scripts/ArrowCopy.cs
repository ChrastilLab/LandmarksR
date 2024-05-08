using System;
using UnityEngine;

namespace PerspectiveTransformation.Scripts
{
    public class ArrowCopy : MonoBehaviour
    {

        public GameObject arrowToCopy;
        public GameObject arrow;


        private void Update()
        {
            // make sure this y is always 100 greater than the arrowToCopy on the y-axis
            arrow.transform.position = new Vector3(arrowToCopy.transform.position.x, arrowToCopy.transform.position.y + 100, arrowToCopy.transform.position.z);


            // make sure the arrow is always facing the same direction as the arrowToCopy
            arrow.transform.rotation = arrowToCopy.transform.rotation;

            // make sure the arrow is always the same active state as the arrowToCopy
            arrow.SetActive(arrowToCopy.activeSelf);
        }
    }
}

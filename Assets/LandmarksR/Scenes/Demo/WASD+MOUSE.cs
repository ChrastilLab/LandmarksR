using UnityEngine;

public class ToggleObjectVisibility : MonoBehaviour   // New class name to avoid conflict
{
    public GameObject objectToCheck;   // The object whose active state will be checked
    public string tagToHide;           // The tag of the objects that will be hidden if the objectToCheck is active

    void Update()
    {
        // Check if the objectToCheck is active in the scene
        if (objectToCheck.activeInHierarchy)
        {
            // If objectToCheck is active, find all objects with the specified tag and hide them
            GameObject[] objectsToHide = GameObject.FindGameObjectsWithTag(tagToHide);
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(false);  // Hide the objects
            }
        }
        else
        {
            // If objectToCheck is not active, find all objects with the specified tag and show them
            GameObject[] objectsToShow = GameObject.FindGameObjectsWithTag(tagToHide);
            foreach (GameObject obj in objectsToShow)
            {
                obj.SetActive(true);   // Show the objects
            }
        }
    }
}

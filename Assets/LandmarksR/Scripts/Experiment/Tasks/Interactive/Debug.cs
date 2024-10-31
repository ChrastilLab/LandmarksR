using UnityEngine;
using UnityEngine.SceneManagement;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ObjectChecker : MonoBehaviour
    {
        // Tag to search for (e.g., "Target")
        [SerializeField] private string targetTag = "Target";

        // This method will find both active and inactive objects, and activate the inactive ones
        public void CheckAndActivateObjectsWithTag(string tag)
        {
            // Find all GameObjects in the scene (both active and inactive)
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            int foundCount = 0; // To keep track of how many objects were found
            int activatedCount = 0; // To keep track of how many objects were activated

            foreach (GameObject obj in allObjects)
            {
                // Check if the object has the correct tag
                if (obj.CompareTag(tag))
                {
                    // Ensure the object is in the active scene and not in the Project window
                    if (obj.scene.isLoaded && obj.scene == SceneManager.GetActiveScene())
                    {
                        foundCount++;
                        UnityEngine.Debug.Log($"Found object with tag '{tag}': {obj.name} in scene {obj.scene.name}");

                        // If the object is inactive, activate it
                        if (!obj.activeInHierarchy)
                        {
                            obj.SetActive(true);
                            activatedCount++;
                            UnityEngine.Debug.Log($"Activated inactive object: {obj.name}");
                        }
                    }
                }
            }

            // Log the result
            UnityEngine.Debug.Log($"Found {foundCount} objects with tag '{tag}'. Activated {activatedCount} inactive objects.");
        }

        // Call this method when all objects are loaded, such as in Start or after prefabs are instantiated
        private void Start()
        {
            // Make sure all objects are loaded in the scene
            CheckAndActivateObjectsWithTag(targetTag);
        }
    }
}

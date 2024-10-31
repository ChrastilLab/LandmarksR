using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Interactive
{
    /// <summary>
    /// Represents a task where the player can freely learn.
    /// Ensures objects with the "Target" tag are hidden and objects tagged "Wall" use the material from Element 1.
    /// </summary>
    public class FreeLearningTask : InstructionTask
    {
        /// <summary>
        /// The time after which the instruction is hidden.
        /// </summary>
        [SerializeField] private float hideInstructionAfter = 3;

        /// <summary>
        /// The key to skip the task.
        /// </summary>
        [SerializeField] private KeyCode skipKey = KeyCode.Backspace;

        /// <summary>
        /// The tag of the objects to ensure they are hidden.
        /// </summary>
        [SerializeField] private string targetTag = "Target";  // Set this in the Inspector or leave as "Target"

        /// <summary>
        /// Prepares the task, enabling player input and setting up the HUD.
        /// Ensures that objects with the "Target" tag are hidden and objects with the "Wall" tag use the material from Element 1.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            UnregisterDefaultKeyHandler();

            // Ensure all objects with the "Target" tag are hidden
            HideObjectsWithTag(targetTag);

            // Change all objects tagged "Wall" to use their Element 1 material
            SetWallMaterialToElement1("Wall");

            Player.TryEnableDesktopInput();
            Player.StartPlayerLogging();

            Player.GetMainCamera().orthographic = false;
            HUD.HideAllAfter(timer <= hideInstructionAfter ? timer - 0.5f : hideInstructionAfter); // Ensure the instruction is hidden before the task ends
            PlayerEvent.RegisterKeyHandler(skipKey, Skip);
        }

        /// <summary>
        /// Finishes the task, disabling player input and clearing the HUD.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            Player.StopPlayerLogging();
            PlayerEvent.UnregisterKeyHandler(KeyCode.Backspace, Skip);
            HUD.ClearAllText();
        }

        /// <summary>
        /// Skips the task when the backspace key is pressed.
        /// </summary>
        private void Skip()
        {
            StopCurrentTask();
        }

        /// <summary>
        /// Hides all objects with the specified tag, including inactive ones.
        /// </summary>
        private void HideObjectsWithTag(string tag)
        {
            // Find all GameObjects in the scene, both active and inactive
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int count = 0;

            foreach (GameObject obj in allObjects)
            {
                // Check if the object has the correct tag
                if (obj.CompareTag(tag))
                {
                    // Deactivate the object if it's active
                    if (obj.activeInHierarchy)
                    {
                        obj.SetActive(false);
                        UnityEngine.Debug.Log($"Deactivated object: {obj.name}");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Object is already inactive: {obj.name}");
                    }
                    count++;
                }
            }

            UnityEngine.Debug.Log($"Found and deactivated {count} objects with tag '{tag}'");
        }

        /// <summary>
        /// Changes all objects with the "Wall" tag to use the material from Element 1.
        /// </summary>
        private void SetWallMaterialToElement1(string tag)
        {
            // Find all GameObjects with the specified tag
            GameObject[] walls = GameObject.FindGameObjectsWithTag(tag);
            int count = 0;

            foreach (GameObject wall in walls)
            {
                MeshRenderer wallRenderer = wall.GetComponent<MeshRenderer>();

                if (wallRenderer != null)
                {
                    // Check if the wall has more than one material (i.e., Element 1 exists)
                    if (wallRenderer.materials.Length > 1)
                    {
                        // Get the material from Element 1 and apply it as the current material
                        Material element1Material = wallRenderer.materials[1];
                        wallRenderer.material = element1Material;
                        UnityEngine.Debug.Log($"Changed material for wall: {wall.name} to Element 1 material");
                        count++;
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"Wall object {wall.name} does not have enough material elements.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Wall object {wall.name} does not have a MeshRenderer component.");
                }
            }

            UnityEngine.Debug.Log($"Changed material for {count} objects with tag '{tag}'");
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace LandmarksR.Scripts.Experiment.Tasks.Interactive
{
    /// <summary>
    /// Represents an explore task where the player can freely explore the environment.
    /// Ensures objects with the "Target" tag are visible and allows objects tagged "Wall" to revert to their original materials.
    /// </summary>
    public class ExploreTask : InstructionTask
    {
        /// <summary>
        /// The time after which the instruction is hidden.
        /// </summary>
        [SerializeField] private float hideInstructionAfter = 3;

        /// <summary>
        /// The key to skip the explore task.
        /// </summary>
        [SerializeField] private KeyCode skipKey = KeyCode.Backspace;

        /// <summary>
        /// The tag of the objects to ensure visibility.
        /// </summary>
        [SerializeField] private string targetTag = "Target";  // Set this in the Inspector or leave as "Target"

        /// <summary>
        /// The tag of the wall objects whose material will be changed.
        /// </summary>
        [SerializeField] private string wallTag = "Wall";  // Set this in the Inspector or leave as "Wall"

        /// <summary>
        /// The material to apply to objects with the "Wall" tag.
        /// </summary>
        [SerializeField] private Material newWallMaterial;  // Set this in the Inspector

        // Store the original materials for walls
        private Dictionary<GameObject, Material> originalWallMaterials = new Dictionary<GameObject, Material>();

        /// <summary>
        /// Prepares the explore task, enabling player input and setting up the HUD.
        /// Ensures that objects with the "Target" tag are visible and walls' materials are reverted back to original when needed.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            UnregisterDefaultKeyHandler();

            // Ensure all objects with the "Target" tag are visible
            ShowObjectsWithTag(targetTag);

            // Store original materials for walls and change them to new material
            ChangeWallMaterial(wallTag, newWallMaterial);

            Player.TryEnableDesktopInput();
            Player.StartPlayerLogging();

            Player.GetMainCamera().orthographic = false;
            HUD.HideAllAfter(timer <= hideInstructionAfter ? timer - 0.5f : hideInstructionAfter); // Ensure the instruction is hidden before the task ends
            PlayerEvent.RegisterKeyHandler(skipKey, Skip);
        }

        /// <summary>
        /// Finishes the explore task, disabling player input and clearing the HUD.
        /// Reverts the wall materials to their original state.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            Player.StopPlayerLogging();
            PlayerEvent.UnregisterKeyHandler(KeyCode.Backspace, Skip);
            HUD.ClearAllText();

            // Revert wall materials to original
            RevertWallMaterial(wallTag);
        }

        /// <summary>
        /// Skips the explore task when the backspace key is pressed.
        /// </summary>
        private void Skip()
        {
            StopCurrentTask();
        }

        /// <summary>
        /// Shows all objects with the specified tag, including inactive ones.
        /// </summary>
        private void ShowObjectsWithTag(string tag)
        {
            // Find all GameObjects in the scene, both active and inactive
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int count = 0;

            foreach (GameObject obj in allObjects)
            {
                // Check if the object has the correct tag
                if (obj.CompareTag(tag))
                {
                    // Activate the object if it's inactive
                    if (!obj.activeInHierarchy)
                    {
                        obj.SetActive(true);
                        UnityEngine.Debug.Log($"Activated object: {obj.name}");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Object is already active: {obj.name}");
                    }
                    count++;
                }
            }

            UnityEngine.Debug.Log($"Found and ensured visibility of {count} objects with tag '{tag}'");
        }

        /// <summary>
        /// Changes the material of all objects with the specified tag to the given material and stores their original material.
        /// </summary>
        private void ChangeWallMaterial(string tag, Material newMaterial)
        {
            // Find all GameObjects in the scene with the specified tag
            GameObject[] walls = GameObject.FindGameObjectsWithTag(tag);
            int count = 0;

            foreach (GameObject wall in walls)
            {
                Renderer wallRenderer = wall.GetComponent<Renderer>();

                if (wallRenderer != null)
                {
                    // Store the original material if not already stored
                    if (!originalWallMaterials.ContainsKey(wall))
                    {
                        originalWallMaterials.Add(wall, wallRenderer.material);
                    }

                    // Change the material of the wall
                    wallRenderer.material = newMaterial;
                    UnityEngine.Debug.Log($"Changed material for wall: {wall.name}");
                    count++;
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Wall object {wall.name} does not have a Renderer component.");
                }
            }

            UnityEngine.Debug.Log($"Changed material for {count} objects with tag '{tag}'");
        }

        /// <summary>
        /// Reverts the material of all objects with the specified tag to their original material.
        /// </summary>
        private void RevertWallMaterial(string tag)
        {
            foreach (var wall in originalWallMaterials)
            {
                Renderer wallRenderer = wall.Key.GetComponent<Renderer>();

                if (wallRenderer != null)
                {
                    // Revert to the original material
                    wallRenderer.material = wall.Value;
                    UnityEngine.Debug.Log($"Reverted material for wall: {wall.Key.name}");
                }
            }
        }
    }
}

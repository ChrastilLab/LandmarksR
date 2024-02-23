#if UNITY_EDITOR
using System.Collections.Generic;
using LandmarksR.Scripts.Experiment;
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Editor
{
    public class SetUid : EditorWindow
    {

        [MenuItem("LandmarksR/Resume/Assign Uid to Tasks")]
        private static void AssignUid()
        {
            var usedIds = new HashSet<uint>();

            // Get all the game objects in the scene
            if (FindObjectsOfType(typeof(Transform)) is not Transform[] transforms) return;
            foreach (var transform in transforms)
            {
                var renew = true;
                // Try get the Uid component and if it exists add the ID member of Uid to the usedIds hashset
                if (transform.gameObject.TryGetComponent<Task>(out var uid))
                {
                    if (uid.ID != 0)
                    {
                        usedIds.Add(uid.ID);
                        renew = false;
                    }
                }
                else
                {
                    uid = transform.gameObject.AddComponent<Task>();
                }
                // If the game object does not have a Uid component, add one
                if (renew)
                {
                    uid.ID = GetUnusedID(usedIds);
                    usedIds.Add(uid.ID);
                }

                EditorUtility.SetDirty(uid);
            }
        }

        [MenuItem("LandmarksR/Resume/Remove All Uid")]
        public static void RemoveAllUid()
        {

            if (!(FindObjectsOfType(typeof(Transform)) is Transform[] transforms)) return;
            foreach (var transform in transforms)
            {
                UnityEditor.EditorUtility.SetDirty(transform.gameObject);
                if (transform.gameObject.TryGetComponent<Task>(out var uid))
                {
                    DestroyImmediate(uid);
                }
            }

        }

        private static readonly System.Random Random = new System.Random();

        // Get a random uint
        private static uint RandomUInt() => (uint) Random.Next(1 << 30);

        private static uint GetUnusedID(ICollection<uint> usedIDs)
        {
            uint id = 0;

            do
            {
                id = RandomUInt();
            } while (id == 0 || usedIDs.Contains(id));

            return id;
        }
    }
}
#endif

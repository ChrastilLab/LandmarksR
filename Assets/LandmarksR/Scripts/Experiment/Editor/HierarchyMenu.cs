#if UNITY_EDITOR
using LandmarksR.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Editor
{
    public class HierarchyMenu: MonoBehaviour
    {
        [MenuItem("GameObject/Experiment/Config", false, 1)]
        private static void CreateRootTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<Settings>(menuCommand, "Config");
        }
    }
}

#endif

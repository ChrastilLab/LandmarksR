using LandmarksR.Scripts.Editor;
using LandmarksR.Scripts.Experiment.Tasks.Debug;
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Editor
{
    public class HierarchyMenu : MonoBehaviour
    {
        [MenuItem("GameObject/Experiment/Tasks/1. RootTask", false, 1)]
        private static void CreateRootTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<RootTask>(menuCommand, "RootTask");
        }

        [MenuItem("GameObject/Experiment/Tasks/2. CollectionTask", false, 1)]
        private static void CreateCollectionTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<CollectionTask>(menuCommand, "CollectionTask");
        }

        [MenuItem("GameObject/Experiment/Tasks/2. InstructionTask", false, 1)]
        private static void CreateInstructionTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<InstructionTask>(menuCommand, "InstructionTask");
        }


        [MenuItem("GameObject/Experiment/Tasks/4. RepeatTask", false, 1)]
        private static void CreateRepeatTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<RepeatTask>(menuCommand, "RepeatTask");
        }

        [MenuItem("GameObject/Experiment/Tasks/5. ExploreTask", false, 1)]
        private static void CreateExploreTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<ExploreTask>(menuCommand, "ExploreTask");
        }
    }
}

using LandmarksR.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Editor
{
    public class HierarchyMenu: MonoBehaviour
    {
        [MenuItem("GameObject/Experiment/1. RootTask", false, 1)]
        private static void CreateRootTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<RootTask>(menuCommand, "RootTask");
        }

        [MenuItem("GameObject/Experiment/2. InstructionTask", false, 1)]
        private static void CreateInstructionTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<InstructionTask>(menuCommand, "InstructionTask");
        }

        [MenuItem("GameObject/Experiment/3. DummyTask", false, 1)]
        private static void CreateDummyTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<DummyTask>(menuCommand, "DummyTask");
        }

        [MenuItem("GameObject/Experiment/4. RepeatTask", false, 1)]
        private static void CreateRepeatTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<RepeatTask>(menuCommand, "RepeatTask");
        }

        [MenuItem("GameObject/Experiment/5. ExploreTask", false, 1)]
        private static void CreateExploreTask(MenuCommand menuCommand)
        {
            CreateMenuItem.CreateGameObjectContextMenu<ExploreTask>(menuCommand, "ExploreTask");
        }


    }
}

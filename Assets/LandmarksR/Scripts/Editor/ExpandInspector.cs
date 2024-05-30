#if UNITY_EDITOR
using UnityEditor;

namespace LandmarksR.Scripts.Editor
{
    public class ExpandInspector: EditorWindow
    {
        /// <summary>
        /// Expands the selected GameObject in the Hierarchy window.
        /// </summary>
        [MenuItem("EditorUtility/Shortcuts/Expand Selected Tasks &q")]
        //https://stackoverflow.com/a/66366775
        private static void ExpandTasks()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var window = GetWindow(type);
            var expandCallback = type.GetMethod("SetExpandedRecursive");
            if (expandCallback != null)
                expandCallback.Invoke(window, new object[] {Selection.activeGameObject.GetInstanceID(), true});
        }
    }
}
#endif

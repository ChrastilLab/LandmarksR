#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Editor
{
    public static class CreateMenuItem
    {
        public static void CreateGameObjectContextMenu<T>(MenuCommand menuCommand, string name) where T: Component
        {
            var go = new GameObject(name);
            go.AddComponent<T>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}

#endif

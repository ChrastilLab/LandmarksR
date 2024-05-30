#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Editor
{
    public static class CreateMenuItem
    {
        /// <summary>
        /// Creates a new GameObject with a specified component attached to it.
        /// This is a help method to create a GameObject with a component attached to it.
        /// Check Assets/LandmarksR/Scripts/Tasks/Editor/hierarchyMenu.cs for example usage.
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
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

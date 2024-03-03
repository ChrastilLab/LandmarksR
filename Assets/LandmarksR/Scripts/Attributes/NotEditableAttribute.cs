﻿using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Attributes
{
#if UNITY_EDITOR
    public class NotEditableAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(NotEditableAttribute))]
    public sealed class NotEditableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#else
    public class NotEditableAttribute: System.Attribute
    {
    }
#endif
}

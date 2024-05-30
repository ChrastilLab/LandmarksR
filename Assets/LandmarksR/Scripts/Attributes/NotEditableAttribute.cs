using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Attributes
{
#if UNITY_EDITOR
    /// <summary>
    /// Attribute to make a property not editable in the Unity Inspector.
    /// </summary>
    public class NotEditableAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// Custom property drawer for the NotEditableAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(NotEditableAttribute))]
    public sealed class NotEditableDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The label of the property.</param>
        /// <returns>The height of the property.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <summary>
        /// Renders the property in the Unity Inspector.
        /// </summary>
        /// <param name="position">The position to render the property.</param>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The label of the property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#else
    /// <summary>
    /// Attribute to make a property not editable.
    /// </summary>
    public class NotEditableAttribute : System.Attribute
    {
    }
#endif
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data.Editor
{
    /// <summary>
    /// Custom property drawer for the DelimiterOption class.
    /// </summary>
    [CustomPropertyDrawer(typeof(DelimiterOption))]
    public class DelimiterOptionDrawer : PropertyDrawer
    {
        private int _selectedOption;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight - 1);
            GUI.Label(labelRect, "Choose delimiter");

            var selectedOptionRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width,
                EditorGUIUtility.singleLineHeight);
            _selectedOption = property.FindPropertyRelative("delimiterIndex").intValue;
            _selectedOption = GUI.SelectionGrid(selectedOptionRect, _selectedOption, DelimiterOption.Options,
                DelimiterOption.Options.Length);


            property.FindPropertyRelative("delimiterIndex").intValue = _selectedOption;

            if (_selectedOption == 4)
            {
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width,
                        EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("customValue"));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 10;
        }
    }
}

#endif

using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Editor
{

    [CustomPropertyDrawer(typeof(RepeatOption))]
    public class RepeatOptionDrawer: PropertyDrawer
    {
        private SerializedProperty _useTable;
        private SerializedProperty _table;
        private SerializedProperty _numberOfRepeat;

        private readonly string[] options = {"Use Table", "Use Repeat Number"};
        private int _selectedOption;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var tableRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            var numberOfRepeatRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            _useTable = property.FindPropertyRelative("useTable");
            _table = property.FindPropertyRelative("table");
            _numberOfRepeat = property.FindPropertyRelative("numberOfRepeat");

            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight - 1);
            _selectedOption = _useTable.boolValue ? 0 : 1;
            _selectedOption = GUI.SelectionGrid(labelRect, _selectedOption, options, 2);

            if (_selectedOption == 0)
            {
                EditorGUI.PropertyField(tableRect, _table);
            }
            else
            {
                EditorGUI.PropertyField(numberOfRepeatRect, _numberOfRepeat);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 10;
        }

    }
}

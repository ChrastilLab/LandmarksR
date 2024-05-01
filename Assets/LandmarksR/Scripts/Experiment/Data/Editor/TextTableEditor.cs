using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LandmarksR.Scripts.Experiment.Data.Editor
{
    [CustomEditor(typeof(TextTable))]
    public class TextTableEditor : UnityEditor.Editor
    {
        private SerializedProperty _seed;
        private SerializedProperty _randomize;
        private SerializedProperty _rows;
        private SerializedProperty _hasHeader;
        private SerializedProperty _delimiter;
        private SerializedProperty _headers;
        private SerializedProperty _dataPath;
        private SerializedProperty _debugSelectedRows;

        private void OnEnable()
        {

            _randomize = serializedObject.FindProperty("randomize");

            _rows = serializedObject.FindProperty("rows");
            _rows.isExpanded = true;

            _hasHeader = serializedObject.FindProperty("hasHeader");

            _delimiter = serializedObject.FindProperty("delimiterOption");

            _dataPath = serializedObject.FindProperty("dataPath");

            _headers = serializedObject.FindProperty("headers");
            _headers.isExpanded = true;

            _debugSelectedRows = serializedObject.FindProperty("indexesToExclude");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_randomize);
            EditorGUILayout.PropertyField(_headers, true);
            EditorGUILayout.PropertyField(_rows, true);
            EditorGUILayout.PropertyField(_hasHeader);
            EditorGUILayout.PropertyField(_delimiter);
            EditorGUILayout.PropertyField(_dataPath);
            EditorGUILayout.PropertyField(_debugSelectedRows);
            serializedObject.ApplyModifiedProperties();
            var table = (TextTable) target;
            DrawDataPathPicker(table);


        }

        private void DrawDataPathPicker(TextTable textTable)
        {

            if (GUILayout.Button("Choose"))
            {
                _dataPath.stringValue = EditorUtility.OpenFilePanel("Load Table", Directory.GetParent(Application.dataPath)?.ToString(), "");
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }

            if (GUILayout.Button("Load From File"))
            {
                // check if the path is a valid file using system io
                if (!File.Exists(_dataPath.stringValue))
                {
                    // show a dialog box
                    EditorUtility.DisplayDialog("File not found", "The path is invalid or the file does not exist", "OK");
                }
                else
                {
                    textTable.LoadFromFile();
                }
            }

            if (GUILayout.Button("Save To File"))
            {
                // check if the path is a valid file using system io
                if (!File.Exists(_dataPath.stringValue))
                {
                    // show a dialog box
                    EditorUtility.DisplayDialog("File not found", "The path is invalid or the file does not exist", "OK");
                }
                else
                {
                    LoadPopup.Popup("Confirm again", textTable.SaveToFile);
                }
            }
        }
    }

    public class LoadPopup: EditorWindow
    {
        private Action _onConfirm;
        private string _text;
        public static void Popup(string text, Action onConfirm)
        {
            var window = CreateInstance<LoadPopup>();
            // Get cursor position and set the window position to cursor position
            var cursorPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

            window.position = new Rect(cursorPosition.x, cursorPosition.y, 250, 100);
            window._onConfirm = onConfirm;
            window._text = text;
            window.ShowPopup();
        }

        private void CreateGUI()
        {
            var label = new Label(_text);
            rootVisualElement.Add(label);
            var button = new Button(() =>
            {
                _onConfirm?.Invoke();
                Close();
            })
            {
                text = "Confirm"
            };
            rootVisualElement.Add(button);

            var cancelButton = new Button(Close)
            {
                text = "Cancel"
            };
            rootVisualElement.Add(cancelButton);
        }
    }
}

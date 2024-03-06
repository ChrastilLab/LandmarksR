using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data.Editor
{
    [CustomEditor(typeof(Table))]
    public class TableEditor : UnityEditor.Editor
    {
        private SerializedProperty _rows;
        private SerializedProperty _hasHeader;
        private SerializedProperty _delimiter;
        private SerializedProperty _headers;
        private SerializedProperty _dataPath;

        private void OnEnable()
        {
            _rows = serializedObject.FindProperty("rows");
            _rows.isExpanded = true;

            _hasHeader = serializedObject.FindProperty("hasHeader");

            _delimiter = serializedObject.FindProperty("delimiter");

            _dataPath = serializedObject.FindProperty("dataPath");

            _headers = serializedObject.FindProperty("headers");
            _headers.isExpanded = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_headers, true);
            EditorGUILayout.PropertyField(_rows, true);
            EditorGUILayout.PropertyField(_hasHeader);
            EditorGUILayout.PropertyField(_delimiter);
            EditorGUILayout.PropertyField(_dataPath);
            serializedObject.ApplyModifiedProperties();
            var table = (Table) target;
            DrawDataPathPicker(table);


        }

        private void DrawDataPathPicker(Table table)
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
                    table.LoadFromFile();
                }
            }
        }
    }
}

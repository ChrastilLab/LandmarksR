using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace LandmarksR.Scripts.Editor
{
    public class DataUtility: EditorWindow
    {
        /// <summary>
        /// Opens the data folder in the file explorer.
        /// </summary>
        [MenuItem("LandmarksR/Open Data Folder")]
        private static void OpenDataFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath,
                Application.productName));
        }
    }
}
#endif

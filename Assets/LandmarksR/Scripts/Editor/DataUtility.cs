using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace LandmarksR.Scripts.Editor
{
    public class DataUtility: EditorWindow
    {
        [MenuItem("LandmarksR/Open Data Folder")]
        private static void OpenDataFolder()
        {
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath,
                Application.productName));
        }
    }
}
#endif

using System.Collections.Generic;
using LandmarksR.Scripts.Experiment.Data;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace PerspectiveTransformation.Scripts
{
    public class GenerateRandomLocations: EditorWindow
    {
        [MenuItem("PerspectiveTransformation/Generate Random Locations")]
        public static void ShowWindow()
        {
            GetWindow<GenerateRandomLocations>("Generate Random Locations");
        }


        private TextTable _textTable;

        private float minRadius = 4;
        private float maxRadius = 6;
        private float minAngle = -40f;
        private float maxAngle = 40f;
        private int startingIndex = 1;
        private int numberOfLocations = 5;

        private bool directionN;
        private bool directionS;
        private bool directionE;
        private bool directionW;

        private readonly string[] suffix = new string[2]
        {
            "TF", "FT"
        };


        private void OnGUI()
        {

            _textTable = EditorGUILayout.ObjectField("Text Table", _textTable, typeof(TextTable), true) as TextTable;
            // Draw the min and max radius fields
            minRadius = EditorGUILayout.FloatField("Min Radius", minRadius);
            maxRadius = EditorGUILayout.FloatField("Max Radius", maxRadius);
            minAngle = EditorGUILayout.FloatField("Min Angle", minAngle);
            maxAngle = EditorGUILayout.FloatField("Max Angle", maxAngle);

            startingIndex = EditorGUILayout.IntField("Starting Index", startingIndex);
            numberOfLocations = EditorGUILayout.IntField("Number of Locations", numberOfLocations);
            directionN = EditorGUILayout.Toggle("North", directionN);
            directionS = EditorGUILayout.Toggle("South", directionS);
            directionW = EditorGUILayout.Toggle("West", directionW);
            directionE = EditorGUILayout.Toggle("East", directionE);


            GUILayout.Label("Generate Random Locations", EditorStyles.boldLabel);
            if (GUILayout.Button("Generate"))
            {

                Debug.Log("Generating random locations");


                SetHeaders();

                var checkLocations = new GameObject("CheckLocations");

                var stringRows = new List<string>();
                for (var i = 0; i < numberOfLocations; i++)
                {
                    stringRows.Add(Generate(i, checkLocations));
                }

                if (_textTable != null) _textTable.SetStringRows(stringRows);
            }
        }

        private void SetHeaders()
        {
            _textTable.SetHeaders(new List<string>
            {
                "PX",
                "PY",
                "PZ",
                "RX",
                "RY",
                "RZ",
                "ORDER"
            });
        }

        private string Generate(int index, GameObject parent)
        {
            // First get the selected object
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogError("No object selected");
                return "";
            }

            // Then generate a random position around the position of the selected object but must be 4 units away from the selected object
            var randomPosition = GenerateRandomPosition(selectedObject.transform.position, minRadius, maxRadius);

            // Then create a new game object at the random position
            var newObject = new GameObject("Random Location");
            newObject.transform.position = randomPosition;

            var direction = GetRandomDirection();
            newObject.transform.eulerAngles = direction;

            // Randomly select 1 from 'T' and 'F'
            var transformation = suffix[Random.Range(0, 2)];


            // rename the object to the index
            newObject.name = $"{(index)}";

            // set parent
            newObject.transform.SetParent(parent.transform);

            return ($"{randomPosition.x},{randomPosition.y},{randomPosition.z},{direction.x},{direction.y},{direction.z},{transformation}");
        }

        private static Vector3 GenerateRandomPosition(Vector3 center, float minRadius, float maxRadius)
        {
            Debug.Log("Generating random position around " + center + " with min radius " + minRadius + " and max radius " + maxRadius);
            // Get a random direction
            var randomDirection = Random.insideUnitCircle.normalized;

            // Scale the direction by a random value within our min and max radius
            var randomDistance = Random.Range(minRadius, maxRadius);

            // Convert the 2D point to 3D and apply it to the center position
            var randomPosition = center + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;

            return randomPosition;
        }

        private void RotateRandomlyWithinRange(GameObject target, float minAngle, float maxAngle)
        {
            // Generate a random angle within the specified range
            var randomAngle = Random.Range(minAngle, maxAngle);

            // Get the current rotation in Euler angles
            var currentRotation = target.transform.eulerAngles;

            // Adjust the Y-axis rotation by adding the random angle
            currentRotation.y += randomAngle;

            // Apply the new rotation to the GameObject
            target.transform.eulerAngles = currentRotation;
        }

        private Vector3 GetRandomDirection()
        {
            var possibleDirections = new List<Vector3>();

            if (directionN) possibleDirections.Add(new Vector3(0,0,0));

            if (directionE) possibleDirections.Add(new Vector3(0,90,0));

            if (directionS) possibleDirections.Add(new Vector3(0, 180, 0));

            if (directionW) possibleDirections.Add(new Vector3(0, 270, 0));

            // randomly pick one from possibleDirection

            if (possibleDirections.Count < 1) return Vector3.zero;

            var index = Random.Range(0, possibleDirections.Count);

            return possibleDirections[index];
        }
    }
}
#endif

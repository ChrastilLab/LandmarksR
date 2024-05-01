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
        private GameObject _object;

        private float squareSideLength = 12;
        // private float minRadius = 4;
        // private float maxRadius = 6;
        // private float minAngle = -40f;
        // private float maxAngle = 40f;
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
            _object = EditorGUILayout.ObjectField("Object", _object, typeof(GameObject), true) as GameObject;


            // Draw the square side length field
            squareSideLength = EditorGUILayout.FloatField("Square Side Length", squareSideLength);
            // Draw the min and max radius fields
            // minRadius = EditorGUILayout.FloatField("Min Radius", minRadius);
            // maxRadius = EditorGUILayout.FloatField("Max Radius", maxRadius);
            // minAngle = EditorGUILayout.FloatField("Min Angle", minAngle);
            // maxAngle = EditorGUILayout.FloatField("Max Angle", maxAngle);

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

                var checkLocations = new GameObject($"{_object?.name} Locations for validation (Please delete)");

                var stringRows = new List<string>();
                for (var i = 0; i < numberOfLocations; i++)
                {
                    stringRows.Add(Generate(i, checkLocations));
                }

                if (_textTable != null) _textTable.AppendStringRows(stringRows);
            }

            if (GUILayout.Button("Clear"))
            {
                if (_textTable != null) _textTable.SetStringRows(new List<string>());
            }

            if (GUILayout.Button("Update Location of Selected Object in Locations For Validation"))
            {
                UpdateOnce();
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
            var selectedObject = _object;

            // Then generate a random position around the position of the selected object but must be 4 units away from the selected object
            // var randomPosition = GenerateRandomPositionAround(selectedObject.transform.position, minRadius, maxRadius);

            var randomPosition = GenerateRandomPositionInSquare(selectedObject.transform.position, squareSideLength);

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

        public void UpdateOnce()
        {
            if (_textTable == null)
            {
                Debug.LogError("No text table selected");
                return;
            }

            // Get the selected object
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogError("No object selected");
                return;
            }

            // Get the postion of the selected object
            var pos = selectedObject.transform.position;

            // Get the rotation of the selected object
            var rot = selectedObject.transform.eulerAngles;

            // Get the name of the selected object
            if (!int.TryParse(selectedObject.name, out var index))
            {
                Debug.LogError("Selected object does not have a valid index");
                return;
            }

            var stringRows = new List<string>(_textTable.StringRows);

            var transformation = stringRows[index].Split(',')[^1];

            stringRows[index] = $"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{transformation}";
            Debug.Log($"Updating location: {stringRows[index]}");

            _textTable.SetStringRows(stringRows);
        }

        private static Vector3 GenerateRandomPositionAround(Vector3 center, float minRadius, float maxRadius)
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

        public static Vector3 GenerateRandomPositionInSquare(Vector3 center, float sideLength)
        {
            float halfSide = sideLength / 2;
            return new Vector3(
                center.x + Random.Range(-halfSide, halfSide),
                center.y,
                center.z + Random.Range(-halfSide, halfSide)
            );
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

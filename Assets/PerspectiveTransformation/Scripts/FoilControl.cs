using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Data;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace PerspectiveTransformation.Scripts
{
    public class FoilControl : BaseTask
    {

        [SerializeField] private bool revert;
        [SerializeField] private FoilControl previousFoilControl;

        private readonly List<Material> _originalWallMaterials = new();
        private readonly Dictionary<char, int> _wallNameNumberMap = new()
        {
            {'A', 0},
            {'B', 1},
            {'C', 2},
            {'D', 3},
        };

        private readonly List<Tuple<GameObject, GameObject>> _swapPositionCache = new();
        private List<GameObject> _walls = new();
        private readonly Dictionary<string, GameObject> _targetDict = new();

        private readonly Dictionary<string, Material> _targetOriginalMaterialDict = new();
        private readonly Dictionary<string, string> _targetOriginalColorNameMap = new()
        {
            {"Sphere", "Red"},
            {"Cube", "Orange"},
            {"Cylinder", "Green"},
            {"Pyramid", "Yellow"},
            {"Capsule", "Blue"},
        };

        public new void Start()
        {
            _walls = GameObject.FindGameObjectsWithTag("Wall").OrderBy(w => w.name).ToList();
            _walls.ForEach(wall => _originalWallMaterials.Add(GetMaterial(wall)));

            if (_walls.Count != 4)
            {
                Logger.W("data", "Wall count is not 4");
            }

            foreach (var target in GameObject.FindGameObjectsWithTag("Target"))
            {
                _targetDict.Add(target.name, target);
                if (!_targetOriginalColorNameMap.TryGetValue(target.name, out var color)) continue;

                // Get the material of the target object
                // Add the material to the dictionary with the color as the key
                if (target.TryGetComponent<Renderer>(out var rnd))
                {
                    _targetOriginalMaterialDict.Add(color, rnd.material);
                }
            }
        }

        protected override void Prepare()
        {
            var repeatTask = transform.parent.GetComponent<RepeatTask>();
            Assert.IsNotNull(repeatTask, "FoilControl must be a child of RepeatTask");

            var currentData = repeatTask.CurrentData;
            Assert.IsFalse(currentData.ColumnCount < 1 || currentData.RowCount < 1, "Must have at least one row and one column in the data table");

            base.Prepare();

            var currentFoilData = repeatTask.CurrentDataByTable(0);


            if (revert)
            {
                if (previousFoilControl)
                {
                    previousFoilControl.Revert(currentFoilData);
                }
                else
                {
                    Logger.E("data", "Previous Foil Control is null, Cannot revert.");
                }
            }
            else
            {
                Show(currentFoilData);
            }
            isRunning = false;
        }


        private void Show(DataFrame foilData)
        {
            var foilType = foilData.GetFirstInColumn<string>("Type");
            switch (foilType)
            {
                case "Permute Wall":
                    Logger.I("data", "Permute Wall");
                    PermuteWall(foilData);
                    break;
                case "Swap Position":
                    Logger.I("data", "Swap Position");
                    SwapPosition(foilData);
                    break;
                case "Swap Color":
                    Logger.I("data", "Swap Color");
                    SwapColor(foilData);
                    break;
                // case "Change Color":
                //     Logger.I("data", "Change Color");
                //     ChangeColor(foilData);
                //     break;
                case "Rotate":
                    Logger.I("data", "Rotate");
                    Rotate(foilData);
                    break;
                case "Scale":
                    break;
                case "Arrow":
                    Logger.I("data", "Arrow foil should be applied");
                    break;
                case "No Foil":
                    Logger.I("data", "No Foil Revert");
                    break;
                default:
                    return;
            }
            Logger.I("data", foilData.ToString());
        }

        private void Revert(DataFrame foilData)
        {
            var foilType = foilData.GetFirstInColumn<string>("Type");
            switch (foilType)
            {
                case "Permute Wall":
                    Logger.I("data", "Permute Wall Revert");
                    PermuteWallRevert();
                    break;
                case "Swap Position":
                    Logger.I("data", "Swap Position Revert");
                    RevertSwapPosition();
                    break;
                case "Swap Color":
                    Logger.I("data", "Swap Color Revert");
                    SwapColorRevert();
                    break;
                // case "Change Color":
                //     Logger.I("data", "Change Color Revert");
                //     RevertChangeColor();
                    // break;
                case "Rotate":
                    Logger.I("data", "Rotate Revert");
                    RotateRevert();
                    break;
                case "Scale":
                    break;
                case "Arrow":
                    break;
                case "No Foil":
                    break;
                default:
                    return;
            }
        }

        private static Material GetMaterial(GameObject go)
        {
            var rnd = go.GetComponent<Renderer>();
            if (rnd)
            {
                return rnd.material;
            }

            foreach (Transform child in go.transform)
            {
                var childMaterial = GetMaterial(child.gameObject);
                if (childMaterial)
                {
                    return childMaterial;
                }
            }

            return null;
        }

        private static void SetMaterial(GameObject go, Material material)
        {
            var rnd = go.GetComponent<Renderer>();
            if (rnd)
            {
                rnd.material = material;
            }

            foreach (Transform child in go.transform)
            {
                SetMaterial(child.gameObject, material);
            }
        }
        #region permute wall
        private void PermuteWall(DataFrame foilData)
        {
            var order = foilData.GetValue<string>(0, 1);
            if (order == null)
            {
                Logger.W("data", "Permute Wall order is null");
                return;
            }

            foreach (var pair in order.ToCharArray().Zip(_walls, (c, wall) => (c, wall)))
            {
                var (c, wall) = pair;
                SetMaterial(wall, _originalWallMaterials[_wallNameNumberMap[c]]);
            }
        }


        private void PermuteWallRevert()
        {
            foreach (var pair in _originalWallMaterials.Zip(_walls, (material, wall) => (material, wall)))
            {
                var (material, wall) = pair;
                SetMaterial(wall, material);
            }
        }
        #endregion

        #region swap

        private static void SwapPosition(GameObject obj1, GameObject obj2)
        {
            (obj1.transform.position, obj2.transform.position) = (obj2.transform.position, obj1.transform.position);
        }
        private void SwapPosition(string pair)
        {
            // Get the pair of objects to swap using regex to match "object1, object2"
            var regex = new System.Text.RegularExpressions.Regex(@"(\w+),\s*(\w+)");
            var match = regex.Match(pair);
            if (!match.Success)
            {
                Logger.W("data", $"Invalid parameter {pair}");
                return;
            }

            var obj1 = _targetDict[match.Groups[1].Value];
            var obj2 = _targetDict[match.Groups[2].Value];

            // Swap the objects position

            SwapPosition(obj1, obj2);
            _swapPositionCache.Add(new Tuple<GameObject, GameObject>(obj1, obj2));
        }

        private void RevertSwapPosition()
        {
            foreach (var (obj1, obj2) in _swapPositionCache)
            {
                SwapPosition(obj1, obj2);
            }
            _swapPositionCache.Clear();
        }


        private void SwapPosition(DataFrame foilData)
        {
            var end = foilData.ColumnCount;
            Logger.I("data", $"Swap Position: {foilData}");
            for (var i = 1; i < end; i++)
            {
                var pair = foilData.GetValue<string>(0, i);
                if (pair == null || string.IsNullOrEmpty(pair) || string.IsNullOrWhiteSpace(pair))
                {
                    return;
                }
                SwapPosition(pair);
            }
        }
        # endregion

        #region swap color
        private readonly List<Tuple<GameObject, GameObject>> _swapColorCache = new();

        private static void SwapMaterial(GameObject obj1, GameObject obj2)
        {
            (obj1.GetComponent<Renderer>().material, obj2.GetComponent<Renderer>().material) = (obj2.GetComponent<Renderer>().material, obj1.GetComponent<Renderer>().material);
        }
        private void SwapColor(string pair)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(\w+),\s*(\w+)");
            var match = regex.Match(pair);
            if (!match.Success)
            {
                Logger.W("data", $"Invalid parameter Swap Color: {pair}");
                return;
            }

            var obj1 = _targetDict[match.Groups[1].Value];
            var obj2 = _targetDict[match.Groups[2].Value];

            SwapMaterial(obj1, obj2);
            _swapColorCache.Add(new Tuple<GameObject, GameObject>(obj1, obj2));
        }

        private void SwapColor(DataFrame foilData)
        {
            var end = foilData.ColumnCount;
            for (var i = 1; i < end; i++)
            {
                var pair = foilData.GetValue<string>(0, i);
                if (pair == null || string.IsNullOrEmpty(pair) || string.IsNullOrWhiteSpace(pair))
                {
                    return;
                }
                SwapColor(pair);
            }
        }

        private void SwapColorRevert()
        {
            foreach (var (obj1, obj2) in _swapColorCache)
            {
                SwapMaterial(obj1, obj2);
            }
            _swapColorCache.Clear();
        }

        #endregion

        #region rotate

        private readonly List<Tuple<GameObject, Quaternion>> _rotateCache = new();

        private void Rotate(string target, string angle)
        {
            var obj = _targetDict[target];
            var rotation = obj.transform.rotation;
            // get the first char in angle check if it is X or Y or Z
            var axis = angle[0];
            var value = float.Parse(angle[1..]);
            switch (axis)
            {
                case 'X':
                    obj.transform.Rotate(value, 0, 0);
                    break;
                case 'Y':
                    obj.transform.Rotate(0, value, 0);
                    break;
                case 'Z':
                    obj.transform.Rotate(0, 0, value);
                    break;
            }

            _rotateCache.Add(new Tuple<GameObject, Quaternion>(obj, rotation));
        }

        private void RotateRevert()
        {
            foreach (var (obj, rotation) in _rotateCache)
            {
                obj.transform.rotation = rotation;
            }
            _rotateCache.Clear();
        }

        private void Rotate(DataFrame foilData)
        {
            var target = foilData.GetValue<string>(0, 1);
            var angle = foilData.GetValue<string>(0, 2);
            if (target == null || string.IsNullOrEmpty(target) || string.IsNullOrWhiteSpace(target) ||
                angle == null || string.IsNullOrEmpty(angle) || string.IsNullOrWhiteSpace(angle))
            {
                return;
            }
            Rotate(target.Trim(), angle.Trim());
        }
        #endregion

        #region arrow

        public static Vector3 ApplyFoilDirectionToArrow(DataFrame foilData, Vector3 originalRotation)
        {
            var arrowType = foilData.GetFirstInColumn<string>("Type");
            if (string.IsNullOrEmpty(arrowType) || string.IsNullOrWhiteSpace(arrowType) || arrowType != "Arrow")
            {
                return originalRotation;
            }

            var arrowDirection = foilData.GetValue<string>(0, 1);
            if (string.IsNullOrEmpty(arrowDirection) || string.IsNullOrWhiteSpace(arrowDirection) ||
                !float.TryParse(arrowDirection, out var rotation))
            {
                Debug.LogError("Error while parsing direction with string: " + arrowDirection);
                return originalRotation;
            }

            return new Vector3(0, rotation + originalRotation.y, 0);
        }

        #endregion
    }
}

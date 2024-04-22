using LandmarksR.Scripts.Experiment.Data;
using UnityEngine;

namespace PerspectiveTransformation.Scripts
{
    public static class Utilities
    {
        public static Vector3 GetRotationFromDataFrame(DataFrame dataFrame)
        {
            return new Vector3
            {
                x = float.Parse((string)dataFrame[0, "RX"]),
                y = float.Parse((string)dataFrame[0, "RY"]),
                z = float.Parse((string)dataFrame[0, "RZ"])
            };
        }

        public static Vector3 GetPositionFromDataFrame(DataFrame dataFrame)
        {
            return new Vector3
            {
                x = float.Parse((string)dataFrame[0, "PX"]),
                y = float.Parse((string)dataFrame[0, "PY"]),
                z = float.Parse((string)dataFrame[0, "PZ"])
            };
        }

        public static string GetOrderFromDataFrame(DataFrame dataFrame)
        {
            return (string)dataFrame[0, "ORDER"];
        }

        public static string GetFoilTypeFromDataFrame(DataFrame dataFrame)
        {
            return (string)dataFrame[0, "TYPE"];
        }

    }
}

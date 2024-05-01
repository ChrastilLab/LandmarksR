using System;
using LandmarksR.Scripts.Experiment.Data;
using UnityEngine;

namespace PerspectiveTransformation.Scripts
{
    public static class Utilities
    {
        public static Vector3 GetRotationFromDataFrame(DataFrame dataFrame)
        {
            try
            {
                return new Vector3
                {
                    x = float.Parse(dataFrame.GetFirstInColumn<string>("RX")),
                    y = float.Parse(dataFrame.GetFirstInColumn<string>("RY")),
                    z = float.Parse(dataFrame.GetFirstInColumn<string>("RZ"))
                };
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new Exception("Error in GetRotationFromDataFrame");
            }
        }

        public static Vector3 GetPositionFromDataFrame(DataFrame dataFrame)
        {
            try
            {
                return new Vector3
                {
                    x = float.Parse(dataFrame.GetFirstInColumn<string>("PX")),
                    y = float.Parse(dataFrame.GetFirstInColumn<string>("PY")),
                    z = float.Parse(dataFrame.GetFirstInColumn<string>("PZ"))
                };
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new Exception("Error in GetPositionFromDataFrame");
            }
        }

        public static string GetOrderFromDataFrame(DataFrame dataFrame)
        {
            return dataFrame.GetFirstInColumn<string>("ORDER");
        }

        public static string GetFoilTypeFromDataFrame(DataFrame dataFrame)
        {
            return dataFrame.GetFirstInColumn<string>("Type");
        }
    }
}

using Framework.Scripts.Data;
using UnityEngine;

namespace Framework.Scripts.Helpers
{
    public class PathingHelper
    {
        public static PathingAlgorithm getClassByPathingType(PathFindingMethods pathFindingMethod)
        {
            PathingAlgorithm method = null;
            
            switch (pathFindingMethod)
            {
                case PathFindingMethods.FIRSTPATH:
                    method = new FirstPath();
                    break;
                case PathFindingMethods.FIRSTPATH_SHORTEST:
                    method = new FirstPathShortest();
                    break;
                case PathFindingMethods.FIRSTPATH_SHORTEST_TOLERANCE:
                    method = new FirstPathShortestWithTolerance();
                    break;
                default:
                    Debug.LogError("Invalid pathing method");
                    break;
            }

            return method;
        }
    }
}
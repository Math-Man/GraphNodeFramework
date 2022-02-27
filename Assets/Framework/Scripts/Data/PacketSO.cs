using UnityEngine;

namespace Framework.Scripts.Data
{
    [CreateAssetMenu(fileName = "PacketSO", menuName = "GraphNode/Data/Packet", order = 0)]
    public class PacketSO : ScriptableObject
    {
        public float reachDistance;
        public float speed;

        public PathFindingMethods pathFindingMethod;
        public bool snapping;
        public bool dynamicPathFinding;
        
    }

    public enum PathFindingMethods
    {
        FIRSTPATH,
        FIRSTPATH_SHORTEST,
        FIRSTPATH_SHORTEST_TOLERANCE,
    }
}
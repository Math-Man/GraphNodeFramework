using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scripts.Data
{
    [CreateAssetMenu(fileName = "GraphNodeData", menuName = "GraphNode/Data/Node Data", order = 0)]
    public class GraphNodeData : ScriptableObject
    {
        public int type;
        public float range;
        public int connectionCount;
        public List<int> connectableTypes;

    }
}
using UnityEngine;

namespace Sample.Scripts
{
    [CreateAssetMenu(fileName = "StructureSO", menuName = "GraphNode/Structure/Structure Data", order = 0)]
    public class StructureSO : ScriptableObject
    {
        public float constructionCost = 10f;
    }
}
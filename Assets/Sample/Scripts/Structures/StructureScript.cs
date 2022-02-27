using System;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Sample.Scripts
{
    [RequireComponent(typeof(GraphNode))]
    public class StructureScript : MonoBehaviour
    {
        public StructureSO structureData;
        public GraphNode node;

        private void Awake()
        {
            node = GetComponent<GraphNode>();
        }
    }
}
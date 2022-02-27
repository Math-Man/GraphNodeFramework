using System;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Sample.Colony_Sample_Project.Scripts
{
    public class BaseCore : MonoBehaviour
    {
        [HideInInspector] public GraphNode node;

        public float storedEnergy;

        private void Awake()
        {
            node = GetComponent<GraphNode>();
        }
    }
}
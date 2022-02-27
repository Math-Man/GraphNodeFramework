using System;
using System.Collections.Generic;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Sample.Scripts
{
    [RequireComponent(typeof(StructureScript))]
    public class ConstructionState : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> disabledBehaviour;
        private StructureScript structureScript;
        private float constructionProgress;

        private void Awake()
        {
            structureScript = GetComponent<StructureScript>();
            foreach (var behaviourScript in disabledBehaviour)
            {
                behaviourScript.enabled = false;
            }

            structureScript.node.packetEndCallback.AddListener(packetReached);

        }



        private void packetReached(GraphPacket packet, bool success)
        {
            print("bruh moment");

            constructionProgress++;

        }
    }
}
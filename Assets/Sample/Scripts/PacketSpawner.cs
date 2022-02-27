using System;
using System.Collections.Generic;
using Framework.Scripts.Helpers;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Sample.Scripts
{
    public class PacketSpawner : MonoBehaviour
    {
        [SerializeField] private List<GraphNode> path;
        [SerializeField] private GameObject packetPrefab;
        
        
        [SerializeField] private bool usePathFinding;
        [SerializeField] private GraphNode start;
        [SerializeField] private GraphNode end;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {

                if (usePathFinding)
                {
                    FirstPathShortest pf = new FirstPathShortest();
                    path = pf.PathFind(start, end, 10);
                }

                var packet = GameObject.Instantiate(packetPrefab).GetComponent<GraphPacket>();
                packet.path = path;
                packet.sourceNode = path[0];
                packet.destinationNode = path[path.Count - 1];
                //packet.path = new List<GraphNode>(path);
            }
                
        }
    }
}
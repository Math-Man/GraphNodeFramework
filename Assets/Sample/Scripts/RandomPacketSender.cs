using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.Model;
using Framework.Scripts.Objects;
using Framework.Scripts.Objects.GraphNodeComponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sample.Scripts
{
    [RequireComponent(typeof(PacketSpawnerComponent), typeof(GraphNode))]
    public class RandomPacketSender : MonoBehaviour
    {
        private PacketSpawnerComponent spawner;
        private GraphNode graphNode;
        
        public List<GraphNode> deliviries;

        public PacketRequestData reqData;

        private void Awake()
        {
            spawner = GetComponent<PacketSpawnerComponent>();
            graphNode = GetComponent<GraphNode>();
            deliviries = new List<GraphNode>();
            
            reqData = new PacketRequestData();
            reqData.packetStartCallback = PacketStart;
            reqData.packetReachedCallback = PacketReached;
            reqData.packetCancelledCallback = PacketCancelled;
        }

        private void PacketStart(PacketRequestResultData resData)
        {
            Debug.LogWarning("start");
            deliviries.Add(resData.ActualDestination);
        }
        
        private void PacketReached(PacketRequestResultData resData)
        {
            Debug.LogWarning(resData);
            deliviries.RemoveAt(deliviries.LastIndexOf(resData.ActualDestination));
        }

        private void PacketCancelled(PacketRequestResultData resData)
        {
            Debug.LogWarning(resData);
            deliviries.RemoveAt(deliviries.LastIndexOf(resData.ActualDestination));
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                //Pick a random node that is not itself
                var rs = GameObject.FindObjectsOfType<GraphNode>().Where(node => !node.Equals(graphNode)).ToList();
                var randomNode = rs[Random.Range((int) 0, (int) (rs.Count))];
            
                spawner.addPacketRequest(randomNode, reqData);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.Data;
using Framework.Scripts.Helpers;
using Framework.Scripts.Model;
using UnityEngine;

namespace Framework.Scripts.Objects.GraphNodeComponents
{
    [RequireComponent(typeof(GraphNode))]
    public class PacketSpawnerComponent : MonoBehaviour
    {
        [SerializeField] private PathFindingMethods pathFindingMethod;
        [SerializeField] private GameObject packetPrefab;

        //Carry these over to SO ?
        [SerializeField] private float cooldown;
        [SerializeField] private int numberOfPacketsPerSend;
        
        private HashSet<List<GraphNode>> cachedPaths;

        private Queue<RequestPair> RequestQue;
        
        private float clk;
        
        private void Awake()
        {
            clk = Time.time;
            cachedPaths = new HashSet<List<GraphNode>>();
            RequestQue = new Queue<RequestPair>();
        }

        private void Update()
        {
            if (RequestQue.Count > 0 &&  Time.time - clk >= cooldown)
            {
                for (int i = 0; i < Math.Min(numberOfPacketsPerSend, RequestQue.Count); i++)
                {
                    SendPacketTo(RequestQue.Dequeue());
                }
                clk = Time.time;
            }
            
        }

        public void addPacketRequest(GraphNode node, PacketRequestData reqData)
        {
            
            RequestQue.Enqueue(new RequestPair(node, reqData));
        }

        public bool IsPathCached(List<GraphNode> path)
        {
            foreach (var cachedPath in cachedPaths)
            {
                if (cachedPath.SequenceEqual(path))
                    return true;
            }
            return false;
        }

        private List<GraphNode> getCachedPathByDestination(GraphNode destination)
        {
            return cachedPaths.FirstOrDefault(list => list[list.Count - 1].Equals(destination));
        }

        private bool IsPathValid(List<GraphNode> path)
        {
            return !path.Contains(null) &&
                   path[0].Equals(this.GetComponent<GraphNode>());
        }

        public void ClearInvalidPaths()
        {
            cachedPaths.RemoveWhere(list => list.Contains(null));
        }

        public void CachePath(List<GraphNode> path)
        {
            cachedPaths.Add(path);
        }

        private void SendPacketTo(RequestPair destination)
        {
            var pathingAlgorithm = PathingHelper.getClassByPathingType(pathFindingMethod);

            List<GraphNode> path = null;
            var cachedPath = getCachedPathByDestination(destination.node);
            if (cachedPath != null && cachedPath.Count > 0 && IsPathValid(cachedPath))
            {
                path = cachedPath;
            }
            else
            {
                path = pathingAlgorithm.PathFind(gameObject.GetComponent<GraphNode>(), destination.node);
            }

            if (path != null && path.Count > 0 && IsPathValid(path))
            {
                var packet = GameObject.Instantiate(packetPrefab).GetComponent<GraphPacket>();
                packet.reqData = destination.reqData;
                packet.path = path;
                packet.sourceNode = path[0];
                packet.destinationNode = path[path.Count - 1];
                CachePath(path);
            }
            else
            {
                Debug.LogWarning("SendPacketTo failed, no valid path(s)");
            }
        }
    }

    public class RequestPair
    {
        public GraphNode node { get; set; }
        public PacketRequestData reqData { get; set; }

        public RequestPair(GraphNode node, PacketRequestData reqData)
        {
            this.node = node;
            this.reqData = reqData;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Scripts.Data;
using Framework.Scripts.Helpers;
using Framework.Scripts.Model;
using UnityEngine;

namespace Framework.Scripts.Objects
{
    public class GraphPacket : MonoBehaviour
    {
        [SerializeField] public PacketSO typeData;

        public GraphNode sourceNode;
        public GraphNode destinationNode;
        public List<GraphNode> path;
        private GraphNode lastNode;
        private GraphNode nextNode;
        private int nodeIndex;
        private bool isMoving;
        private PathingAlgorithm pathingAlgorithm;
        
        public PacketRequestData reqData { get; set; }

        private void Awake()
        {
            pathingAlgorithm = PathingHelper.getClassByPathingType(typeData.pathFindingMethod);
        }

        private void Start()
        {
            if (path == null || path.Count == 0)
            {
                var pathFindResult = pathingAlgorithm.PathFind(sourceNode, destinationNode);
                if (pathFindResult == null || pathFindResult.Count == 0)
                {
                    //error
                    Debug.LogWarning($"Can't find path between ${sourceNode.name} and ${destinationNode.name} for packet ${this.gameObject.name}");
                    DestroySelf(false,false);
                    return;
                }
                else
                {
                    path = pathFindResult;
                }
            }

            transform.position = path[0].rendererPivot.position;
            nodeIndex = 0;
            nextNode = path[1];
            lastNode = path[path.Count - 1];
            
            if (reqData != null)
            {
                reqData.packetStartCallback.Invoke(new PacketRequestResultData()
                {
                    Success = false, 
                    ActualDestination = destinationNode, 
                    LastNode = path[nodeIndex], 
                    StartNode = path[0]
                });
            }
            MoveToNextNode();
        }

        private void Update()
        {
            if (path == null || path.Count == 0)
                return;
            
            if (!isMoving)
            {
                if (nextNode.Equals(lastNode))
                {
                    DestroySelf(true, false);
                    return;
                }

                path[nodeIndex].PacketPassThroughCallback(this);
                    
                nodeIndex++;
                nextNode = path[nodeIndex + 1];
                
                if (typeData.snapping)
                    transform.position = path[nodeIndex].rendererPivot.position;
                
                MoveToNextNode();
            }
        }

        private void MoveToNextNode()
        {
            StartCoroutine(MoveTo());
        }

        private bool HasReachedNextNode() 
        {
            try
            {
                return Vector3.Distance(transform.position, nextNode.rendererPivot.position) < typeData.reachDistance;
            }
            catch (Exception e)
            {
                DestroySelf(false, true);
                return false;
            }
        }

        private bool HasReachedDestination()
        {
            return nextNode.Equals(lastNode) && HasReachedNextNode();
        }

        private bool TryFindPath()
        {
            var result = pathingAlgorithm.PathFind(path[nodeIndex], lastNode);

            if (result != null && result.Count > 0)
            {
                path = result;
                nodeIndex = 0;
                nextNode = path[1];
                return true;
            }

            return false;
        }

        private void DestroySelf(bool succesful, bool tryFindPathFirst)
        {
            if (tryFindPathFirst && TryFindPath())
            {
                return;
            }

            if (reqData != null)
            {
                if (succesful)
                {
                    lastNode.PacketEndedOnCallback(this, true);
                    reqData.packetReachedCallback.Invoke(new PacketRequestResultData()
                    {
                        Success = true, 
                        ActualDestination = destinationNode, 
                        LastNode = path[nodeIndex], 
                        StartNode = path[0]
                    });
                }
                else
                {
                    lastNode.PacketEndedOnCallback(this, false);
                    reqData.packetCancelledCallback.Invoke(new PacketRequestResultData()
                    {
                        Success = false, 
                        ActualDestination = destinationNode, 
                        LastNode = path[nodeIndex], 
                        StartNode = path[0]
                    });
                }
            }
            Destroy(this.gameObject);
        }

        private IEnumerator MoveTo()
        {
            isMoving = true;
            while (!HasReachedNextNode())
            {
                //Do validity check
                try
                {
                    transform.position = Vector3.MoveTowards(transform.position, nextNode.rendererPivot.position,
                        Time.deltaTime * typeData.speed);
                }
                catch (Exception e)
                {
                    DestroySelf(false, true);
                    //TODO: Handle discard
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }

            isMoving = false;
            yield break;
        }

    }
}
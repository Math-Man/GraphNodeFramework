using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.Behaviour;
using Framework.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Scripts.Objects
{
    /// <summary>
    /// Graph Node component
    /// Connects to other graph node components and handles packet movement
    /// </summary>
    [ExecuteInEditMode]
    public class GraphNode : MonoBehaviour
    {
        [SerializeField] public GraphNodeData nodeData;
        [SerializeField] public Transform rendererPivot;
        public List<GraphNode> connectedNodes { get; private set; }

        public UnityEvent<GraphPacket> packetPassCallback;
        public UnityEvent<GraphPacket, bool> packetEndCallback;

        //Rule of demeter methods
        public List<int> getConnectableTypes() => nodeData.connectableTypes;
        public int getType() => nodeData.type;
        public float getRange() => nodeData.range;
        public int getMaxConnectionsCount() => nodeData.connectionCount;

        private void Awake()
        {
            connectedNodes ??= new List<GraphNode>();
        }

        private void Start()
        {
            ConnectToClosestNodes();
        }

        private void LateUpdate()
        {
            if (transform.hasChanged)
            {
                ConnectToClosestNodes();
            
                bool collectionChanged = true;
                while (collectionChanged)
                {
                    if (connectedNodes.Count == 0) return;
                
                    foreach (var other in connectedNodes)
                    {
                        collectionChanged = UpdateConnection(other);
                        if (collectionChanged)
                            break;
                    }
                }
                transform.hasChanged = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                for (int i = 0; i < connectedNodes.Count; i++)
                {
                    var noden = connectedNodes[i];
                    Gizmos.DrawLine(rendererPivot.position, noden.rendererPivot.position);
                }
            }
        
        }
#endif

        private void OnDestroy()
        {
            this.enabled = false;
            gameObject.SetActive(false);
            Debug.Log("Destroy called");
            foreach (var other in connectedNodes)
            {
                //No need to modify the current list since this object is going to be destroyed and the list is going to be garbage collected
                other.RemoveConnection((this), false);
            }
            connectedNodes.Clear();
        }

        /// <summary>
        /// both nodes needs to have connectableTypes in each others data
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool IsNodeValidConnection(GraphNode other)
        {
            return (this.getConnectableTypes().Contains(other.getType()) && 
                    other.getConnectableTypes().Contains(this.getType()));
        }
    
        /// <summary>
        /// Both nodes needs to be in range of each others
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool IsNodeInRange(GraphNode other)
        {
            var sqrDist = SqrDistance(other);
            return Mathf.Pow(this.getRange(), 2) > sqrDist && 
                   Mathf.Pow(other.getRange(), 2) > sqrDist;
        }

        private float SqrDistance(GraphNode other)
        {
            return Vector3.SqrMagnitude(rendererPivot.position - other.rendererPivot.position);
        }

        /// <summary>
        /// Both nodes needs to have valid capacity
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool DoesNodeHaveConnectionCapacity(GraphNode other)
        {
            return this.getMaxConnectionsCount() > this.connectedNodes.Count &&
                   other.getMaxConnectionsCount() > other.connectedNodes.Count;
        }


        /// <summary>
        /// General check to see if this node can be connected to the other node
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool CanConnectToNode(GraphNode other)
        {
            bool result = true;
            result &= IsNodeValidConnection(other);
            result &= IsNodeInRange(other);
            result &= DoesNodeHaveConnectionCapacity(other);
            //TODO: Validity check
            return result;
        }

        /// <summary>
        /// Tried to connect to closest valid nodes
        /// </summary>
        private void ConnectToClosestNodes()
        {
            var rs = GameObject.FindObjectsOfType<GraphNode>().Where(other => !this.Equals(other) && IsNodeInRange(other));

            if (!rs.Any())
                return;
            
            GameObject.FindObjectsOfType<GraphNode>()
                .Where(other => !this.Equals(other) && IsNodeInRange(other))
                .OrderBy(other => SqrDistance(other))
                .Take(nodeData.connectionCount)
                .ToList()
                .ForEach(other =>
                {
                    if (CanConnectToNode(other))
                    {
                        TryConnectToNode(other);
                    }
                });
        }
    
        /// <summary>
        /// Tries to connect the given node, returns true if connection is succesful
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool TryConnectToNode(GraphNode other)
        {
            if (connectedNodes.Contains(other) || other.Equals(this))
                return false;

            connectedNodes.Add(other);
            other.connectedNodes.Add(this);
        
            if(ConnectionRenderManager.Instance != null)
                ConnectionRenderManager.Instance.AddConnectionRenderer(this, other);
            return true;
        }

        /// <summary>
        /// Removes connection between this node and another node
        /// if optional parameter is given true, also removes this node from the connected node's connections
        /// </summary>
        /// <param name="other"></param>
        /// <param name="alsoRemoveFromOther"></param>
        private void RemoveConnection(GraphNode other, bool alsoRemoveFromOther = true)
        {
            connectedNodes.Remove(other);
            if(alsoRemoveFromOther)
                other.connectedNodes.Remove(this);
        
// #if UNITY_EDITOR // Skips renderManager based action during edit
//         if (!Application.isPlaying) return;
// #endif
        
            if(ConnectionRenderManager.Instance != null)
                ConnectionRenderManager.Instance.RemoveConnectionRenderer(this, other);
        
            //Try to make a new connection 
            ConnectToClosestNodes();
        }

        /// <summary>
        /// Updates the connection between this node and given node, updates renderer positions
        /// Only validates distance by default, optional parameters can be used to check for other criterions
        /// </summary>
        /// <param name="other"></param>
        /// <param name="checkValidConnection"></param>
        /// <param name="checkCapacity"></param>
        /// <returns></returns>
        private bool UpdateConnection(GraphNode other, bool checkValidConnection = false, bool checkCapacity = false)
        {
            bool collectionChanged = false;

            if (IsNodeInRange(other))
            {
                if (!TryConnectToNode(other)
                    && (!checkValidConnection || (IsNodeValidConnection(other)))
                    && (!checkCapacity || (DoesNodeHaveConnectionCapacity(other))))
                {
                    if(ConnectionRenderManager.Instance != null)
                        ConnectionRenderManager.Instance.UpdateConnectionRenderer(this, other);
                    collectionChanged = false;
                }
                else
                {
                    collectionChanged = true;
                }
            }
            else
            {
                RemoveConnection(other);
                collectionChanged = true;
                if(ConnectionRenderManager.Instance != null)
                    ConnectionRenderManager.Instance.RemoveConnectionRenderer(this, other);
            }

            return collectionChanged;
        }


        public void PacketPassThroughCallback(GraphPacket packet)
        {
            if(packetPassCallback != null)
                packetPassCallback.Invoke(packet);
        }
    
        public void PacketEndedOnCallback(GraphPacket packet, bool successful)
        {
            if(packetEndCallback != null)
                packetEndCallback.Invoke(packet, successful);
        }
    
    

    }
}

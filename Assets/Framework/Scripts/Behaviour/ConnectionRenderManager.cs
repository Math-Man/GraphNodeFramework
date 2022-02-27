using System.Collections.Generic;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Framework.Scripts.Behaviour
{
    public class ConnectionRenderManager : MonoBehaviour
    {
        public static ConnectionRenderManager Instance { get; private set; }
        private Transform m_Holder;
        private List<Connection> m_Connections;
        
        [SerializeField] private GameObject renderObjectPrefab;
        
        private void Awake()
        {
            Instance = this;
            m_Holder = new GameObject("Connection Renderer Object Holder").transform;
            m_Holder.parent = this.transform;
            m_Connections = new List<Connection>();
        }


        public void AddConnectionRenderer(GraphNode node1, GraphNode node2)
        {
            LineRenderer objectInstance = GameObject.Instantiate(renderObjectPrefab.gameObject, m_Holder).GetComponent<LineRenderer>();
            objectInstance.SetPosition(0, node1.rendererPivot.position);
            objectInstance.SetPosition(1, node2.rendererPivot.position);
            Connection connectionObject = new Connection(node1, node2, objectInstance);
            m_Connections.Add(connectionObject);
        }


        public void RemoveConnectionRenderer(GraphNode node1, GraphNode node2)
        {
            var conn = FindConnection(node1, node2);
            if (conn != null)
            {
                //Todo: Poll this instead of deleting
                Destroy(conn.renderObject.gameObject);
                m_Connections.Remove(conn);
            }
        }

        public void UpdateConnectionRenderer(GraphNode node1, GraphNode node2)
        {
            var conn = FindConnection(node1, node2);
            if (conn != null)
            {
                conn.renderObject.SetPosition(0, node1.rendererPivot.position);
                conn.renderObject.SetPosition(1, node2.rendererPivot.position);
            }
        }

        private Connection FindConnection(GraphNode node1, GraphNode node2)
        {
            return m_Connections.Find(connection => connection.IsMatching(node1, node2));
        }

        public class Connection
        {
            public GraphNode n1 { get; set; }
            public GraphNode n2 { get; set; }
            public LineRenderer renderObject { get; set; }

            public Connection(GraphNode n1, GraphNode n2, LineRenderer renderer)
            {
                this.n1 = n1;
                this.n2 = n2;
                this.renderObject = renderer;
            }

            public bool IsMatching(GraphNode node1, GraphNode node2)
            {
                return (node1.Equals(n1) && node2.Equals(n2)) || (node1.Equals(n2) && node2.Equals(n1));
            }

        }
    }


}
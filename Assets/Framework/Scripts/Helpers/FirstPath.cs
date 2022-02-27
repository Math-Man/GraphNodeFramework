using System.Collections.Generic;
using Framework.Scripts.Objects;

namespace Framework.Scripts.Helpers
{
    /// <summary>
    /// Simplest possible path-finding implementation, recursive and doesn't care if it finds the shortest path, just finds a path
    /// </summary>
    public class FirstPath : PathingAlgorithm
    {
        
        private List<GraphNode> final;
        
        public void PathFindInner(GraphNode current, GraphNode destination,  HashSet<GraphNode> visited, List<GraphNode> localList, bool found = false)
        {
            if (found)
                return;
            
            if (current.Equals(destination))
            {
                found = true;
                final = new List<GraphNode>(localList);
                return;
            }

            visited.Add(current);
            
            foreach (var node in current.connectedNodes)
            {
                if (!visited.Contains(node) && node.enabled)
                {
                    localList.Add(node);
                    PathFindInner(node, destination, visited, localList, found);
                    localList.RemoveAt(localList.LastIndexOf(node));  //removes the last index
                }
            }

            visited.Remove(current);
        }

        public List<GraphNode> PathFind(GraphNode current, GraphNode destination)
        {
            HashSet<GraphNode> visited = new HashSet<GraphNode>();
            List<GraphNode> localList = new List<GraphNode>();
            localList.Add(current);
            PathFindInner(current, destination, visited, localList);
            return final;
        }
    }
}
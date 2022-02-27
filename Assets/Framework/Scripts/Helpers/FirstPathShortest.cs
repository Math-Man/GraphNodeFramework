using System.Collections.Generic;
using System.Linq;
using Framework.Scripts.Objects;
using UnityEngine;

namespace Framework.Scripts.Helpers
{
    /// <summary>
    /// Simpler path-finding implementation, Similar to "FirstPath" almost always returns the shortest possible path by finding a path close enough to minimum possible length
    /// Can give mixed results if the nodes are intertwining a lot
    /// </summary>
    public class FirstPathShortest : PathingAlgorithm
    {
        private float lowestPossibleScore;
        private List<List<GraphNode>> allPaths;
        private List<GraphNode> final;

        private float tolerance = 25f;
        
        public void PathFindInner(GraphNode current, GraphNode destination, HashSet<GraphNode> visited, List<GraphNode> localList, float totalDistance = 0f)
        {
            if (current.Equals(destination))
            {
                allPaths.Add(new List<GraphNode>(localList));
                return;
            }

            visited.Add(current);
            
            foreach (var node in current.connectedNodes)
            {
                if (!visited.Contains(node) && node.enabled)
                {
                    localList.Add(node);
                    totalDistance += Vector3.Distance(current.rendererPivot.position, node.rendererPivot.position);
                    PathFindInner(node, destination, visited, localList, totalDistance);
                    localList.RemoveAt(localList.LastIndexOf(node)); //removes the last index
                }
            }

            visited.Remove(current);
        }
        
        public List<GraphNode> PathFind(GraphNode current, GraphNode destination, float tolerance)
        {
            this.tolerance = tolerance;
            return PathFind(current, destination);
        }

        public List<GraphNode> PathFind(GraphNode current, GraphNode destination)
        {
            if (destination == null || current == null)
                return null;
            
            lowestPossibleScore = Vector3.Distance(current.rendererPivot.position, destination.rendererPivot.position);
            allPaths = new List<List<GraphNode>>();
            HashSet<GraphNode> visited = new HashSet<GraphNode>();
            List<GraphNode> localList = new List<GraphNode>();
            localList.Add(current);
            PathFindInner(current, destination, visited, localList);

            return allPaths.OrderBy(list => list.Count).FirstOrDefault();

        }
    }
}
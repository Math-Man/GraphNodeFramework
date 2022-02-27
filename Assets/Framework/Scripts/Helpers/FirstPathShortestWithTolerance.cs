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
    public class FirstPathShortestWithTolerance : PathingAlgorithm
    {
        private float lowestPossibleScore;
        private List<List<GraphNode>> allPaths;
        private List<GraphNode> final;

        private float tolerance = 25f;
        
        public void PathFindInner(GraphNode current, GraphNode destination, HashSet<GraphNode> visited, List<GraphNode> localList, bool found = false, float totalDistance = 0f)
        {
            if (found)
                return;
            
            if (current.Equals(destination))
            {
                allPaths.Add(new List<GraphNode>(localList));

                
                if (percentageDifference(lowestPossibleScore, totalDistance) < tolerance)
                {
                    final = new List<GraphNode>(localList);
                    found = true;
                }
                return;
            }

            visited.Add(current);
            
            foreach (var node in current.connectedNodes)
            {
                if (!visited.Contains(node) && node.enabled) 
                {
                    localList.Add(node);
                    totalDistance += Vector3.Distance(current.rendererPivot.position, node.rendererPivot.position);
                    PathFindInner(node, destination, visited, localList, found, totalDistance);
                    localList.RemoveAt(localList.LastIndexOf(node)); //removes the last index
                }
            }

            visited.Remove(current);
        }

        private float percentageDifference(float v1, float v2)
        {
            return (Mathf.Abs(v1 - v2) / ((v1 + v2) / 2)) * 100;
        }
        
        public List<GraphNode> PathFind(GraphNode current, GraphNode destination, float tolerance)
        {
            this.tolerance = tolerance;
            return PathFind(current, destination);
        }

        public List<GraphNode> PathFind(GraphNode current, GraphNode destination)
        {
            lowestPossibleScore = Vector3.Distance(current.rendererPivot.position, destination.rendererPivot.position);
            allPaths = new List<List<GraphNode>>();
            HashSet<GraphNode> visited = new HashSet<GraphNode>();
            List<GraphNode> localList = new List<GraphNode>();
            localList.Add(current);
            PathFindInner(current, destination, visited, localList);

            if (final == null && allPaths.Count > 0)
            {
                //Return the path with lowest amount of connections
                return allPaths.OrderBy(list => list.Count).FirstOrDefault();
            }

            return final;
        }
    }
}
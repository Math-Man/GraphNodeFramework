using System.Collections.Generic;
using Framework.Scripts.Objects;

namespace Framework.Scripts.Helpers
{
    public interface PathingAlgorithm
    {
        public List<GraphNode> PathFind(GraphNode start, GraphNode destination);
    }
}
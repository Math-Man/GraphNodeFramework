using Framework.Scripts.Objects;

namespace Framework.Scripts.Model
{
    public class PacketRequestResultData
    {
        public GraphNode StartNode { get; set; }
        public GraphNode LastNode { get; set; }
        public GraphNode ActualDestination { get; set; }
        public bool Success { get; set; }

        public override string ToString()
        {
            return StartNode + " " + LastNode + " " + ActualDestination + " " + Success;
        }
    }
}
using Framework.Scripts.Objects;
using JetBrains.Annotations;
using UnityEngine;

namespace Sample.Scripts
{
    public class NodePacketListener : MonoBehaviour
    {
        [UsedImplicitly]
        public void PacketPassed(GraphPacket packet)
        {
            Debug.Log(gameObject.name + " packet passed " + packet.gameObject.name);
        }
        
        [UsedImplicitly]
        public void PacketEnded(GraphPacket packet, bool success)
        {
            Debug.Log(gameObject.name + " packet ended " + packet.gameObject.name);
        }
    }
}
using System;

namespace Framework.Scripts.Model
{
    public class PacketRequestData
    {
        public Action<PacketRequestResultData> packetStartCallback;
        public Action<PacketRequestResultData> packetCancelledCallback;
        public Action<PacketRequestResultData> packetReachedCallback;
        
        public void InitCallbacks(Action<PacketRequestResultData> packetStartCallback, Action<PacketRequestResultData> packetCancelledCallback,
            Action<PacketRequestResultData> packetReachedCallback)
        {
            this.packetCancelledCallback = packetCancelledCallback;
            this.packetStartCallback = packetStartCallback;
            this.packetReachedCallback = packetReachedCallback;
        }
    }
}
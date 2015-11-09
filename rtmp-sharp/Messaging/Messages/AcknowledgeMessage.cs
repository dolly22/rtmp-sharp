using RtmpSharp.IO;
using System;

namespace RtmpSharp.Messaging.Messages
{
    [Serializable]
    [SerializedName("flex.messaging.messages.AcknowledgeMessage")]
    public class AcknowledgeMessage : FlexMessage
    {
        public AcknowledgeMessage()
        {
            Timestamp = Environment.TickCount;
        }

        [SerializedName("correlationId")]
        public string CorrelationId { get; set; }
    }
}

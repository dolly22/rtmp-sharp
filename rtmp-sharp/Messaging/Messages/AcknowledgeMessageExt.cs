using RtmpSharp.IO;
using RtmpSharp.IO.AMF3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtmpSharp.Messaging.Messages
{
    [SerializedName("DSK")]
    public class AcknowledgeMessageExt : AcknowledgeMessage, IExternalizable
    {
        [Flags]
        public enum PartFlags
        {
            BODY_FLAG = 0x1,
            CLIENT_ID_FLAG = 0x2,
            DESTINATION_FLAG = 0x4,
            HEADERS_FLAG = 0x8,
            MESSAGE_ID_FLAG = 0x10,
            TIMESTAMP_FLAG = 0x20,
            TIME_TO_LIVE_FLAG = 0x40,
            HAS_NEXT_FLAG = 0x80,
        }

        private ByteArray clientIdBytes;
        private ByteArray messageIdBytes;
        private ByteArray correlationIdBytes;

        public void ReadExternal(IDataInput input)
        {
            // AbstractMessage
            int state;

            PartFlags[] flags = ReadFlags(input);
            for (int i = 0; i < flags.Length; i++)
            {
                state = 0;
                PartFlags flag = flags[i];
                if (i == 0)
                {
                    if ((flag & PartFlags.BODY_FLAG) != 0)
                    {
                        Body = input.ReadObject();
                    }
                    else
                    {
                        Body = null;
                    }
                    if ((flag & PartFlags.CLIENT_ID_FLAG) != 0)
                        ClientId = input.ReadObject() as string;
                    if ((flag & PartFlags.DESTINATION_FLAG) != 0)
                        Destination = input.ReadUtf();
                    if ((flag & PartFlags.HEADERS_FLAG) != 0)
                        Headers = input.ReadObject() as Dictionary<string, object>;
                    if ((flag & PartFlags.MESSAGE_ID_FLAG) != 0)
                        MessageId = input.ReadUtf();
                    if ((flag & PartFlags.TIMESTAMP_FLAG) != 0)
                        Timestamp = (long)(double)input.ReadObject();
                    if ((flag & PartFlags.TIME_TO_LIVE_FLAG) != 0)
                        TimeToLive = (long)input.ReadObject();
                    state = 7;
                }
                else if (i == 1)
                {
                    // clientidbytes
                    if (((int)flag & 0x1) != 0)
                    {
                        clientIdBytes = input.ReadObject() as ByteArray;
                        ClientId = AmfMessageUtils.UIDBytesToString(clientIdBytes.ToArray());
                    }

                    // messageidbytes
                    if (((int)flag & 0x2) != 0)
                    {
                        messageIdBytes = input.ReadObject() as ByteArray;
                        MessageId = AmfMessageUtils.UIDBytesToString(messageIdBytes.ToArray());
                    }
                    state = 2;
                }

                if (((uint)flag >> state) != 0)
                {
                    int local6 = state;
                    while (local6 < 6)
                    {
                        if ((((uint)flag >> local6) & 0x1) != 0)
                        {
                            input.ReadObject();
                        }
                        local6++;
                    }
                }
            }

            ProcessAsyncMessage(input);
            ProcessAcknowledgeMessage(input);
        }

        private void ProcessAcknowledgeMessage(IDataInput input)
        {
            var flags2 = ReadFlags(input);

            for (int idx = 0; idx < flags2.Length; idx++)
            {
                var flag2 = flags2[idx];
                int x = 0;
                if (((uint)flag2 >> x) != 0)
                {
                    int y = x;
                    while (y < 6)
                    {
                        if ((((uint)flag2 >> y) & 0x1) != 0)
                        {
                            input.ReadObject();
                        }
                        y++;
                    }
                }
            }
        }

        private void ProcessAsyncMessage(IDataInput input)
        {
            var flags = ReadFlags(input);

            for (int idx = 0; idx < flags.Length; idx++)
            {
                var flag = (uint)flags[idx];
                int x = 0;

                if (idx == 0)
                {
                    //CORRELATION_ID_FLAG
                    if ((flag & 0x1) != 0)
                    {
                        CorrelationId = input.ReadObject() as string;
                    }
                    // CORRELATION_ID_BYTES_FLAG
                    if ((flag & 0x2) != 0)
                    {
                        correlationIdBytes = input.ReadObject() as ByteArray;
                        CorrelationId = AmfMessageUtils.UIDBytesToString(correlationIdBytes.ToArray());
                    }
                    x = 2;
                }
                if ((flag >> x) != 0)
                {
                    int y = x;
                    while (y < 6)
                    {
                        if (((flag >> y) & 0x1) != 0)
                        {
                            input.ReadObject();
                        }
                        y++;
                    }
                }
            }
        }

        public void WriteExternal(IDataOutput output)
        {
            ProcessWriteAcknowledgeMessage(output);
        }

        private void ProcessWriteAcknowledgeMessage(IDataOutput output)
        {
            ProcessWriteAsyncMessage(output);
            output.WriteByte(0);
        }

        private void ProcessWriteAsyncMessage(IDataOutput output)
        {
            ProcessWriteAbstractMessage(output);

            if (!String.IsNullOrEmpty(CorrelationId) || correlationIdBytes != null)
            {
                // write correlation id bytes
                if (correlationIdBytes == null)
                {
                    byte[] corBytes = AmfMessageUtils.UIDStringToBytes(CorrelationId);
                    correlationIdBytes.WriteBytes(corBytes);
                }

                // CORRELATION_ID_BYTES_FLAG
                output.WriteByte(0x2);
                output.WriteObject(correlationIdBytes);
            }
        }

        private void ProcessWriteAbstractMessage(IDataOutput output)
        {
            byte flag = 0;

            if (Body != null)
                flag |= (int)PartFlags.BODY_FLAG;
            if (!String.IsNullOrEmpty(Destination))
                flag |= (int)PartFlags.DESTINATION_FLAG;
            if (Headers != null && Headers.Any())
                flag |= (int)PartFlags.HEADERS_FLAG;
            if (Timestamp != 0)
                flag |= (int)PartFlags.TIMESTAMP_FLAG;
            if (TimeToLive != 0)
                flag |= (int)PartFlags.TIME_TO_LIVE_FLAG;

            if (ClientId != null || !String.IsNullOrEmpty(MessageId))
                flag |= (int)PartFlags.HAS_NEXT_FLAG;

            output.WriteByte(flag);

            byte flag2 = 0;

            if (ClientId != null || clientIdBytes != null)
            {
                // write correlation id bytes
                if (clientIdBytes == null)
                {
                    byte[] cBytes = AmfMessageUtils.UIDStringToBytes((string)ClientId);
                    clientIdBytes = new ByteArray();
                    clientIdBytes.WriteBytes(cBytes);
                }

                flag2 |= 0x1;
            }
            if (MessageId != null || messageIdBytes != null)
            {
                // write correlation id bytes
                if (messageIdBytes == null)
                {
                    byte[] cBytes = AmfMessageUtils.UIDStringToBytes((string)MessageId);
                    messageIdBytes = new ByteArray();
                    messageIdBytes.WriteBytes(cBytes);
                }

                flag2 |= 0x2;
            }

            if (flag2 != 0)
                output.WriteByte(flag2);

            if (Body != null)
                output.WriteObject(Body);
            if (!String.IsNullOrEmpty(Destination))
                output.WriteObject(Destination);
            if (Headers != null && Headers.Any())
                output.WriteObject(Headers);
            if (Timestamp != 0)
                output.WriteObject(Timestamp);
            if (TimeToLive != 0)
                output.WriteObject(TimeToLive);
            if (clientIdBytes != null)
                output.WriteObject(clientIdBytes);
            if (messageIdBytes != null)
                output.WriteObject(messageIdBytes);
        }

        PartFlags[] ReadFlags(IDataInput input)
        {
            IList<PartFlags> flags = new List<PartFlags>();

            while (true)
            {
                PartFlags flag = (PartFlags)input.ReadByte();
                flags.Add(flag);

                if ((flag & PartFlags.HAS_NEXT_FLAG) == 0)
                    break;
            }

            return flags.ToArray();
        }
    }
}

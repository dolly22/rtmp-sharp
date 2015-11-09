/*
	FluorineFx open source library 
	Copyright (C) 2007 Zoltan Csibi, zoltan@TheSilentGroup.com, FluorineFx.com 
	
	This library is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public
	License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.
	
	This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	Lesser General Public License for more details.
	
	You should have received a copy of the GNU Lesser General Public
	License along with this library; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections;
using System.IO;
using RtmpSharp.IO;
using RtmpSharp.Messaging.Messages;

namespace RtmpSharp.Net
{
	/// <summary>
	/// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
	/// </summary>
	public class AmfPacketSerializer : AmfWriter
	{

		/// <summary>
		/// Initializes a new instance of the AMFSerializer class.
		/// </summary>
		/// <param name="stream"></param>
		public AmfPacketSerializer(Stream stream, SerializationContext serializationContext) 
            : base(stream, serializationContext)
		{
		}

        /// <summary>
        /// This method supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="amfMessage"></param>
        public void WriteMessage(AmfPacket amfMessage)
		{
			try
			{
				base.WriteUInt16(amfMessage.Version);
				int headerCount = amfMessage.HeaderCount;
				base.WriteUInt16((ushort)headerCount);
				for(int i = 0; i < headerCount; i++)
				{
					this.WriteHeader(amfMessage.GetHeaderAt(i), ObjectEncoding.Amf0);
				}
				int bodyCount = amfMessage.BodyCount;
				base.WriteUInt16((ushort)bodyCount);
				for(int i = 0; i < bodyCount; i++)
				{
                    AmfPacketBody amfBody = amfMessage.GetBodyAt(i);
                    if (amfBody == null)
                        throw new InvalidOperationException("amfBody is null");
                    amfBody.WriteBody(amfMessage.ObjectEncoding, this);
				}
			}
			catch(Exception)
			{
				throw;
			}
        }

		private void WriteHeader(AmfPacketHeader header, ObjectEncoding objectEncoding)
		{
			base.Reset();
			base.WriteUtfPrefixed(header.Name);
			base.WriteBoolean(header.MustUnderstand);
			base.WriteInt32(-1);
			base.WriteAmfItem(objectEncoding, header.Content);
		}
 	}
}

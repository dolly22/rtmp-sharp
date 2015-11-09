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
using System.Collections.Generic;
using RtmpSharp.Messaging.Messages;
using RtmpSharp.IO;

namespace RtmpSharp.Net
{ 
    /// <summary>
    /// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
    /// </summary>
    public class AmfPacketDeserializer : AmfReader
	{
        /// <summary>
		/// Initializes a new instance of the AMFDeserializer class.
		/// </summary>
		/// <param name="stream"></param>
		public AmfPacketDeserializer(Stream stream, SerializationContext serializationContext) 
            : base(stream, serializationContext)
		{
		}
        /// <summary>
        /// This method supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <returns></returns>
        public AmfPacket ReadAMFMessage()
		{
			// Version stored in the first two bytes.
			ushort version = base.ReadUInt16();
			AmfPacket message = new AmfPacket(version);
			// Read header count.
			int headerCount = base.ReadUInt16();
			for (int i = 0; i < headerCount; i++)
			{
				message.AddHeader(this.ReadHeader());
			}
			// Read header count.
			int bodyCount = base.ReadUInt16();
			for (int i = 0; i < bodyCount; i++)
			{
                AmfPacketBody amfBody = this.ReadBody();
				if( amfBody != null )//not failed
					message.AddBody(amfBody);
			}
			return message;
		}

		private AmfPacketHeader ReadHeader()
		{
            this.Reset();
			// Read name.
			string name = base.ReadUtf();
			// Read must understand flag.
			bool mustUnderstand = base.ReadBoolean();
			// Read the length of the header.
			int length = base.ReadInt32();
			// Read content.
			object content = base.ReadAmf3Item();
			return new AmfPacketHeader(name, mustUnderstand, content);
		}

		private AmfPacketBody ReadBody()
		{
			this.Reset();
			string target = base.ReadUtf();

			// Response that the client understands.
			string response = base.ReadUtf();
			int length = base.ReadInt32();

			long position = base.Position;
			// Read content.
			try
			{
				object content = base.ReadAmf0Item();
                AmfPacketBody amfBody = new AmfPacketBody(target, response, content);
                return amfBody;
			}
			catch(Exception)
			{
                throw;
			}			
		}
	}
}

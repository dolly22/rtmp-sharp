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
using RtmpSharp.IO;
using System.Collections.Generic;

namespace RtmpSharp.Net
{
	/// <summary>
	/// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
	/// </summary>
    public class AmfPacket
	{
        /// <summary>
        /// AMF packet version.
        /// </summary>
		protected ushort version = 0;

        /// <summary>
        /// AMF packet body values.
        /// </summary>
        protected List<AmfPacketBody> bodies;
        /// <summary>
        /// AMF packet headers.
        /// </summary>
        protected List<AmfPacketHeader> headers;

        /// <summary>
		/// Initializes a new instance of the AMFMessage class.
		/// </summary>
		public AmfPacket() 
            : this(0)
		{
		}
		/// <summary>
		/// Initializes a new instance of the AMFMessage class.
		/// </summary>
		/// <param name="version"></param>
		public AmfPacket(ushort version)
		{
			this.version = version;
            headers = new List<AmfPacketHeader>(1);
            bodies = new List<AmfPacketBody>(1);
		}
        /// <summary>
        /// Gets the AMF packet version.
        /// </summary>
		public ushort Version
		{
			get{ return version; }
		}
        /// <summary>
        /// Adds a body to the AMF packet.
        /// </summary>
        /// <param name="body">The body object to add.</param>
		public void AddBody(AmfPacketBody body)
		{
			this.bodies.Add(body);
		}
        /// <summary>
        /// Adds a header to the AMF packet.
        /// </summary>
        /// <param name="header">The header object to add.</param>
		public void AddHeader(AmfPacketHeader header)
		{
			this.headers.Add(header);
		}
        /// <summary>
        /// Gets the body count.
        /// </summary>
		public int BodyCount
		{
			get{ return bodies.Count; }
		}

        /// <summary>
        /// Gets a readonly collection of AMF bodies.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<AmfPacketBody> Bodies
        {
            get { return bodies.AsReadOnly(); }
        }

		/// <summary>
		/// Gets the header count.
		/// </summary>
		public int HeaderCount
		{
			get{ return headers.Count; }
		}

        /// <summary>
        /// Gets a single AMF body object by index.
        /// </summary>
        /// <param name="index">The numerical index of the body.</param>
        /// <returns>The body referenced by index.</returns>
		public AmfPacketBody GetBodyAt(int index)
		{
			return bodies[index] as AmfPacketBody;
		}

        /// <summary>
        /// Gets a single AMF header object by index.
        /// </summary>
        /// <param name="index">The numerical index of the header.</param>
        /// <returns>The header referenced by index.</returns>
		public AmfPacketHeader GetHeaderAt(int index)
		{
			return headers[index] as AmfPacketHeader;
		}

        /// <summary>
        /// Gets the value of a single AMF header object by name.
        /// </summary>
        /// <param name="header">The name of the header.</param>
        /// <returns>The header referenced by name.</returns>
		public AmfPacketHeader GetHeader(string header)
		{
			for(int i = 0; headers != null && i < headers.Count; i++)
			{
                AmfPacketHeader amfHeader = headers[i] as AmfPacketHeader;
				if( amfHeader.Name == header )
					return amfHeader;
			}
			return null;
		}

        /// <summary>
        /// Removes the named header from teh AMF packet.
        /// </summary>
        /// <param name="header">The name of the header.</param>
        public void RemoveHeader(string header)
        {
            for (int i = 0; headers != null && i < headers.Count; i++)
            {
                AmfPacketHeader amfHeader = headers[i] as AmfPacketHeader;
                if (amfHeader.Name == header)
                {
                    headers.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// Gets the AMF version/encoding used for this AMF packet.
        /// </summary>
		public ObjectEncoding ObjectEncoding
		{
			get
			{
                if (version == 0 || version == 1)
                    return ObjectEncoding.Amf0;
                if (version == 3)
                    return ObjectEncoding.Amf3;
                throw new InvalidOperationException("Unexpected amf format version: "+ version);
            }
		}
	}
}

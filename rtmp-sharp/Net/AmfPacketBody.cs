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
using System.Text;
using System.IO;
using System.Collections.Generic;
using RtmpSharp.Messaging.Messages;
using RtmpSharp.IO;

namespace RtmpSharp.Net
{
	/// <summary>
	/// This type supports the Fluorine infrastructure and is not intended to be used directly from your code.
	/// </summary>
    public class AmfPacketBody
	{
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public const string Recordset = "rs://";
        /// <summary>
        /// Suffix to denote a success.
        /// </summary>
        public const string OnResult = "/onResult";
        /// <summary>
        /// Suffix to denote a failure.
        /// </summary>
        public const string OnStatus = "/onStatus";
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public const string OnDebugEvents = "/onDebugEvents";
        
        /// <summary>
        /// The actual data associated with the operation.
        /// </summary>
        protected object content;
        /// <summary>
        /// Response URI which specifies a unique operation name that will be used to match the response to the client invocation.
        /// </summary>
		protected string response;
        /// <summary>
        /// Target URI describes which operation, function, or method is to be remotely invoked.
        /// </summary>
		protected string target;
		/// <summary>
		/// IgnoreResults is a flag to tell the serializer to ignore the results of the body message.
		/// </summary>
		protected bool ignoreResults;
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected bool isAuthenticationAction;
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected bool isDebug;
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected bool isDescribeService;

		/// <summary>
		/// Initializes a new instance of the AMFBody class.
		/// </summary>
		public AmfPacketBody()
		{
		}

		/// <summary>
		/// Initializes a new instance of the AMFBody class.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="response"></param>
		/// <param name="content"></param>
		public AmfPacketBody(string target, string response, object content)
		{
			this.target = target;
			this.response = response;
			this.content = content;
		}

        /// <summary>
        /// Gets or set the target URI.
        /// The target URI describes which operation, function, or method is to be remotely invoked.
        /// </summary>
		public string Target
		{
			get{ return target; }
			set
			{ 
				target = value;
			}
		}
        /// <summary>
        /// Indicates an empty target.
        /// </summary>
		public bool IsEmptyTarget
		{
			get
			{
				return target == null || target == string.Empty || target == "null";
			}
		}
        /// <summary>
        /// Gets or sets the response URI.
        /// Response URI which specifies a unique operation name that will be used to match the response to the client invocation.
        /// </summary>
		public string Response
		{
			get{ return response; }
			set{ response = value; }
		}
        /// <summary>
        /// Gets or sets the actual data associated with the operation.
        /// </summary>
		public object Content
		{
			get{ return content; }
			set{ content = value; }
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
		public bool IsAuthenticationAction
		{
			get{ return isAuthenticationAction; }
			set{ isAuthenticationAction = value; }
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public bool IgnoreResults
		{
			get{ return ignoreResults; }
			set{ ignoreResults = value; }
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public bool IsDebug
		{
			get{ return isDebug; }
			set{ isDebug = value; }
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public bool IsDescribeService
		{
			get{ return isDescribeService; }
			set{ isDescribeService = value; }
		}

        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public bool IsRecordsetDelivery
		{
			get
			{
				if( target.StartsWith(AmfPacketBody.Recordset) )
					return true;
				else
					return false;
				
			}
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public string GetRecordsetArgs()
		{
			if( target != null )
			{
				if( this.IsRecordsetDelivery )
				{
					string args = target.Substring( AmfPacketBody.Recordset.Length );
					args = args.Substring( 0, args.IndexOf("/") );
					return args;
				}
			}
			return null;
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
		public string TypeName
		{
			get
			{
				if( target != "null" && target != null && target != string.Empty )
				{
					if( target.LastIndexOf('.') != -1 )
					{
						string target = this.target.Substring(0, this.target.LastIndexOf('.'));
						if( this.IsRecordsetDelivery )
						{
							target = target.Substring( AmfPacketBody.Recordset.Length );
							target = target.Substring( target.IndexOf("/") + 1 );
							target = target.Substring(0, target.LastIndexOf('.'));
						}
						return target;
					}
				}
				return null;
			}
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
		public string Method
		{
			get
			{
				if( target != "null" && target != null && target != string.Empty )
				{
					if( target != null && target.LastIndexOf('.') != -1 )
					{
						string target = this.target;
						if( this.IsRecordsetDelivery )
						{
							target = target.Substring( AmfPacketBody.Recordset.Length );
							target = target.Substring( target.IndexOf("/") + 1 );
						}

						if( this.IsRecordsetDelivery )
							target = target.Substring(0, target.LastIndexOf('.'));
						string method = target.Substring(target.LastIndexOf('.')+1);

						return method;
					}
				}
				return null;
			}
		}
        /// <summary>
        /// This member supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public string Call
		{
			get{ return this.TypeName + "." + this.Method; }
		}
        /// <summary>
        /// This method supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public string GetSignature()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( this.Target );
			IList parameterList = GetParameterList();
			for(int i = 0; i < parameterList.Count; i++)
			{
				object parameter = parameterList[i];
				sb.Append( parameter.GetType().FullName );
			}
			return sb.ToString();
		}
        /// <summary>
        /// This method supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public virtual IList GetParameterList()
		{
			IList list = null;
			if( !this.IsEmptyTarget )//Flash RPC parameters
			{
				if(!(content is IList))
				{
                    list = new List<object>();
					list.Add(content );
				}
				else 
					list = content as IList;
			}
			else
			{
				object content = this.Content;
				if( content is IList )
					content = (content as IList)[0];
				FlexMessage message = content as FlexMessage;
				if( message != null )
				{
					//for RemotingMessages only now
					if( message is RemotingMessage )
					{
						list = message.Body as IList;
					}
				}
			}

			if( list == null )
				list = new List<object>();

            return list;
		}

        internal void WriteBody(ObjectEncoding objectEncoding, AmfWriter writer)
        {
            writer.Reset();
            if (this.Target == null)
                writer.WriteUtfPrefixed("null");
            else
                writer.WriteUtfPrefixed(this.Target);

            if (this.Response == null)
                writer.WriteUtfPrefixed("null");
            else
                writer.WriteUtfPrefixed(this.Response);
            writer.WriteInt32(-1);

            WriteBodyData(objectEncoding, writer);
        }
        /// <summary>
        /// This method supports the Fluorine infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected virtual void WriteBodyData(ObjectEncoding objectEncoding, AmfWriter writer)
        {
            object content = this.Content;
            writer.WriteAmfItem(objectEncoding, content);
        }
	}
}

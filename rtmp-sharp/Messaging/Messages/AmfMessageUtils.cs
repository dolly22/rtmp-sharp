using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtmpSharp.Messaging.Messages
{
    internal static class AmfMessageUtils
    {
        public static string UIDBytesToString(byte[] UIDbytes)
        {
            var uid = String.Format("{0}-{1}-{2}-{3}-{4}",
                    BitConverter.ToString(UIDbytes, 0, 4).Replace("-", ""),
                    BitConverter.ToString(UIDbytes, 4, 2).Replace("-", ""),
                    BitConverter.ToString(UIDbytes, 6, 2).Replace("-", ""),
                    BitConverter.ToString(UIDbytes, 8, 2).Replace("-", ""),
                    BitConverter.ToString(UIDbytes, 10, 6).Replace("-", "")
                );

            return uid;
        }

        public static byte[] UIDStringToBytes(string uid)
        {
            if (String.IsNullOrEmpty(uid))
                return null;

            string uidfiltered = uid.Replace("-", "");
            return Enumerable.Range(0, uidfiltered.Length).
                       Where(x => 0 == x % 2).
                       Select(x => Convert.ToByte(uidfiltered.Substring(x, 2), 16)).
                       ToArray();
        }
    }
}

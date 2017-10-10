using System;
using System.Collections.Generic;
using System.Net;

namespace IP2Country.Net
{
    public class IPAddressComparer : IComparer<IPAddress>
    {
        public int Compare(IPAddress x, IPAddress y)
        {
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            var result = x.AddressFamily.CompareTo(y.AddressFamily);
            if (result != 0)
            {
                return result;
            }

            var xBytes = x.GetAddressBytes();
            var yBytes = y.GetAddressBytes();

            var octets = Math.Min(xBytes.Length, yBytes.Length);
            for (var i = 0; i < octets; i++)
            {
                var octetResult = xBytes[i].CompareTo(yBytes[i]);
                if (octetResult != 0)
                {
                    return octetResult;
                }
            }
            return 0;
        }
    }
}
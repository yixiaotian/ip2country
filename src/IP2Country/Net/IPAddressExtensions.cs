using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace IP2Country.Net
{
    public static class IPAddressExtensions
    {
        public static IPAddress GetPreviousIPAddress(this IPAddress ipAddress)
        {
            return ipAddress.Add(-1);
        }

        public static string GetSortKey(this IPAddress ipAddress)
        {
            var arr = new List<string> {ipAddress.AddressFamily.ToString()};
            foreach (var b in ipAddress.GetAddressBytes())
            {
                arr.Add(b.ToString("000"));
            }
            return string.Join(".", arr);
        }

        public static long GetCode(this IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(ipAddress));
            }
            var data = ipAddress.GetAddressBytes();
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public static IPAddress Add(this IPAddress ipAddress, int total)
        {
            var code = ipAddress.GetCode() + total;
            var data = BitConverter.GetBytes(Convert.ToUInt32(code));
            Array.Reverse(data);
            return new IPAddress(data);
        }
    }
}
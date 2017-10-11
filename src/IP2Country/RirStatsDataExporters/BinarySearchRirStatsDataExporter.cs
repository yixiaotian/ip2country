using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IP2Country.Dto;

namespace IP2Country.RirStatsDataExporters
{
    public class BinarySearchRirStatsDataExporter : IRirStatsDataExporter
    {
        public string Extension { get; } = ".dat";

        public string Export(IReadOnlyCollection<RirStatsListDto> items)
        {
            var bodys = new Dictionary<string, BodyInfo>();
            var prefixIndices = new Dictionary<byte, PrefixIndex>();
            var encode = Encoding.UTF8;
            var tmp = Path.GetTempFileName();
            using (var bodyStream = new MemoryStream())
            {
                var index = 0;
                foreach (var item in items)
                {
                    var country = item.Country;
                    if (!bodys.ContainsKey(country))
                    {
                        var data = encode.GetBytes(country);
                        var bodyInfo = new BodyInfo
                        {
                            Offset = (int) bodyStream.Position + 16,
                            Length = (byte) data.Length
                        };
                        bodyStream.Write(data, 0, data.Length);
                        bodys[country] = bodyInfo;
                    }
                    var body = bodys[country];
                    var prefix = item.BeginIPAddress.GetAddressBytes()[0];
                    if (!prefixIndices.ContainsKey(prefix))
                    {
                        prefixIndices[prefix] = new PrefixIndex
                        {
                            Prefix = prefix,
                            StartIndex = index,
                            EndIndex = index
                        };
                    }
                    else
                    {
                        prefixIndices[prefix].EndIndex = index;
                    }
                    index++;
                }

                using (var fs = File.OpenWrite(tmp))
                {
                    var header = new Header();
                    fs.Seek(16, SeekOrigin.Begin);
                    using (var writer = new BinaryWriter(fs, encode))
                    {
                        bodyStream.Seek(0, SeekOrigin.Begin);
                        bodyStream.CopyTo(fs);
                        header.FirstIndexOffset = Convert.ToInt32(fs.Position);
                        foreach (var indexModel in items)
                        {
                            WriteToStream(indexModel.BeginIPAddress, fs);
                            WriteToStream(indexModel.EndIPAddress, fs);
                            var body = bodys[indexModel.Country];
                            body.WriteToStream(fs);
                        }
                        header.LastIndexOffset = Convert.ToInt32(fs.Position) - 12;
                        header.FirstPrefixOffset = Convert.ToInt32(fs.Position);
                        foreach (var prefixIndex in prefixIndices.OrderBy(i => i.Key).Select(i => i.Value))
                        {
                            writer.Write(prefixIndex.Prefix);
                            writer.Write(prefixIndex.StartIndex);
                            writer.Write(prefixIndex.EndIndex);
                        }
                        header.LastPrefixOffset = Convert.ToInt32(fs.Position) - 9;
                        fs.Seek(0, SeekOrigin.Begin);
                        writer.Write(header.FirstIndexOffset);
                        writer.Write(header.LastIndexOffset);
                        writer.Write(header.FirstPrefixOffset);
                        writer.Write(header.LastPrefixOffset);
                    }
                }
            }
            return tmp;
        }

        public static void WriteToStream(IPAddress ipAddress, Stream stream)
        {
            var buffer = ipAddress.GetAddressBytes();
            Array.Reverse(buffer);
            stream.Write(buffer, 0, buffer.Length);
        }

        private class PrefixIndex
        {
            public byte Prefix { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }

        private class BodyInfo
        {
            public int Offset { get; set; }
            public byte Length { get; set; }

            public void WriteToStream(Stream stream)
            {
                var buffer = BitConverter.GetBytes(Offset);
                buffer[3] = Length;
                stream.Write(buffer, 0, buffer.Length);
            }

            public override string ToString()
            {
                return $"[{Offset},{Length}]";
            }
        }

        private class Header
        {
            public int FirstIndexOffset { get; set; }
            public int LastIndexOffset { get; set; }
            public int FirstPrefixOffset { get; set; }
            public int LastPrefixOffset { get; set; }
        }
    }
}
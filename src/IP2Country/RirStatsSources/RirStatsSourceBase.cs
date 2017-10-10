using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Abp.Dependency;
using IP2Country.Dto;
using IP2Country.Extensions;

namespace IP2Country.RirStatsSources
{
    public abstract class RirStatsSourceBase : ISingletonDependency, IRirStatsSource
    {
        private readonly Regex _hashRegex = new Regex("([0-9a-z]{32})");
        public abstract string Name { get; }
        public virtual Guid Id => Url.AsGuid();
        public abstract string Url { get; }

        public bool Update()
        {
            var local = GetPath();
            var localHash = string.Empty;
            var remoteHash = string.Empty;
            if (File.Exists(local))
            {
                localHash = GetHash(local);
            }
            using (var http = new WebClient())
            {
                var str = http.DownloadString($"{Url}.md5");
                if (_hashRegex.IsMatch(str))
                {
                    remoteHash = _hashRegex.Match(str).Groups[1].Value;
                }
                if (!string.IsNullOrWhiteSpace(remoteHash) && remoteHash != localHash)
                {
                    var tmp = Path.GetTempFileName();
                    try
                    {
                        http.DownloadFile(Url, tmp);
                        File.Copy(tmp, local, true);
                        return true;
                    }
                    finally
                    {
                        File.Delete(tmp);
                    }
                }
            }
            return false;
        }

        public IEnumerable<RirStatsListDto> GetRirStats()
        {
            var path = GetPath();
            if (File.Exists(path))
            {
                using (var fs = File.OpenRead(path))
                {
                    using (var sr = new StreamReader(fs, Encoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                            {
                                var arr = line.Split('|');
                                if (arr.Length >= 7)
                                {
                                    var addressFamily = arr[2];
                                    if (addressFamily == "ipv4")
                                    {
                                        var item = new RirStatsListDto
                                        {
                                            Registry = arr[0],
                                            Country = arr[1],
                                            Type = arr[2],
                                            Start = arr[3],
                                            Value = arr[4],
                                            Date = arr[5],
                                            Status = arr[6]
                                        };
                                        yield return item;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual string GetPath()
        {
            return Path.Combine(Id.MapWorkspacePath(), "latest.txt");
        }

        private string GetHash(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    var hash = md5.ComputeHash(fs);
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}
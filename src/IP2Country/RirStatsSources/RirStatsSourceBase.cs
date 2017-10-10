using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using IP2Country.Dto;
using IP2Country.Extensions;
using IP2Country.Net;

namespace IP2Country.RirStatsSources
{
    public abstract class RirStatsSourceBase : IRirStatsSource
    {
        private readonly Regex _hashRegex = new Regex("([0-9a-z]{32})");
        public ILogger Logger { get; set; } = NullLogger.Instance;
        public abstract string Url { get; }
        public abstract string Name { get; }
        public virtual Guid Id => Url.AsGuid();

        public bool TryUpdate(out string version)
        {
            Logger.Debug("TryUpdate");
            var local = GetPath();
            var localHash = string.Empty;
            var remoteHash = string.Empty;
            if (File.Exists(local))
            {
                localHash = GetHash(local);
                Logger.DebugFormat("Local file hash : {0} .", localHash);
            }
            using (var http = new WebClient())
            {
                var str = http.DownloadString($"{Url}.md5");
                if (_hashRegex.IsMatch(str))
                {
                    remoteHash = _hashRegex.Match(str).Groups[1].Value;
                    Logger.DebugFormat("Remote file hash : {0} .", remoteHash);
                }
                if (!string.IsNullOrWhiteSpace(remoteHash) && remoteHash != localHash)
                {
                    var tmp = Path.GetTempFileName();
                    try
                    {
                        Logger.InfoFormat("Begin update to : {0}", remoteHash);
                        http.DownloadFile(Url, tmp);
                        var tmpHash = GetHash(tmp);
                        if (tmpHash == remoteHash)
                        {
                            File.Copy(tmp, local, true);
                            version = remoteHash;
                            Logger.Info("Update success");
                            return true;
                        }
                        else
                        {
                            Logger.WarnFormat("Update error,need hash {0} ,get hash {1} .", remoteHash, tmpHash);
                        }
                    }
                    finally
                    {
                        File.Delete(tmp);
                    }
                }
            }
            version = localHash;
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
                                            Raw = line,
                                            Registry = arr[0],
                                            Country = arr[1],
                                            Type = arr[2],
                                            Start = arr[3],
                                            Value = arr[4],
                                            Date = arr[5],
                                            Status = arr[6],
                                            BeginIPAddress = IPAddress.Parse(arr[3]),
                                            EndIPAddress = IPAddress.Parse(arr[3]).Add(Convert.ToInt32(arr[4]) - 1)
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
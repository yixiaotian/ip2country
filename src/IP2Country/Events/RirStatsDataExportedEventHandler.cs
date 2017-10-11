using System;
using System.IO;
using System.Security.Cryptography;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.IO;

namespace IP2Country.Events
{
    public class RirStatsDataExportedEventHandler : IEventHandler<RirStatsDataExportedEventData>, ITransientDependency
    {
        public void HandleEvent(RirStatsDataExportedEventData eventData)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");
            DirectoryHelper.CreateIfNotExists(dir);
            var path = Path.Combine(dir, $"latest{eventData.Extension}");
            File.Copy(eventData.Path, path, true);
            File.WriteAllText(path + ".md5", GetMd5(eventData.Path));
        }

        private string GetMd5(string path)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var fs = File.OpenRead(path))
                {
                    var data = md5.ComputeHash(fs);
                    return BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}
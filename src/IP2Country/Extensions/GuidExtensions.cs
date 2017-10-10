using System;
using System.IO;
using Abp.IO;

namespace IP2Country.Extensions
{
    public static class GuidExtensions
    {
        public static string MapWorkspacePath(this Guid guid)
        {
            var dir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Workspace",
                guid.ToString("N")
            );
            DirectoryHelper.CreateIfNotExists(dir);
            return dir;
        }
    }
}
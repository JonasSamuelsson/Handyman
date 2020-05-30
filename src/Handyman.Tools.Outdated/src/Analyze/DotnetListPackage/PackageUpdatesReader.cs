using Handyman.Tools.Outdated.Model;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class PackageUpdatesReader : PackageReaderBase
    {
        protected override void Read(string[] strings, Package package)
        {
            if (TryParseVersion(strings[0], out var version, out var info))
            {
                var severity = GetUpdateSeverity(package.CurrentVersion, version);

                package.Updates[severity] = new PackageUpdate
                {
                    Info = info,
                    Version = version
                };
            }
            else
            {
                package.Info = info;
            }
        }

        private static bool TryParseVersion(string s, out string version, out string info)
        {
            var match = Regex.Match(s, @"^(?<version>\d+\.\d+\.\d+\S*)( (?<info>.+))?$");

            if (match.Success == false)
            {
                version = string.Empty;
                info = s;
                return false;
            }

            version = match.Groups["version"].Value;
            info = match.Groups["info"].Value;

            if (info == "(D)")
            {
                info = "deprecated";
            }

            return true;
        }

        private static UpdateSeverity GetUpdateSeverity(string currentVersion, string updateVersion)
        {
            var x = currentVersion.Split('.', 3);
            var y = updateVersion.Split('.', 3);

            if (x[0] != y[0])
                return UpdateSeverity.Major;

            if (x[1] != y[1])
                return UpdateSeverity.Minor;

            return UpdateSeverity.Patch;
        }
    }
}
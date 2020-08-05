using Handyman.Tools.Outdated.Model;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class PackageUpdatesReader : PackageReaderBase
    {
        protected override void Read(string[] strings, Package package)
        {
            if (TryParseVersion(strings[0], out var version, out var info, out var deprecated))
            {
                var severity = GetUpdateSeverity(package.CurrentVersion, version);

                if (deprecated)
                {
                    info = string.IsNullOrEmpty(info)
                        ? "Deprecated"
                        : $"Deprecated, {info}";
                }

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
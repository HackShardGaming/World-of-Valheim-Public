using System;
using System.Globalization;
using System.Reflection;

namespace ValheimPermissions
{
    public static class ModInfo
    {
        public const string Name = "ValheimPermissions";
        public const string Title = "Valheim - Permissions";
        public const string Group = "HackShardGaming";
        public const string Guid = "HackShardGaming.ValheimPermissions";
        // Version follow Semantic Versioning Scheme (https://semver.org/)
        public const string Version = "1.1.2";
        public const string buildDate = "2021-09-23";

        // Nexus Plugin ID (Use to maintain updates with Nexus)
        public const int NexusID = 1050;

        // Use GetBuildDate(Assembly.GetExecutingAssembly()); to get build date
        private static DateTime m_GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }
            return default;
        }

        public static String GetBuildDate()
        {
            return ModInfo.buildDate;
            //DateTime buildDate = m_GetBuildDate(Assembly.GetExecutingAssembly());
            //return buildDate.ToString("yyyy-MM-dd");
        }
    }
}

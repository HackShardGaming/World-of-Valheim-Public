using System;
using System.Reflection;
using System.Globalization;

namespace WorldofValheimServerSideCharacters
{
    public static class ModInfo
    {
        public const string Name = "ServerSideCharacters";
        public const string Title = "ServerSideCharacters";
        public const string Group = "FixItFelix";
        public const string Guid = "FixItFelix.ServerSideCharacters";
        // Version follow Semantic Versioning Scheme (https://semver.org/)
        public const string Version = "0.0.1";
        public const string buildDate = "2021-06-12";
        // TODO new id
        public const int NexusID = 892;
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

using System;
using System.Reflection;
using System.Globalization;

namespace ValheimOnline
{
    public static class ModInfo
    {
        public const string Name = "ValheimOnline";
        public const string Guid = "ValheimOnline";
        public const string Version = "0.1.0.0";


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
            DateTime buildDate = m_GetBuildDate(Assembly.GetExecutingAssembly());
            return buildDate.ToString("yyyy-MM-dd");
        }
    }
}

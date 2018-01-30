using System.Reflection;

namespace Cyanide
{
    public static class AppHelper
    {
        public static string Version { get; } =
            typeof(AppHelper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
            typeof(AppHelper).GetTypeInfo().Assembly.GetName().Version.ToString(3) ??
            "Unknown";
    }
}
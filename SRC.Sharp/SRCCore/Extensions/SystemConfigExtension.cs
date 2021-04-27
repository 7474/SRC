using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Extensions
{
    public static class SystemConfigExtension
    {
        public static bool GetFlag(this ISystemConfig config, string section, string name)
        {
            return config.GetItem(section, name)?.ToLower() == "on";
        }

        public static void SetFlag(this ISystemConfig config, string section, string name, bool value)
        {
            config.SetItem(section, name, value ? "On" : "Off");
        }
        public static void ToggleFlag(this ISystemConfig config, string section, string name)
        {
            config.SetFlag(section, name, !config.GetFlag(section, name));
        }

    }
}

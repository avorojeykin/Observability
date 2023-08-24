using System;
using System.Collections.Generic;
using System.Text;

namespace Observability.LibraryBase.Logging.Logging_Objects
{
    public static class UserConfiguration
    {
        public static string AppName { get; set; }
        public static object AppVersion { get; set; }
        public static string HostName { get; set; }

        public static List<KeyValuePair<string, object>> ConvertToKeyValuePairs()
        {
            List<KeyValuePair<string, object>> context = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("AppName", AppName ?? string.Empty),
                new KeyValuePair<string, object>("AppVersion", AppVersion ?? string.Empty),
                new KeyValuePair<string, object>("HostName", HostName ?? string.Empty)
            };
            return context;
        }
    }
}

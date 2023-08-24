using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.LibraryBase.SettingOptions
{
    public class ObservabilityGenericOptions
    {
        public const string CONFIG_SECTION_NAME = "ObservabilityGenericOptions";
        public string CloudRoleName { get; set; } = "";
        public bool ConsoleOuputEnabled { get; set; } = false;
        public string DefaultConsoleOutputLogLevel { get; set; } = "Information";
    }
}

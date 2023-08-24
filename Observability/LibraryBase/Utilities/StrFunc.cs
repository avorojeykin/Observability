using System.Text.Json;
using System.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Observability.Utilities
{
    public  static  class StrFunc
    {

        public static string CleanMessage(string message)
        {
            //Veracode:  Improper Output Neutralization for Logs (CWE ID 117)(bg.next.log.dll)
            return HttpUtility.HtmlEncode(message.Replace("\r", string.Empty).Replace("\n", string.Empty));
        }

        public static bool IsValidJson(string stringToCheck)
        {
            if (string.IsNullOrEmpty(stringToCheck))
                return false;
            try
            {
                JsonDocument.Parse(stringToCheck);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        internal static string GetExceptionString(ref Exception oE)
        {
            System.Text.StringBuilder oSB = new System.Text.StringBuilder(300);
            oSB.Append("Error type of System.Exception occured:\r\n");
            oSB.Append("InnerException:\t" + oE.InnerException + "\r\n");
            oSB.Append("Message:\t" + oE.Message + "\r\n");
            oSB.Append("Source:\t" + oE.Source + "\r\n");
            oSB.Append("TargetSite:\t" + oE.TargetSite + "\r\n");
            oSB.Append("Stack:\t" + oE.StackTrace + "\r\n");
            return oSB.ToString();
        }
    }
}

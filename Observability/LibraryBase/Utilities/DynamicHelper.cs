using Newtonsoft.Json.Linq;

namespace Observability.LibraryBase.Utilities
{
    public static class DynamicHelper
    {
        /// <summary>
        /// Check to see if item is in cache
        /// </summary>
        /// <param name="obj">the dynamic object</param>
        /// <param name="name">the property name</param>
        /// <returns>true if property exists</returns>
        public static bool IsPropertyExist(dynamic obj, string name) => (obj is JObject @object) ? @object.ContainsKey(name) : obj.GetType().GetProperty(name) != null;
    }
}

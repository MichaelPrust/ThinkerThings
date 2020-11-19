using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThinkerThingsHost.AlexaServices
{
    internal class PortInfo
    {
        public bool Ideal { get; set; }
        public string TargetPort { get; set; }
        public string Query { get; set; }

        public static PortInfo Locate(string[] ports, string query)
        {
            query = RemoveSingleChars(query);
            var result = new PortInfo()
            {
                Query = query
            };

            var idealTargetPort = ports.FirstOrDefault(p => string.Equals(p, query, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(idealTargetPort))
            {
                var itemsForRegex = SplitBySpaces(query);
                var regexPattern = $@".*{string.Join(@".*", itemsForRegex)}.*";
                var regex = new Regex(regexPattern);
                result.TargetPort = ports.FirstOrDefault(regex.IsMatch);
            }
            else
            {
                result.Ideal = true;
                result.TargetPort = idealTargetPort;
            }

            return result;
        }

        private static string RemoveSingleChars(string query)
        {
            var items = SplitBySpaces(query);
            var validItems = items.Where(p => p.Length > 1);

            return string.Join(" ", validItems);
        }

        private static string[] SplitBySpaces(string query)
        {
            return Regex.Split(query, @"\s", RegexOptions.Compiled);
        }

    }

}
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Frederikskaj2.EmailAgent.Functions.Extensions
{
    internal static class StringExtensions
    {
        private static readonly Regex markdownRegex = new Regex(@"[\\`\*_\{\}\[\]\(\)#\+\-\.!]");

        public static string EscapeMarkdown(this string @string) => markdownRegex.Replace(@string, match => $"&#{(int) match.Captures[0].Value[0]};");

        public static string EscapeJsonString(this string @string) => JsonConvert.ToString(@string)[1..^1];
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AM_i18n.Scripts.Core
{
    public class LanguageEntryDataIOUtility
    {
        public static string[] LoadLanguagesOptions(string contentFolder)
        {
            List<string> result = new List<string>();
            string languangeEntryPattern = "[a-z]{2}" + Regex.Escape("_") + "[A-Z]{2}" + Regex.Escape(".") + "json";
            Regex regex = new Regex(languangeEntryPattern);
            IEnumerable<string> languages = Directory.EnumerateFiles(contentFolder, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string language in languages)
            {
                Match match = regex.Match(language);
                if (match.Success)
                {
                    result.Add(match.Value.Replace(".json", string.Empty));
                }
            }

            return result.ToArray();
        }
    }
}

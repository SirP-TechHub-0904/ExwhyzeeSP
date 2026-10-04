using System;
using System.Text.RegularExpressions;

namespace ExwhyzeeSP.Core.Helpers
{
    public static class UrlSlugHelper
    {
        public static string ToSlug(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Lowercase and trim
            string str = text.ToLowerInvariant().Trim();

            // Replace accented characters/symbols
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            // Replace multiple spaces/hyphens with a single hyphen
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();

            // Truncate to reasonable max length (e.g. 75 characters) to keep URLs clean
            if (str.Length > 75)
            {
                str = str.Substring(0, 75).Trim();
            }

            // Replace spaces with hyphens
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }
    }
}

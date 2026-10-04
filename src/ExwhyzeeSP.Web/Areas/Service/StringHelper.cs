using System;
using System.Collections.Generic;
using System.Linq;
// // using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ExwhyzeeSP.Web.Areas.Service
{
    public static class StringHelper
    {
        public static string ToSeoUrl(this string url)
        {
            string encodedUrl = (url ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }
    }
}


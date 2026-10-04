using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace ExwhyzeeSP.Services
{
    public class MainServices
    {
        public static string SelectColor()
        {
            var list = new List<string> {
                "bg-xyz-1","bg-xyz-2","bg-xyz-3","bg-xyz-4","bg-xyz-5","bg-xyz-6","bg-xyz-7","bg-xyz-8","bg-xyz-9","bg-xyz-10",
                "bg-xyz-11","bg-xyz-12","bg-xyz-13","bg-xyz-14","bg-xyz-15","bg-xyz-16","bg-xyz-17","bg-xyz-18","bg-xyz-19","bg-xyz-20"
            };
            var random = new Random();
            return list[random.Next(list.Count)];
        }

        public static bool VerifyPortfolio(string data)
        {
            return true;
        }

        public string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    return networkInterface.GetPhysicalAddress().ToString();
                }
            }
            return string.Empty;
        }

        public static string RemoveHtmlAndCss(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string noHtmlTags = Regex.Replace(input, @"<[^>]+>|&nbsp;", string.Empty);
                string noCssProperties = Regex.Replace(noHtmlTags, @"\sstyle\s*=\s*""[^""]*""", string.Empty);
                return noCssProperties;
            }
            return "";
        }

        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "";

            string slug = title.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9]+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');
            return slug;
        }
    }
}

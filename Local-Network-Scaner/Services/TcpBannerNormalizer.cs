using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Local_Network_Scanner.Services
{
    public static class TcpBannerNormalizer
    {
        public static string NormalizeBanner(string rawBanner)
        {
            if (string.IsNullOrEmpty(rawBanner))
                return string.Empty;

            // Remove non-printable characters
            StringBuilder cleaned = new StringBuilder();
            foreach (char c in rawBanner)
            {
                if (!char.IsControl(c) || c == '\n')
                {
                    cleaned.Append(c);
                }
            }

            string result = cleaned.ToString();

            // Trim excessive whitespace
            result = Regex.Replace(result, @"\s+", " ");

            // Limit length for the UI
            const int maxLength = 120;
            if (result.Length > maxLength)
            {
                result = result.Substring(0, maxLength) + "...";
            }

            return result;
        }
    }
}

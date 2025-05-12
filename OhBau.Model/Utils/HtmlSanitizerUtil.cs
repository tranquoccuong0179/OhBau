using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganss.Xss;

namespace OhBau.Model.Utils
{
    public class HtmlSanitizerUtil
    {
        public string Sanitize(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                return string.Empty;

            var sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("img");

            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedAttributes.Add("rel");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedAttributes.Add("style");
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowedAttributes.Add("alt");

            return sanitizer.Sanitize(htmlContent);
        }
    }
}

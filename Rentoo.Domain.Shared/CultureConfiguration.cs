using System;

namespace Rentoo.Domain.Shared
{
    public class CultureConfiguration
    {
        public static readonly string[] AvailableCultures = { "ar-SA", "en-US" };
        public static readonly string DefaultRequestCulture = "ar-SA";

        public List<string> Cultures { get; set; }
        public string DefaultCulture { get; set; } = DefaultRequestCulture;

        public static bool IsArabic
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");
            }
        }
    }
}

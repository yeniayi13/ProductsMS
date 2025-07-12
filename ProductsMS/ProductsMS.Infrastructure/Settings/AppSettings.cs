using System.Diagnostics.CodeAnalysis;

namespace ProductsMs.Infrastructure.Settings
{
    public class AppSettings
    {
        [ExcludeFromCodeCoverage]

        public string? key1 { get; set; }
    }
}

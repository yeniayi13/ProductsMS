
using System.Text.RegularExpressions;

namespace ProductsMs.Domain.Entities.Products.ValueObjects
{
    public partial class ProductImage
    {
        private const string Pattern = @"^(http(s)?://)?([a-zA-Z0-9_-]+\.)+[a-zA-Z]+(/.*\.(jpg|jpeg|png|bmp|gif))$";

        private ProductImage(string url) => Url = url;

        public static ProductImage Create(string url)
        {
            try
            {
                //if (string.IsNullOrEmpty(url)) throw new NullAttributeException("Image URL is required");
               // if (!UrlRegex().IsMatch(url)) throw new InvalidAttributeException("Invalid image URL format");

                return new ProductImage(url);
            }
            catch (Exception e)
            {
                throw; // Aquí podrías agregar lógica de manejo de excepciones como logging.
            }
        }

        public string Url { get; init; } // Inmutable, solo se asigna desde el constructor

        [GeneratedRegex(Pattern)]
        private static partial Regex UrlRegex();
    }
}

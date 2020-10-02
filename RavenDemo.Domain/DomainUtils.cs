using Faker;
using System.Collections.Generic;

namespace RavenDemo.Domain
{
    public static class DomainUtils
    {
        public static IList<Product> BuildSampleProducts(int nbOfProducts)
        {
            var products = new List<Product>();

            for (int id = 0; id < nbOfProducts; id++)
            {
                var p = BuildAProduct(id + 1);
                UpdateProductTags(p);
                products.Add(p);
            }

            return products;
        }

        public static Product BuildAProduct(int id)
        {
            return new Product()
            {
                Id = id.ToString(),
                Name = Lorem.Sentence(3),
                Intro = Lorem.Paragraph(2),
                Price = RandomNumber.Next(10, 1000)
            };
        }

        public static void UpdateProductTags(Product product)
        {
            var tags = new List<string>();
            var number = RandomNumber.Next(0, 5);

            if (number >= 1)
            {
                tags.Add("Sport");
            }
            if (number >= 2)
            {
                tags.Add("Investment");
            }
            if (number >= 3)
            {
                tags.Add("Banking");
            }
            if (number >= 4)
            {
                tags.Add("Real Estate");
            }

            product.Tags = tags;
        }
    }
}

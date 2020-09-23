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
                products.Add(new Product()
                {
                    Id = id + 1,
                    Name = Lorem.Sentence(3),
                    Intro = Lorem.Paragraph(2),
                    Price = RandomNumber.Next(10, 1000)
                });
            }

            return products;
        }
    }
}

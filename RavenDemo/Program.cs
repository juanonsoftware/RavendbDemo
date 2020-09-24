using Raven.Client;
using Raven.Client.Document;
using RavenDemo.Domain;
using System;
using System.Configuration;
using System.Linq;

namespace RavenDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var selection = 0;
            do
            {
                int.TryParse(ShowMenu(), out selection);

                if (selection == 1)
                {
                    InitializeSampleData();
                }

                if (selection == 2)
                {
                    SearchProducts();
                }

            } while (selection >= 0);
        }

        private static string ShowMenu()
        {
            Console.WriteLine(
                "---" + Environment.NewLine +
                "Select your option:" + Environment.NewLine +
                "1: Initialize sample data" + Environment.NewLine +
                "2: Search for products" + Environment.NewLine +
                "-1: Exit"
            );

            return Console.ReadLine();
        }

        private static void InitializeSampleData()
        {
            var products = DomainUtils.BuildSampleProducts(150);

            using (var store = CreateStore())
            {
                using (var session = store.OpenSession())
                {
                    foreach (var product in products)
                    {
                        session.Store(product);
                    }

                    session.SaveChanges();
                }
            }

            Console.WriteLine("Documents saved");
        }

        private static void SearchProducts()
        {
            Console.WriteLine("Enter your text to search");
            var text = Console.ReadLine().Trim();

            var products = Enumerable.Empty<Product>();

            using (var store = CreateStore())
            {
                using (var session = store.OpenSession())
                {
                    var productsQuery = session.Query<Product>()
                        .Customize(c => c.BeforeQueryExecution(a => a.PageSize = 20));

                    if (string.IsNullOrEmpty(text))
                    {
                        products = productsQuery.ToList();
                    }
                    else
                    {
                        // this doesn't work for searching substring
                        //products = productsQuery.Where(p => p.Name.Contains(text)).ToList();

                        // Search by AND option
                        //products = productsQuery
                        //    .Search(p => p.Name, text, escapeQueryOptions: EscapeQueryOptions.RawQuery)
                        //    .ToList();

                        // Adding boost on Name field to prefer that than Intro field
                        products = productsQuery
                            .Search(p => p.Name, text, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards, boost: 10)
                            .Search(p => p.Intro, text, options: SearchOptions.Or, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
                            .ToList();
                    }
                }
            }

            Console.WriteLine("Results of the search: {0} items", products.Count());
            foreach (var p in products)
            {
                Console.WriteLine("Id: {0} \tName: {1}", p.Id, p.Name);
            }
        }

        private static IDocumentStore CreateStore()
        {
            return new DocumentStore()
            {
                Url = ConfigurationManager.AppSettings["RavenDbUrl"],
                ApiKey = ConfigurationManager.AppSettings["RavenDbApiKey"],
                DefaultDatabase = ConfigurationManager.AppSettings["RavenDbDefaultDatabase"]
            }.Initialize();
        }
    }
}

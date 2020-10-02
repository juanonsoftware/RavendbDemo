using Raven.Abstractions.Data;
using Raven.Client;
using RavenDemo.Domain;
using System;
using System.Linq;

namespace RavenDemo
{
    public class Utils
    {
        public static void StartDemo(IDocumentStore store)
        {
            var selection = 0;

            do
            {
                int.TryParse(Common.ShowMenu(), out selection);

                if (selection == 1)
                {
                    InitializeSampleData(store);
                }

                if (selection == 2)
                {
                    SearchProducts(store);
                }

                if (selection == 3)
                {
                    GetAProduct(store);
                }

                if (selection == 4)
                {
                    UpdateAProduct(store);
                }

                if (selection == 5)
                {
                    DeleteAProduct(store);
                }

                if (selection == 6)
                {
                    DeleteAllProducts(store);
                }

            } while (selection >= 0);
        }

        public static void InitializeSampleData(IDocumentStore documentStore)
        {
            var products = DomainUtils.BuildSampleProducts(150);

            using (var session = documentStore.OpenSession())
            {
                foreach (var product in products)
                {
                    session.Store(product);
                }

                session.SaveChanges();
            }

            Console.WriteLine("Documents saved");
        }



        public static void SearchProducts(IDocumentStore documentStore)
        {
            Console.WriteLine("---" + Environment.NewLine + "Enter your text to search");
            var text = Console.ReadLine().Trim();

            var products = Enumerable.Empty<Product>();

            using (var session = documentStore.OpenSession())
            {
                var productsQuery = session.Query<Product>()
                    .Customize(c =>
                    {
                        c.BeforeQueryExecution(a =>
                        {
                            a.PageSize = 20;
                        });
                    });

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

            Console.WriteLine("Results of the search: {0} items", products.Count());
            foreach (var p in products)
            {
                Console.WriteLine("Id: {0} \tName: {1}", p.Id, p.Name);
            }
        }

        public static void DeleteAProduct(IDocumentStore documentStore)
        {
            Console.WriteLine("---" + Environment.NewLine + "Enter a product id");
            var productId = Console.ReadLine().Trim();

            using (var session = documentStore.OpenSession())
            {
                session.Delete(productId);
                session.SaveChanges();
            }
        }

        public static void DeleteAllProducts(IDocumentStore documentStore)
        {
            var ops = documentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName",
                new IndexQuery()
                {
                    Query = "Tag:Products"
                },
                new BulkOperationOptions()
                {
                    RetrieveDetails = true,
                    AllowStale = false
                });
            ops.OnProgressChanged = o =>
            {
                Console.WriteLine("TotalEntries: {0}, ProcessedEntries: {1}", o.TotalEntries, o.ProcessedEntries);
            };
            ops.WaitForCompletion();
        }

        public static void UpdateAProduct(IDocumentStore documentStore)
        {
            Console.WriteLine("---" + Environment.NewLine + "Enter a product id");
            var productId = Console.ReadLine().Trim();

            using (var session = documentStore.OpenSession())
            {
                var product = session.Load<Product>(productId);
                if (product != null)
                {
                    DomainUtils.UpdateProductTags(product);

                    // This method will be used to update the product
                    session.Store(product);

                    session.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Id does not match with any product");
                }
            }
        }

        public static void GetAProduct(IDocumentStore documentStore)
        {
            Console.WriteLine("---" + Environment.NewLine + "Enter a product id");
            var productId = Console.ReadLine().Trim();

            using (var session = documentStore.OpenSession())
            {
                var product = session.Load<Product>(productId);
                if (product != null)
                {
                    var metadata = session.Advanced.GetMetadataFor(product);

                    Console.WriteLine("------------");
                    Console.WriteLine("Id: {0}, Name: {1}", product.Id, product.Name);
                    Console.WriteLine("Last-Modified: {0}", metadata["Last-Modified"]);
                }
                else
                {
                    Console.WriteLine("Id does not match with any product");
                }
            }
        }
    }
}

using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using RavenDemo.Domain;
using System;
using System.Linq;

namespace RavenDemoV4
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
                        c.BeforeQueryExecuted(a =>
                        {
                            //a.PageSize = 20;
                        });
                    });

                if (string.IsNullOrEmpty(text))
                {
                    // NEW: Setting pagesize now is via LINQ commands
                    products = productsQuery.Take(30).ToList();
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
                        .Search(p => p.Name, text, boost: 10)
                        .Search(p => p.Intro, text, options: SearchOptions.Or)
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

            using (var session = documentStore.OpenSession(new SessionOptions()
            {
                TransactionMode = TransactionMode.ClusterWide
            }))
            {
                session.Delete(productId);
                session.SaveChanges();
            }
        }

        public static void DeleteAllProducts(IDocumentStore documentStore)
        {
            // NEW: use Send an operation to the server
            var ops = documentStore.Operations.Send(
                    new DeleteByQueryOperation(
                       new IndexQuery()
                       {
                           Query = "from Products"
                       },
                       new QueryOperationOptions()
                       {
                           AllowStale = false,
                           RetrieveDetails = true,
                           StaleTimeout = TimeSpan.FromMinutes(1)
                       }
                ));

            var result = ops.WaitForCompletion<BulkOperationResult>();

            Console.WriteLine(result.Message);
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

                    // NEW: This method will be used to update the product
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
                    // NEW: the metadata now use lower-case for last modified
                    Console.WriteLine("Last-Modified: {0}", metadata["@last-modified"]);
                }
                else
                {
                    Console.WriteLine("Id does not match with any product");
                }
            }
        }
    }
}

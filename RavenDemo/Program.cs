using Raven.Client;
using Raven.Client.Document;
using RavenDemo.Domain;
using System;
using System.Configuration;

namespace RavenDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int selection = 0;
            do
            {
                selection = ShowMenu();
                if (selection == 1)
                {
                    InitializeSampleData();
                }



            } while (selection > 0);
        }

        private static int ShowMenu()
        {
            Console.WriteLine(
                "---" + Environment.NewLine +
                "Select your option:" + Environment.NewLine +
                "1: Initialize sample data" + Environment.NewLine +
                "2: " + Environment.NewLine +
                "0: Exit"
            );

            return int.Parse(Console.ReadLine().Trim());
        }

        private static void InitializeSampleData()
        {
            var products = DomainUtils.BuildSampleProducts(100);

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

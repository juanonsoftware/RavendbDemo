using Raven.Client;
using Raven.Client.Document;
using System.Configuration;

namespace RavenDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var store = CreateDocumentStore())
            {
                Utils.StartDemo(store);
            }
        }

        private static IDocumentStore CreateDocumentStore()
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

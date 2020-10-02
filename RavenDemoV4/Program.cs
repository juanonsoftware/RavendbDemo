using Raven.Client.Documents;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace RavenDemoV4
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
            var cert = new X509Certificate2(ConfigurationManager.AppSettings["RavenDbCertificateFile"], ConfigurationManager.AppSettings["RavenDbCertificatePassword"]);

            return new DocumentStore()
            {
                Urls = ConfigurationManager.AppSettings["RavenDbUrls"].Split(","),
                Database = ConfigurationManager.AppSettings["RavenDbDefaultDatabase"],
                Certificate = cert,
            }.Initialize();
        }
    }
}

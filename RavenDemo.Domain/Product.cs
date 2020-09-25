using System.Collections.Generic;
using System.Linq;

namespace RavenDemo.Domain
{
    public class Product
    {
        public int Id { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Intro { get; set; } = string.Empty;

        public int Price { get; set; }

        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}

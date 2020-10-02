using System;
using System.Collections.Generic;
using System.Text;

namespace RavenDemo.Domain
{
    public class Common
    {
        public static string ShowMenu()
        {
            Console.WriteLine(
                "---" + Environment.NewLine +
                "Select your option:" + Environment.NewLine +
                "1: Initialize sample data" + Environment.NewLine +
                "2: Search for products" + Environment.NewLine +
                "3: Get a product" + Environment.NewLine +
                "4: Update a product" + Environment.NewLine +
                "5: Delete a product" + Environment.NewLine +
                "6: Delete ALL products" + Environment.NewLine +
                "-1: Exit"
            );

            return Console.ReadLine();
        }
    }
}

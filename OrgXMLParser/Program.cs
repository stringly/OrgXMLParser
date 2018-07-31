using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgXMLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            Console.WriteLine("Attempting to parse " + path);
            if (File.Exists(path))
            {
                Console.WriteLine("File found!");

            }
            else
            {
                Console.WriteLine("File could not be found. Confirm the filepath and try again.");                
            }

        }
    }
}

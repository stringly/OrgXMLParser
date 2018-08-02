using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OrgXMLParser
{
    class Program
    {
        static void Main(string[] args)
        {

        Console.WriteLine(@"Attempting to parse Query from F:\Projects\BlueDeck\OrgChart1.accdb" );
            if (File.Exists(@"F:\Projects\BlueDeck\OrgChart1.accdb"))
            {
                Console.WriteLine("File found!");
                // Obv, change this to any function that can generate a "flat" List<Component>
                ProcessAccessData();
                bool exit = false;
                do
                {
                    Console.WriteLine("Valid Commands:\n\t[unitname] will attempt to parse a unit tree with the name provided" +
                        "\n\t[exit] will exit the program");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "exit":
                            exit = true;
                            break;

                        default:
                            Console.WriteLine("Command not recognized.\n");
                            break;

                    }
                } while (!exit);
                
            }
            else
            {
                Console.WriteLine("File could not be found. Confirm the data source exists at the filepath and try again.");                
            }

        }
        // This creates a List<Component> flat list of components by connecting to Access
        static void ProcessAccessData()
        {
            OleDbConnection conn = new
                OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                @"Data source= F:\Projects\BlueDeck\OrgChart1.accdb";
            try
            {
                List<Component> rawComponentList = new List<Component>();
                conn.Open();
                OleDbDataReader reader = null;
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM District_I_Map", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    Component n = new Component();
                    n.ComponentID = Convert.ToInt32(reader[0]);
                    n.ComponentName = reader[2].ToString();
                    n.ParentComponentID = Convert.ToInt32(reader[1]);
                    n.ParentComponentName = reader[3].ToString();
                    Console.WriteLine(string.Format($"\nAdding {n.ComponentName} to Component List..."));
                    rawComponentList.Add(n);
                }
                // All Components should be added to the collection at this point
                // Call the function to parse and nest components from the flat list
                NestFlatComponentList(rawComponentList);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to data source");
            }
            finally
            {
                conn.Close();
            }
        }

        static void NestFlatComponentList(List<Component> ls)
        {
            int i = 0;
            foreach (Component current in ls)
            {
                i++;
                Console.WriteLine($"Processing component number {i}, current master list count is: " + ls.Count);


                // Gather My Children
                //List<Component> childList = new List<Component>();
                foreach (Component child in ls)
                {
                    if (child.ParentComponentID == current.ComponentID)
                    {
                        //childList.Add(child);
                        current.DirectChildren.Add(child);
                    }
                }
            }
            DataBreaker brkr = new DataBreaker(ls);
            brkr.BreakThisShit("District I");
        }
    }

}

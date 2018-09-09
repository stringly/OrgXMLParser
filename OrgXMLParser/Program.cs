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
        private static List<Component> Components;

        static void Main(string[] args)
        {
            string input = "init";
            
            while (input != "exit") {
            Console.WriteLine("Please provide the filepath of the Org Chart Access DB." +
            "\n Press [enter] to use the default at: " + @"'E:\Projects\BlueDeck\OrgChart1.accdb'" +
            "\n Type 'exit' to exit the program.");
                input = Console.ReadLine();
                try {
                    switch (input) {
                        case "":
                            if (!File.Exists(@"E:\Projects\BlueDeck\OrgChart1.accdb")) { throw new FileNotFoundException(); }
                            if (ProcessOleDbConnection(@"E:\Projects\BlueDeck\OrgChart1.accdb")) {
                                ProcessListMenu();
                            }
                            else {
                                Console.WriteLine("Could not connect to data source.");
                                break;
                            }                            
                            break;
                        default:
                            if (!File.Exists(input)) { throw new FileNotFoundException(); }
                            if (ProcessOleDbConnection(input)) {
                                ProcessListMenu();
                            }
                            else {
                                Console.WriteLine("Could not connect to data source.");
                                break;
                            }
                            break;
                    }
                }
                catch (FileNotFoundException e) {
                    Console.WriteLine(@"File {input} could not be found. Confirm that the file path is correct.");
                }
            }
        }

        static void ProcessListMenu() {
            string[] response = new string[] { "init" };

            while (response[0].Trim() != "exit") {
                Console.WriteLine("===COMPONENT PARSING OPTIONS===." +
                    "\nEnter '[ComponentName] -nest' to generate a nested XML tree" +
                    "\nEnter '[ComponentName] -flat' to generate a flat XML file." +
                    "\nType 'exit' to exit.");
                response = Console.ReadLine().Split('-');
                if (response.Count() > 1) {
                    switch (response[1]) {
                        case "flat":
                            Console.WriteLine(new DataBreaker(Components).BreakThisShit(response[0].Trim(), false));
                            break;
                        case "nest":
                            Console.WriteLine(new DataBreaker(Components).BreakThisShit(response[0].Trim()));
                            break;
                        default:
                            Console.WriteLine($@"-{response[1]} is not a recognized command.");
                            break;
                    }
                }
                else {
                    Console.WriteLine("You must provide a Component Name and a parsing option.");
                }
            }
        }

        static bool ProcessOleDbConnection(string pFilePath = @"E:\Projects\BlueDeck\OrgChart1.accdb") {
            OleDbConnection conn = new OleDbConnection {
                ConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data source={pFilePath}"
            };

            try {
                Components = new List<Component>();
                conn.Open();
                OleDbDataReader reader = null;
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM District_I_Map", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    Component n = new Component {
                        ComponentID = Convert.ToInt32(reader[0]),
                        ComponentName = reader[2].ToString(),
                        ParentComponentID = Convert.ToInt32(reader[1]),
                        ParentComponentName = reader[3].ToString()
                    };
                    Console.WriteLine(string.Format($"\nAdding {n.ComponentName} to Component List..."));
                    Components.Add(n);
                }
                NestFlatComponentList();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
            finally {
                conn.Close();
            }
        }

        static void NestFlatComponentList() {
            int i = 0;
            foreach (Component current in Components) {
                i++;
                Console.WriteLine($"Processing component number {i}, current master list count is: " + Components.Count);


                // Gather My Children
                //List<Component> childList = new List<Component>();
                foreach (Component child in Components) {
                    if (child.ParentComponentID == current.ComponentID) {
                        //childList.Add(child);
                        current.DirectChildren.Add(child);
                    }
                }
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace OrgXMLParser
{
    class DataBreaker
    {
        public List<Component> MasterList { get; set; }
        private const string defaultOutputPath = @"F:\Projects\BlueDeck\DataBreakerOutPut";
        public DataBreaker(List<Component> ls)
        {
            this.MasterList = ls;
        }
        

        public string BreakThisShit(string pName, bool pMakeNestedList = true, bool pJsonOutputFlag = false)
        {
            Component c = MasterList.Find(x => x.ComponentName == pName);
            string resultMessage;
             if (c != null)
            {
                if (pMakeNestedList) {                    
                        if (pJsonOutputFlag) {
                            // Json Out
                            // var result = ParseComponentToNestedJsonList(c, new List<Component>());
                            resultMessage = $"Nested JSON is not yet supported.";
                        }
                        else {
                            var result = ParseComponentToNestedXMLList(c, new XElement("Master", "PGPDOrg"));
                            result.Save(defaultOutputPath + ".xml");
                            resultMessage = $"Nested XML generated at {defaultOutputPath}.xml";
                        }
                } else {
                    if (pJsonOutputFlag) {
                        // Json Out
                        var result = ParseComponentToFlatJsonList(c, new List<Component>());
                        string json = JsonConvert.SerializeObject(result.ToArray());
                        System.IO.File.WriteAllText(defaultOutputPath + ".json", json);
                        resultMessage = $"Flat JSON generated at {defaultOutputPath}.json";
                    }
                    else {
                        var result = ParseComponentToFlatXMLList(c, new XElement("Master", "PGPDOrg"));
                        result.Save(defaultOutputPath + ".xml");
                        resultMessage = $"Flat XML generated at {defaultOutputPath}.xml";
                    }
                }               
            }
            else
            {
                resultMessage = $@"Unable to find a component named {pName} in the Component List." ;
            }
            return resultMessage;
        }

        public XElement ParseComponentToNestedXMLList(Component pComponent, XElement pElement)
        {
            XElement x = new XElement(
                "componentName", pComponent.ComponentName,
                new XAttribute("id", Convert.ToInt32(pComponent.ComponentID)),
                new XAttribute("parentId", Convert.ToInt32(pComponent.ParentComponentID))
                );
            pElement.Add(x);
            foreach(Component chld in pComponent.DirectChildren)
            {
                ParseComponentToNestedXMLList(chld, x);
            }
            return x;

        }

        public XElement ParseComponentToFlatXMLList(Component pComponent, XElement pElement) {
            XElement x = new XElement(
                "componentName", pComponent.ComponentName,
                new XAttribute("id", Convert.ToInt32(pComponent.ComponentID)),
                new XAttribute("parentId", Convert.ToInt32(pComponent.ParentComponentID))
                );
            pElement.Add(x);
            foreach (Component chld in pComponent.DirectChildren) {
                ParseComponentToFlatXMLList(chld, pElement);                
            }
            return pElement;
        }



        public List<GetOrgChartOutputClass> ParseComponentToFlatJsonList(Component pComponent, List<Component> pComponents) {
            Component x = new Component {
                ComponentID = pComponent.ComponentID,
                ParentComponentID = pComponent.ParentComponentID,
                ComponentName = pComponent.ComponentName
            };
            pComponents.Add(x);
            foreach(Component chld in pComponent.DirectChildren) {
                ParseComponentToFlatJsonList(chld, pComponents);
            }
            List<GetOrgChartOutputClass> outList = new List<GetOrgChartOutputClass>();
            foreach(Component y in pComponents) {
                GetOrgChartOutputClass z = new GetOrgChartOutputClass {
                    id = y.ComponentID,
                    parentId = y.ParentComponentID,
                    componentName = y.ComponentName
                };
                outList.Add(z);
            }
            return outList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrgXMLParser
{
    class DataBreaker
    {
        public List<Component> MasterList { get; set; }

        public DataBreaker(List<Component> ls)
        {
            this.MasterList = ls;
        }
        

        public string BreakThisShit(string pName, bool pMakeNestedList = true)
        {
            Component c = MasterList.Find(x => x.ComponentName == pName);
            XElement result = new XElement("Master", "PGPDOrg");
            if (c != null)
            {
                switch (pMakeNestedList) {                    
                    case true:
                        result = ParseComponentToNestedList(c, new XElement("Master", "PGPDOrg"));
                        result.Save(@"E:\Projects\BlueDeck\Result.xml");
                        break;
                    case false:
                        result = ParseComponentToFlatList(c, new XElement("Master", "PGPDOrg"));
                        result.Save(@"E:\Projects\BlueDeck\Result.xml");
                        break;
                }                
            }
            else
            {
                return $@"Unable to find a component named {pName} in the Component List." ;
            }
            return @"List was successfully generated at E:\Projects\BlueDeck\Result.xml";
        }

        public XElement ParseComponentToNestedList(Component pComponent, XElement pElement)
        {
            XElement x = new XElement(
                "Component", pComponent.ComponentName,
                new XAttribute("componentID", pComponent.ComponentID),
                new XAttribute("parentComponentID", pComponent.ParentComponentID)
                );
            pElement.Add(x);
            foreach(Component chld in pComponent.DirectChildren)
            {
                ParseComponentToNestedList(chld, x);
            }
            return x;

        }

        public XElement ParseComponentToFlatList(Component pComponent, XElement pElement) {
            XElement x = new XElement(
            "Component", pComponent.ComponentName,
            new XAttribute("componentID", pComponent.ComponentID),
            new XAttribute("parentComponentID", pComponent.ParentComponentID)
            );
            pElement.Add(x);
            foreach (Component chld in pComponent.DirectChildren) {
                ParseComponentToFlatList(chld, pElement);                
            }
            return pElement;
        }

    }
}

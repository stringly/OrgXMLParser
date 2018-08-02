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
        

        public int BreakThisShit(string pName)
        {
            Component c = MasterList.Find(x => x.ComponentName == pName);
            if (c != null)
            {
                XElement result = ParseThisDirtyWhore(c, new XElement("Master", "PGPDOrg"));
                result.Save(@"F:\Projects\BlueDeck\Result.xml");
                
                
            }
            else
            {
                return 0;
            }

            return 1;

        }

        public XElement ParseThisDirtyWhore(Component pComponent, XElement pElement)
        {
            XElement x = new XElement("Component", pComponent.ComponentName);
            pElement.Add(x);
            foreach(Component chld in pComponent.DirectChildren)
            {
                ParseThisDirtyWhore(chld, x);
            }
            return x;

        }


    }
}

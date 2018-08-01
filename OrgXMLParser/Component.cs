using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgXMLParser
{
    class Component
    {
        public int ComponentID { get; set; }
        public int ParentComponentID { get; set; }
        public string ComponentName { get; set; }
        public string ParentComponentName { get; set; }
        public List<Component> DirectChildren { get; set; }

        public Component()
        {
            this.DirectChildren = new List<Component>();
        }

    }
}

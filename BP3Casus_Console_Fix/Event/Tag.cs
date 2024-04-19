using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Event
{
    public class Tag
    {
        public int ID { get; set; }
        public string TagName { get; set; }

        public Tag(int id, string tagName)
        {
            ID = id;
            TagName = tagName;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Coder.Model
{
    public class CodeItem
    {
        public string Name { get; set; }

        public string Descroption { get; set; }

        public string Content { get; set; }

        public override string ToString()
        {
            return Content;
        }

        public CodeItem()
        {
            
        }

        public CodeItem(string name, string content, string description = null)
        {
            Name = name;
            Content = content;
            Descroption = description;
        }
    }
}

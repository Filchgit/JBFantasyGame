using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Item
    {
        protected string descrItem;
        public string DescrItem
        {
            get { return descrItem; }
            set { descrItem = value; }
        }

        public Item() => descrItem = "There seems to be nothing much here.";
        public Item(string newdescrItem) => descrItem = newdescrItem;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class CharParty : List <Character>
    {
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}

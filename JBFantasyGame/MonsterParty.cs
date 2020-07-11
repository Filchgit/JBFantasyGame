using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class MonsterParty : List<Monster> 
    {   
            protected string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        public MonsterParty() => name = "The Party has no name.";
    }
}

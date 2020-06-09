using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class PhysObj
    {
        protected string descrPhysObj;
        public string DescrPhysObj
        {
            get { return descrPhysObj; }
            set { descrPhysObj = value; }
        }
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public PhysObj() => descrPhysObj = "There seems to be nothing much here.";
        public PhysObj(string newdescrPhysObj) => descrPhysObj = newdescrPhysObj;
    }
}

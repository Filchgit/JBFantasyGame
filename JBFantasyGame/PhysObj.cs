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
        protected string objType;
        public string ObjType                           // Types I am thinking of at this stage include Armour, Melee 1 Hand , Melee 2 Hand, Missile, Magic, Misc 
        {
            get { return objType; }
            set { objType = value; }
        }
        protected bool isEquipped;
        public bool IsEquipped
        {
            get { return isEquipped; }
            set { isEquipped = value; }
        }
        protected int  aCEffect;                       // Effect on AC if equipped 
                                                       // higher AC is better; harder to ht, base AC of humans is 0.
         public int  ACEffect
        {
            get { return aCEffect; }
            set { aCEffect = value; }
        }
        protected string damage;                       // Damage Effect if equipped 

        public string Damage
        {
            get { return damage; }
            set { damage = value; }
        }


        public PhysObj () => descrPhysObj = "There seems to be nothing much here.";
        public PhysObj (string newdescrPhysObj) => descrPhysObj = newdescrPhysObj;
    }
}

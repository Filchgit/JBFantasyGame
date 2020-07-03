using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace JBFantasyGame
{
    public class Entity
    {
        protected bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // public Entity() => name = "The entity has no name.";
        // public Entity(string newName) => name = newName;

        protected string partyName;
        public string PartyName
        {
            get { return partyName; }
            set { partyName = value;  }
        }        
        public void Poke() => WriteLine($"{name} has been poked!");
        protected int lvl;
        public int Lvl
        {
            get { return lvl; }
            set
            {
               // if (value > 0 && value <= 30)
                  lvl = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected int hp;
        public int Hp
        {
            get { return hp; }
            set { hp = value;  }
        }
        protected int maxHp;
        public int MaxHp
        {
            get { return maxHp; }
            set
            {
               // if (value > 0 && value <= 1000)
                   maxHp = value;
               // else throw new ArgumentOutOfRangeException();
            }
        }
        protected int ac;                // armour class
        public int AC
        {
            get { return ac; }
            set
            { //if (value > -20 && value < 20)
                    ac = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected int hiton20;
        public int HitOn20
        {
            get { return hiton20; }
            set
            { // if (value > -50 && value < 50)
                   hiton20 = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected int initRoll;
        public int InitRoll
        {
            get { return initRoll; }
            set
            { 
                initRoll = value;
                //  else throw new ArgumentOutOfRangeException();
            }
        }

        public int damage;
      
         public List<PhysObj > Inventory = new List<PhysObj> { };                   // this obviously needs a lot of work kind of a placeholder to           
    }
}

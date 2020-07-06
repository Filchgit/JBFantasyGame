using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
        protected bool myTurn;
        public bool MyTurn
        {
            get { return myTurn; }
            set
            {
                myTurn = value;
                //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected string myTargetEnt;
        public string MyTargetEnt
        {
            get { return myTargetEnt; }
            set
            {
                myTargetEnt = value;
                //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected string myTargetParty;
        public string MyTargetParty
        {
            get { return myTargetParty; }
            set
            {
                myTargetParty = value;
                //  else throw new ArgumentOutOfRangeException();
            }
        }
        public virtual int MeleeAttack(Entity Defender)
        {
            RollingDie twentyside = new RollingDie(20, 1);
            int tohit;
            int attRoll = twentyside.Roll();
            if (Defender.AC < hiton20)
            { tohit = 20 - (hiton20 - Defender.AC); }
            else if (Defender.AC >= (hiton20 + 5))
            { tohit = 20 + ((Defender.AC - hiton20) - 5); }
            else tohit = 20;

            if (attRoll >= tohit)
            {
                   int damage = 8;                    //placeholder for damage 
                    Defender.Hp -= damage;
            }

            // same as  Defender.Hp = Defender.Hp - damage;                                                                             
            if (Defender.Hp <= 0)                                  // will change this to proper level for unconsciouness
            {
                // WriteLine($"{Defender.Name} has died.");
                Defender.IsAlive = false;

                return Defender.Hp;
            }
            
            else
            {// WriteLine($"{name} missed!");
                return Defender.Hp;
            }
}
        public virtual int MeleeAttack(Character Defender)
        {
            Defender.AC = 0;
            Character recalcACObject = new Character();
            Defender.AC= recalcACObject.ACRecalc(recalcACObject);          
            RollingDie twentyside = new RollingDie(20, 1);
            int tohit;
            int attRoll = twentyside.Roll();
            if (Defender.AC < hiton20)
            { tohit = 20 - (hiton20 - Defender.AC); }
            else if (Defender.AC >= (hiton20 + 5))
            { tohit = 20 + ((Defender.AC - hiton20) - 5); }
            else tohit = 20;

                               
              //  string damagerange = "";
            
             // if (damagerange is null)
             int  damage = 8;

            if (attRoll >= tohit)
            {   Defender.Hp -= damage;                          // same as  Defender.Hp = Defender.Hp - damage;                                                                             
                if (Defender.Hp <= 0)                                  // will change this to proper level for unconsciouness
                {
                    // WriteLine($"{Defender.Name} has died.");
                    Defender.IsAlive = false;
                }
                return Defender.Hp;
            }
            else
            {// WriteLine($"{name} missed!");
                return Defender.Hp;
            }
        }



        public int damage;
      
         public List<PhysObj > Inventory = new List<PhysObj> { };
        public List<Target> MeleeTargets = new List<Target> { };     // or it may be better to add range as an attribute to Target 
        public List<Target > TargetsAtRange = new List<Target> { };
    }
}

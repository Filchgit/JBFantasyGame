using System;
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
        public void Poke() => WriteLine($"{name} has been poked!");
        protected int lvl;
        public int Lvl
        {
            get { return lvl; }
            set
            {
               // if (value > 0 && value <= 30)
              //      lvl = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected int hp;
        public int Hp
        {
            get { return hp; }
            set
            {
              //  if (value <= 1000)
              //      hp = value;
              //  else throw new ArgumentOutOfRangeException();

            }
        }
        protected int maxHp;
        public int MaxHp
        {
            get { return maxHp; }
            set
            {
               // if (value > 0 && value <= 1000)
               //     maxHp = value;
               // else throw new ArgumentOutOfRangeException();
            }
        }
        protected int ac;                // armour class
        public int AC
        {
            get { return ac; }
            set
            { //if (value > -20 && value < 20)
              //      ac = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }
        protected int hiton20;
        public int HitOn20
        {
            get { return hiton20; }
            set
            { // if (value > -50 && value < 50)
              //      hiton20 = value;
              //  else throw new ArgumentOutOfRangeException();
            }
        }

        public int damage;
        public virtual int MeleeAttack(Entity Defender)
        {
            RollingDie twentyside = new RollingDie(20, 1);
            int tohit;
            int attRoll = twentyside.Roll();
            if (Defender.ac > hiton20)
            { tohit = (20 - (hiton20 + Defender.ac)); }
            else if (Defender.ac < (hiton20 - 5))
            { tohit = 20 - (hiton20 + (Defender.ac + 5)); }
            else tohit = 20;

            if (attRoll >= tohit)
            {
                RollingDie eightsided = new RollingDie(8, 1);
                damage = eightsided.Roll();
                Defender.Hp = Defender.Hp - damage;
                WriteLine($"{name} did  {damage } damage to {Defender.Name}");
                if (Defender.Hp <= 0)
                {
                    WriteLine($"{Defender.Name} has died.");
                    Defender.IsAlive = false;
                }
                return Defender.Hp;
            }
            else
            { WriteLine($"{name} missed!");
                return Defender.Hp; }
        }
        
         //public List<Item> Inventory = new List<Item>{ };
          
    }
}

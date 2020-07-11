using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Target
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
            set { partyName = value; }
        }
        protected int hp;
        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }
        protected int maxHp;
        public int MaxHp
        {
            get { return maxHp; }
            set {maxHp = value;}
        }
    }
    }

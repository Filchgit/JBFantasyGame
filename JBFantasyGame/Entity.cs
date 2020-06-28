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

        public int damage;
      //  public virtual int MeleeAttack(Entity Defender)                will put this back for monsters in simplified form 
      //  {
      //      RollingDie twentyside = new RollingDie(20, 1);
      //      int tohit;
      //      int attRoll = twentyside.Roll();
     //       if (Defender.ac > hiton20)
    //        { tohit = (20 - (hiton20 + Defender.ac)); }
      //      else if (Defender.ac < (hiton20 - 5))
     //       { tohit = 20 - (hiton20 + (Defender.ac + 5)); }
     //       else tohit = 20;

      //      if (attRoll >= tohit)
       //     {
       //         int damage = 0;
       //         string damagerange;
       //         foreach (PhysObj CheckObject in this.Inventory)
       //         {
        //            if (CheckObject.IsEquipped = true && CheckObject.ObjType is "Weapon")     // this was just a rough first concept check  
        //            { damagerange = CheckObject.Damage; }                                     // need to allow for two handed etc etc etc 
         //           else damagerange = "1d1";
         //           (int i1, int i2) = RollingDie.Diecheck(damagerange);
         //           RollingDie thisRoll = new RollingDie(i1, i2);
          //          damage = thisRoll.Roll();                                                 
          //      }
          //      int DamStrAdj = 0;                    // + str adj to damage
          //      if (this.Str <= 3)
          //      { ToHitStrAdj = -3; }
          //      else if (a_character.Str <= 5)
          //      { ToHitStrAdj = -2; }
          //      else if (a_character.Str <= 7)
          //      { ToHitStrAdj = -1; }
          //      else if (a_character.Str > 17)
          //      { ToHitStrAdj = 1; }

          //      Defender.Hp -= damage;                          // same as  Defender.Hp = Defender.Hp - damage;                                                                             
          //      if (Defender.Hp <= 0)
          //      {
                   // WriteLine($"{Defender.Name} has died.");
           //         Defender.IsAlive = false;
           //     }
           //     return Defender.Hp;
           // }
           // else
           // {// WriteLine($"{name} missed!");
           //     return Defender.Hp; }
      //  }       
         public List<PhysObj > Inventory = new List<PhysObj> { };                   // this obviously needs a lot of work kind of a placeholder to           
    }
}

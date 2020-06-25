using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using static System.Console;

namespace JBFantasyGame
{
    public class Mage : Character                                                             // these names may change slightly , but they will do for starting classes 
    {
      //  public Mage(string newName) : base(newName) { }
        
        public override int  MeleeAttack(Entity Defender)                                  // just did this to check override function hiton20  will calculate %chance to hits and euipped weapons + str modifiers will do damage really.
        {
            RollingDie foursided = new RollingDie(4, 1);
            damage = foursided.Roll();
            Defender.Hp = Defender.Hp - damage;
            WriteLine($"{name} did  {damage } damage to {Defender.Name}");
            return Defender.Hp;
        }
        

    }
}

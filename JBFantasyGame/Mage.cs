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
        
       // public override int  MeleeAttack(Entity Defender)                                  // just did this to check override function hiton20  will calculate %chance to hits and euipped weapons + str modifiers will do damage really.
       // {
       //     RollingDie foursided = new RollingDie(4, 1);
       //     damage = foursided.Roll();
       //     Defender.Hp = Defender.Hp - damage;
       //     WriteLine($"{name} did  {damage } damage to {Defender.Name}");
       //     return Defender.Hp;
       // }
        public static Character MageInitialize(Character a_character)
        {
            if (a_character.Exp <= 2500)                                  // this are straight from AD&D atm but will change as time goes on, will also have a better
            { a_character.Lvl = 1; }                                       // check when going between levels by gaining experience
            else if (a_character.Exp <= 5000)
            { a_character.Lvl = 2; }
            else if (a_character.Exp <= 10000)
            { a_character.Lvl = 3; }
            else if (a_character.Exp <= 22500)
            { a_character.Lvl = 4; }
            else if (a_character.Exp <= 40000)
            { a_character.Lvl = 5; }
            else if (a_character.Exp <= 60000)
            { a_character.Lvl = 6; }
            else if (a_character.Exp <= 90000)
            { a_character.Lvl = 7; }
            else if (a_character.Exp <= 135000)
            { a_character.Lvl = 8; }
            else if (a_character.Exp <= 250000)
            { a_character.Lvl = 9; }
            else if (a_character.Exp <= 375000)
            { a_character.Lvl = 10; }
            else if (a_character.Exp <= 750000)
            { a_character.Lvl = 11; }
            else
            { a_character.Lvl = 12; }   
                                                        // gives initial level based on Experience points
            int HpConAdj = 0;
            if (a_character.Con <= 3)                  //Constitution Initial Hp bonuses different only for fighters I think
            { HpConAdj = -2; }
            else if (a_character.Con <= 6)
            { HpConAdj = -1; }
            else if (a_character.Con == 15)
            { HpConAdj = 1; }
            else if (a_character.Con >= 16)
            { HpConAdj = 2; }          

            if (a_character.Lvl < 11)                                                // as above level 11 just add hp I think
            {
                RollingDie lvl4d = new RollingDie(4, a_character.Lvl);
                int BaseHp = lvl4d.Roll();                                         // just cause I wanna watch it clearly
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj);
                a_character.Hp = a_character.MaxHp;
            }
            MageRecalcHitOn20(a_character);
            return a_character;
        }
        public static Character MageRecalcHitOn20(Character a_character)
        {
            int ToHitStrAdj = 0;                    // + str adj to hit 
            if (a_character.Str <= 3)
            { ToHitStrAdj = -3; }
            else if (a_character.Str <= 5)
            { ToHitStrAdj = -2; }
            else if (a_character.Str <= 7)
            { ToHitStrAdj = -1; }
            else if (a_character.Str > 17)
            { ToHitStrAdj = 1; }

            int calcHiton20 = 0;                                           
            int baseHiton20;
            if (a_character.Lvl <= 5)                            // might end up smoothing these by adding in between HitOn20s 
            { baseHiton20 = 1; }
            else if (a_character.Lvl <= 10)
            { baseHiton20 = -1; }
            else if (a_character.Lvl <= 15)
            { baseHiton20 = -4; }
            else if (a_character.Lvl <= 20)
            { baseHiton20 = -7; }            
            else { baseHiton20 = -9; }
 
            calcHiton20 = baseHiton20 - ToHitStrAdj;
            a_character.HitOn20 = calcHiton20;
            return a_character;

        }
    }

}


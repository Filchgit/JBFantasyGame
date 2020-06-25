using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Cleric :Character
    {
        public static Character ClericInitialize(Character a_character)
        {
            if (a_character.Exp <= 1500)                                  // this are straight from AD&D atm but will change as time goes on, will also have a better
            { a_character.Lvl = 1; }                                      // check when going between levels by gaining experience
            else if (a_character.Exp <= 3000)
            { a_character.Lvl = 2; }
            else if (a_character.Exp <= 6000)
            { a_character.Lvl = 3; }
            else if (a_character.Exp <= 13000)
            { a_character.Lvl = 4; }
            else if (a_character.Exp <= 27500)
            { a_character.Lvl = 5; }
            else if (a_character.Exp <= 55000)
            { a_character.Lvl = 6; }
            else if (a_character.Exp <= 110000)
            { a_character.Lvl = 7; }
            else if (a_character.Exp <= 225000)
            { a_character.Lvl = 8; }
            else if (a_character.Exp <= 450000)
            { a_character.Lvl = 9; }
            else if (a_character.Exp <= 675000)
            { a_character.Lvl = 10; }
            else
            { a_character.Lvl = 11; }
                                             // gives initial level based on Experience points
            int HpConAdj=0;
            if (a_character.Con <=3)                  //Constitution Initial Hp bonuses different only for fighters I think
            { HpConAdj = -2; }
            else if (a_character.Con <=6 )
            { HpConAdj = -1; }
            else if (a_character.Con == 15)
            {  HpConAdj = 1;  }
            else if (a_character.Con >= 16)
            { HpConAdj = 2; }
            if (a_character.Lvl < 9)
            {
                RollingDie lvl8d = new RollingDie(8, a_character.Lvl);            // this is the same as rolling a eight sided dice times the level and totalling
                a_character.MaxHp = lvl8d.Roll() + (a_character.Lvl* HpConAdj);   // *RollDie d8 + con adjustments }
                a_character.Hp = a_character.MaxHp;
            }                                                                              
            return a_character;
        }    
    }                                                       
}

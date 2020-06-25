using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Fighter : Character
    {
        public static Character FighterInitialize(Character a_character)
        {
            if (a_character.Exp <= 20000)                                  // this are straight from AD&D atm but will change as time goes on, will also have a better
            { a_character.Lvl = 1; }                                      // check when going between levels by gaining experience
            else if (a_character.Exp <= 4000)
            { a_character.Lvl = 2; }
            else if (a_character.Exp <= 8000)
            { a_character.Lvl = 3; }
            else if (a_character.Exp <= 18000)
            { a_character.Lvl = 4; }
            else if (a_character.Exp <= 35000)
            { a_character.Lvl = 5; }
            else if (a_character.Exp <= 70000)
            { a_character.Lvl = 6; }
            else if (a_character.Exp <= 125000)
            { a_character.Lvl = 7; }
            else if (a_character.Exp <= 250000)
            { a_character.Lvl = 8; }
            else if (a_character.Exp <= 500000)
            { a_character.Lvl = 9; }
            else if (a_character.Exp <= 750000)
            { a_character.Lvl = 10; }
            else
            { a_character.Lvl = 11; }
            // gives initial level based on Experience points
            int HpConAdj = 0;
            if (a_character.Con <= 3)                  //Constitution Initial Hp bonuses different only for fighters I think
            { HpConAdj = -2; }
            else if (a_character.Con <= 6)
            { HpConAdj = -1; }
            else if (a_character.Con == 15)
            { HpConAdj = 1; }
            else if (a_character.Con == 16)
            { HpConAdj = 2; }
            else if (a_character.Con == 17)
            { HpConAdj = 3; }
            else if (a_character.Con == 18)
            { HpConAdj = 4; }
            if (a_character.Lvl < 9)
            {
                RollingDie lvl10d = new RollingDie(10, a_character.Lvl);            // this is the same as rolling a eight sided dice times the level and totalling
                int BaseHp = lvl10d.Roll();                                         // just cause I wanna watch it
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj);   // *RollDie d8 + con adjustments }
                a_character.Hp = a_character.MaxHp;
            }                                                                         // also should do initial THACO here I think 
            return a_character;
        }
        public static Character FighterRecalcThaco(Character a_character)
        {
            return a_character;
        }
    }
}


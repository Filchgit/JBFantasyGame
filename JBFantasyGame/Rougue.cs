using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Rougue : Character
    {


        public static Character RogueInitialize(Character a_character)
        {
            if (a_character.Exp <= 1250)                                  // this are straight from AD&D atm but will change as time goes on, will also have a better
            { a_character.Lvl = 1; }                                       // check when going between levels by gaining experience
            else if (a_character.Exp <= 2500)
            { a_character.Lvl = 2; }
            else if (a_character.Exp <= 5000)
            { a_character.Lvl = 3; }
            else if (a_character.Exp <= 10000)
            { a_character.Lvl = 4; }
            else if (a_character.Exp <= 20000)
            { a_character.Lvl = 5; }
            else if (a_character.Exp <= 42500)
            { a_character.Lvl = 6; }
            else if (a_character.Exp <= 70000)
            { a_character.Lvl = 7; }
            else if (a_character.Exp <= 110000)
            { a_character.Lvl = 8; }
            else if (a_character.Exp <= 160000)
            { a_character.Lvl = 9; }
            else if (a_character.Exp <= 220000)
            { a_character.Lvl = 10; }
            else if (a_character.Exp <= 440000)
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
                RollingDie lvl6d = new RollingDie(6, a_character.Lvl);
                int BaseHp = lvl6d.Roll();                                         // just cause I wanna watch it clearly
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj);
                a_character.Hp = a_character.MaxHp;
            }
            else
            {
                RollingDie lvl6d = new RollingDie(6, a_character.Lvl);
                int BaseHp = lvl6d.Roll();                                         // just cause I wanna watch it clearly
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj) +(2*(a_character.Lvl-11)) ;
                a_character.Hp = a_character.MaxHp;
            }

            RogueRecalcHitOn20(a_character);
            return a_character;
        }
        public static Character RogueRecalcHitOn20(Character a_character)
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
            if (a_character.Lvl <= 4)                            // might end up smoothing these by adding in between HitOn20s 
            { baseHiton20 = 1; }
            else if (a_character.Lvl <= 8)
            { baseHiton20 = -1; }
            else if (a_character.Lvl <= 12)
            { baseHiton20 = -4; }
            else if (a_character.Lvl <= 16)
            { baseHiton20 = -6; }
            else if (a_character.Lvl <= 20)
            { baseHiton20 = -8; }
            else { baseHiton20 = -10; }

            calcHiton20 = baseHiton20 - ToHitStrAdj;
            a_character.HitOn20 = calcHiton20;
            return a_character;

        }
    }
}


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
            if (a_character.Exp <= 375 )                   //2000 the commented Xps are straight from AD&D atm but will change as time goes on, will also have a better
            { a_character.Lvl = 1; }                      // check when going between levels by gaining experience
            else if (a_character.Exp <= 1405)               // 4000  might be funner to have this check a sql table so  that players can easily edit it 
            { a_character.Lvl = 2; }                                 // atm just redone curve for fighter off an exponential kills needed at same level curve
            else if (a_character.Exp <= 3820)                 // 8000)   for fighter and adjusted other calsses off that 
            { a_character.Lvl = 3; }
            else if (a_character.Exp <= 8890)                  // 18000)
            { a_character.Lvl = 4; }
            else if (a_character.Exp <= 17945)                   //35000)
            { a_character.Lvl = 5; }
            else if (a_character.Exp <=  34460)                 //70000)
            { a_character.Lvl = 6; }
            else if (a_character.Exp <=  64070)                //125000)
            { a_character.Lvl = 7; }
            else if (a_character.Exp <= 117800)                 // 250000)
            { a_character.Lvl = 8; }
            else if (a_character.Exp <= 211350)                        //500000)
            { a_character.Lvl = 9; }
            else if (a_character.Exp <= 367175)                         // 750000)
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
            if (a_character.Lvl <= 9)
            {
                RollingDie lvl10d = new RollingDie(10, a_character.Lvl);
                int BaseHp = lvl10d.Roll();                                         // just cause I wanna watch it clearly
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj) ;       // now adjusted for levels above 9 
                a_character.Hp = a_character.MaxHp;
            }
            else
            {
                RollingDie lvl10d = new RollingDie(10, a_character.Lvl);
                int BaseHp = lvl10d.Roll();                                         // just cause I wanna watch it clearly
                a_character.MaxHp = BaseHp + (a_character.Lvl * HpConAdj) + (3 * (a_character.Lvl - 9));       // now adjusted for levels above 9 
                a_character.Hp = a_character.MaxHp;
            }
            FighterRecalcHitOn20(a_character);
            return a_character;
        }
        public static Character FighterRecalcHitOn20(Character a_character)           
        {
            int ToHitStrAdj = 0;                    // + str adj to hit 
            if (a_character.Str  <= 3)                  
            { ToHitStrAdj = -3; }                        
            else if (a_character.Str <= 5)
            { ToHitStrAdj = -2; }
            else if (a_character.Str <= 7)
            { ToHitStrAdj = -1; }
            else if (a_character.Str >= 17)
            { ToHitStrAdj = 1; }
                                     
                int  calcHiton20 = 10 + (a_character.Lvl - 1) + ToHitStrAdj;       //HitOn20 for fighters is 10 at level 1, Increases by 1 per level
            a_character.HitOn20 = calcHiton20;                                    // better AC is positive, starts at AC:0 for unarmoured.    
            return a_character;
        }
    }
}


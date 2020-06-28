using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace JBFantasyGame
{
    public class Character : Entity
    #region Character Stats
    {
        protected int str;
        public int Str
        {
            get { return str; }
            set
            {
                if (value > 0 && value <= 25)
                    str = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        protected int inte;
        public int Inte
        {
            get { return inte; }
            set
            {
                if (value > 0 && value <= 25)
                    inte = value;
                else throw new ArgumentOutOfRangeException();
            }

        }
        protected int wis;
        public int Wis
        {
            get { return wis; }
            set
            {
                if (value > 0 && value <= 25)
                    wis = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        protected int dex;
        public int Dex
        {
            get { return dex; }
            set
            {
                if (value > 0 && value <= 25)
                    dex = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        protected int con;
        public int Con
        {
            get { return con; }
            set
            {
                if (value > 0 && value <= 25)
                    con = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
        protected int chr;
        public int Chr
        {
            get { return chr; }
            set
            {
                if (value > 0 && value <= 25)
                    chr = value;
                else throw new ArgumentOutOfRangeException();
            }
        }
       
        protected int exp;
        public int Exp
        {
            get { return exp; }
            set
            {
                exp = value;
            }
        }
        protected String charType;                                  // think I will use type for class of character
        public String  CharType
        {
            get { return charType; }
            set
            {
                charType = value;              
            }
        }


        #endregion
        public void Iam() => WriteLine($"I am {name}!");
       // public Character(string newName) : base(newName) { }
        public virtual Character NewCharacter (Character a_character)
        { 
            RerollCharacter(a_character);
            a_character.isAlive = true;
            
            a_character.AC = 9;
            a_character.exp = 120000;
            a_character.hiton20 = 0;                                       // temporary to test combat
            a_character.MaxHp =8;                                        // temporary to test combat
            a_character.Hp = 8;                                            // ditto
            a_character.CharType = "Mage Test";
            a_character = Fighter.FighterInitialize(a_character);
            return a_character;
            
         }
        public Character RerollCharacter(Character chartoreroll)

        {
            RollingDie three6d = new RollingDie(6, 3);       // this is the same as rolling a six sided dice three times and totalling
            chartoreroll.Str = three6d.Roll();
            chartoreroll.Inte = three6d.Roll();
            chartoreroll.Wis = three6d.Roll();
            chartoreroll.Dex = three6d.Roll();
            chartoreroll.Con = three6d.Roll();
            chartoreroll.Chr = three6d.Roll();
            return chartoreroll;
        }


    }
}    


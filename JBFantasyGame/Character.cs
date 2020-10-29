using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Console;
using System.Data.Linq; 

namespace JBFantasyGame
{
     
    public class Character : Fant_Entity
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
        protected String charType;                                  
        public String CharType
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
        public virtual Character NewCharacter(Character a_character)
        {
            RerollCharacter(a_character);
            a_character.isAlive = true;
            a_character.Name = "Default";
            a_character.AC =0;
            a_character.exp = 120000;
            a_character.hitOn20 = 0;                                       // temporary to test combat
            a_character.MaxHp = 8;                                        // temporary to test combat
            a_character.Hp = 8;                                            // ditto
            a_character.CharType = "Fighter";
            a_character = Fighter.FighterInitialize(a_character);
            return a_character;

        }
        public static Character RerollCharacter(Character chartoreroll)

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
        public int InitRecalc(Character characterIn)
        {
            InitMod = 0;
            if (characterIn.Dex == 3)
            { InitMod = 16; }
            if (characterIn.Dex == 4)
            { InitMod = 12; }
            if (characterIn.Dex == 5)
            { InitMod = 8; }
            if (characterIn.Dex == 6)
            { InitMod = 4; }
            if (characterIn.Dex == 15)
            { InitMod = -4; }
            else if (characterIn.Dex == 16)
            { InitMod = -8; }
            else if (characterIn.Dex == 17)
            { InitMod = -12; }
            else if (characterIn.Dex == 18)
            { InitMod = -16; }
            return InitMod; 
        }

        public int ACRecalc(Character characterIn )
           {
            AC = 0;
            int DexACAdj = 0;
            if (characterIn.Dex == 3)
            { DexACAdj = -4; }
            if (characterIn.Dex == 4)
            { DexACAdj = -3; }
            if (characterIn.Dex == 5)
            { DexACAdj = -2; }
            if (characterIn.Dex==6)
            { DexACAdj = -1; }
            if (characterIn.Dex ==15)
            { DexACAdj = 1; }
            else if (characterIn.Dex == 16)
            { DexACAdj = 2; }
            else if (characterIn.Dex == 17)
            { DexACAdj = 3; }
            else if (characterIn.Dex == 18)
            { DexACAdj = 4; }

            foreach (PhysObj  CheckObject in characterIn.Inventory)
            {
                if (CheckObject.IsEquipped == true) // && CheckObject.ObjType is "Armour") this was just a rough first concept check  
                                                     //actually doen't need the armour check, should save type to save double equipping
                {
                    AC = AC + CheckObject.ACEffect;
                }
            }
            AC = AC + DexACAdj;
            return AC ;
        }

        public override int MeleeAttack(Fant_Entity Defender)
        {          
            return Defender.Hp = characterAttackCalc(Defender);                   
        }

        public int characterAttackCalc(Fant_Entity Defender)
        {
            RollingDie twentyside = new RollingDie(20, 1);
            int tohit;
            int attRoll = twentyside.Roll();
            if (Defender.AC < hitOn20)
            { tohit = 20 - (hitOn20 - Defender.AC); }
            else if (Defender.AC >= (hitOn20 + 5))
            { tohit = 20 + ((Defender.AC - hitOn20) - 5); }
            else tohit = 20;

            if (attRoll >= tohit)
            {
                int damage = 0;
                string damagerange = "";
                foreach (PhysObj  CheckObject in this.Inventory)
                {
                    if (CheckObject.IsEquipped == true && CheckObject.ObjType is "Weapon")     // this was just a rough first concept check  
                    {
                        damagerange = CheckObject.Damage;                                      // need to allow for two handed etc etc etc 
                        (int i1, int i2, int i3) = RollingDie.Diecheck(damagerange);
                        RollingDie thisRoll = new RollingDie(i1, i2, i3);
                        damage = thisRoll.Roll();

                    }
                }
                int DamStrAdj = 0;                    // + str adj to damage
                if (this.Str <= 5)
                { DamStrAdj = -1; }
                else if (this.Str == 16 || this.Str == 17)
                { DamStrAdj = 1; }
                else if (this.Str >= 18)
                { DamStrAdj = 2; }

                damage = (damage + DamStrAdj);
                if (damage < 1)
                { damage = 1; }



                Defender.Hp -= damage;                          // same as  Defender.Hp = Defender.Hp - damage;                                                                             
                if (Defender.Hp <= 0)                                  // will change this to proper level for unconsciouness
                {
                    // WriteLine($"{Defender.Name} has died.");
                    Defender.IsAlive = false;
                }
                return Defender.Hp;
            }
            else
            {// WriteLine($"{name} missed!");
                return Defender.Hp;
            }
        }
        public override int  MeleeAttack(Character Defender)
        {          
                Defender.AC = 0;
                Defender.AC = ACRecalc(Defender);
            return Defender.Hp = characterAttackCalc(Defender);

        }
    }
}


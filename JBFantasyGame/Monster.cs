using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Monster : Entity
    {
        // might end up having a field for type of monster to facilitate quick data sheets etc 
        // for now just want to see if I can easily make groups and lists with monsters and characters 
        protected string hitDie;
        public string HitDie
        {
            get { return hitDie; }
            set { hitDie = value; }
        }
        protected int noOfAtt;
        public int NoOfAtt
        {
            get { return noOfAtt; }
            set { noOfAtt = value; }
        }
        protected string damPerAtt1;
        public string DamPerAtt1
        {
            get { return damPerAtt1; }
            set { damPerAtt1 = value; }
        }
        protected string damPerAtt2;
        public string DamPerAtt2
        {
            get { return damPerAtt2; }
            set { damPerAtt2 = value; }
        }
        protected string damPerAtt3;
        public string DamPerAtt3
        {
            get { return damPerAtt3; }
            set { damPerAtt3 = value; }
        }

        protected String monsterType;                                  // think I will use type for class of character
        public String MonsterType
        {
            get { return monsterType; }
            set
            {
                monsterType = value;
            }
        }
        public override int MeleeAttack(Entity Defender)
        {
            return Defender.Hp = monsterAttackCalc(Defender);
        }
        public override int MeleeAttack(Character Defender)
        {
            Defender.AC = 0;
            Character thisDefender = new Character();
            thisDefender = Defender;
            thisDefender.ACRecalc(thisDefender);
            Defender.Hp = monsterAttackCalc(thisDefender);
            return Defender.Hp;
        }
          public int monsterAttackCalc(Entity Defender)
        {
            int cumDamage = 0;
           
            RollingDie twentyside = new RollingDie(20, 1);
            int tohit;
             if (Defender.AC < hiton20)
            { tohit = 20 - (hiton20 - Defender.AC); }
            else if (Defender.AC >= (hiton20 + 5))
            { tohit = 20 + ((Defender.AC - hiton20) - 5); }
            else tohit = 20;

            int attRoll = twentyside.Roll();
              if (attRoll >= tohit)
            {
                
                string dieroll = this.damPerAtt1;
                (int i1, int i2, int i3) = RollingDie.Diecheck(dieroll);
                if (i1 != 0)
                {
                    RollingDie thisRoll = new RollingDie(i1, i2, i3);
                    int damage = thisRoll.Roll();
                    cumDamage += damage;
                }
            }
            if (this.NoOfAtt >= 2)
            {
                int attRoll2 = twentyside.Roll();
                if (attRoll2 >= tohit)
                {
                    string dieroll2 = this.damPerAtt2;
                    (int i1, int i2, int i3) = RollingDie.Diecheck(dieroll2);
                    if (i1 != 0)
                    {
                        RollingDie thisRoll = new RollingDie(i1, i2, i3);
                        int damage2 = thisRoll.Roll();
                        cumDamage += damage2;
                    }
                }
            }
            if (this.NoOfAtt >= 3)
            {
                int attRoll3 = twentyside.Roll();
                if (attRoll3 >= tohit)
                {
                    string dieroll3 = this.damPerAtt3;
                    (int i1, int i2, int i3) = RollingDie.Diecheck(dieroll3);
                    if (i1 != 0)
                    {
                        RollingDie thisRoll = new RollingDie(i1, i2, i3);
                        int damage3 = thisRoll.Roll();
                        cumDamage += damage3;
                    }
                }
            }
            Defender.Hp -= cumDamage;                          // same as  Defender.Hp = Defender.Hp - damage;                                                                             
            
                return Defender.Hp; }
        public virtual Monster NewMonster(Monster a_monster)
        {

            a_monster.isAlive = true;           
            a_monster.Name = "Default";
            a_monster.PartyName= "Default";
            a_monster.Lvl = 1;
            a_monster.Hp = 4;
            a_monster.MaxHp = 4;
            a_monster.AC = 0;
            a_monster.HitOn20 = 10;
            a_monster.InitMod = 0;
            a_monster.HitDie= "1d8";
            a_monster.NoOfAtt = 1;
            a_monster.MonsterType = "Gobbo!"; 
            return a_monster;

        }
    }                                        
}

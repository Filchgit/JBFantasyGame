using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace JBFantasyGame
{
    public class Troll : Monster
    {
     public static Monster TrollInitialize (Monster a_monster)
     {
       if (a_monster.Lvl >=6 && a_monster.Lvl <=10)              // Trolls in this are typically levels 6-10
            { }
       else { RollingDie fourD = new RollingDie(4);
                a_monster.Lvl = 5 + fourD.Roll();}
     a_monster.AC = 8;         
     a_monster.HitDie = "d10 +2";
     string timesroll = a_monster.Lvl.ToString();
     string dieroll = timesroll + a_monster.HitDie;
      (int i1, int i2, int i3) = RollingDie.Diecheck(dieroll);
        if (i1 != 0)
            {RollingDie thisRoll = new RollingDie(i1, i2, i3);
                a_monster.MaxHp = thisRoll.Roll();}
      a_monster.Hp = a_monster.MaxHp;
      a_monster.InitMod = 12;
      a_monster.NoOfAtt = 3;
      a_monster.DamPerAtt1 = "1d4 +3";
      a_monster.DamPerAtt2 = "1d4 +3";
      a_monster.DamPerAtt3 = "1d12 +4"; 
      
            return a_monster;
     }
    }
}

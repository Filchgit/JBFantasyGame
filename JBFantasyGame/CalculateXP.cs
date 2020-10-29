using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    class CalculateXP
    {
        private int lvlHPCalcs;
        private int hpXPCalcs;
        private double defeatMult;
        public CalculateXP(int lvlHPCalcs , int hpXPCalcs, double defeatMult)
        {
            this.lvlHPCalcs = lvlHPCalcs;
            this.hpXPCalcs = hpXPCalcs;
            this.defeatMult = defeatMult;
        }
        public double XPForDefeatCalc()
        {   if(defeatMult ==0)
            { defeatMult = 1; }
            double xPCalculatedAs = 0;
            double baseXP = 0;
            double xPPerHP = 0;
            if (lvlHPCalcs == 1)
            {
                baseXP = 10;
                xPPerHP = 1.25;
            }
            if (lvlHPCalcs == 2)
            {
                baseXP = 23.33;
                xPPerHP = 1.46;
            }
            if (lvlHPCalcs == 3)
            {
                baseXP = 46.66;
                xPPerHP = 1.94;
            }
            if (lvlHPCalcs == 4)
            {
                baseXP = 83.33;
                xPPerHP = 2.6;
            }
            if (lvlHPCalcs == 5)
            {
                baseXP = 130;
                xPPerHP = 3.25;
            }
            if (lvlHPCalcs == 6)
            {
                baseXP = 196.66;
                xPPerHP = 4.1;
            }
            if (lvlHPCalcs == 7)
            {
                baseXP = 300;
                xPPerHP = 5.36;
            }
            if (lvlHPCalcs == 8)
            {
                baseXP = 466.66;
                xPPerHP = 7.29;
            }
            if (lvlHPCalcs == 9)
            {
                baseXP = 690;
                xPPerHP = 9.58;
            }
            if (lvlHPCalcs == 10)
            {
                baseXP = 973;
                xPPerHP = 12.17;
            }
            xPCalculatedAs = defeatMult * (baseXP + (hpXPCalcs * xPPerHP));
            return xPCalculatedAs;
        }
    }
}

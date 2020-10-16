using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xaml.Schema;

namespace JBFantasyGame
{
    class RollingDie
    {
        private static readonly Random faceup = new Random();                           //change this to add number of times the dice is thrown
        private int sidesCount;
        private int timesRoll;
        private int dtot;
        private int modifier;
        public RollingDie(int sidesCount, int timesRoll = 1, int modifier =0)
        {
            this.timesRoll = timesRoll;
            this.sidesCount = sidesCount;
            this.modifier = modifier;
        }

        public int GetSidesCount()
        {
            return sidesCount;
        }

        public int Roll()
        {
            dtot = 0;
            for (int i = 0; i < timesRoll; i++)
            { dtot = faceup.Next(1, sidesCount + 1) + dtot + modifier; }
            return dtot ;
        }

        public override string ToString()
        {
            return String.Format("Rolling a die with {0} sides,{1} times and a modifier of {2}.", sidesCount, timesRoll, modifier );
        }


        public static (int i1, int i2, int i3) Diecheck(string diecheck)
        {
            string extraspace = " ";
            diecheck += extraspace;    
            //String diecheck = RollDieDM.Text;                                        
            string rex = "^([0-9]*)[D-d]([0-9]+)([ ]+)([+|-]*)([0-9]*)";
            if (Regex.IsMatch(diecheck, rex) == true)
            {
                string[] splitdie = diecheck.Split(new Char[] { 'D', 'd' ,' '});
                int i1;
                int i2;
                int i3 = 0;
                if (splitdie[0] != "")
                {
                    i2 = Int32.Parse(splitdie[0]);
                    i1 = Int32.Parse(splitdie[1]);                             // need to have a check for something is put after the 3d6 like 3d10Fred                 
                    string modifier = (splitdie[2]);
                    if (modifier != "")
                    { i3 = Int32.Parse(modifier); }

                          return (i1, i2, i3);
                }
                else
                {
                    i1 = Int32.Parse(splitdie[1]);
                    i2 = 1;
                    string modifier = (splitdie[2]);
                    if (modifier != "")
                    { i3 = Int32.Parse(modifier); }

                    return (i1, i2, i3);
                }
            }
           

           else
            {
                MessageBox.Show($"Not a valid input to roll dice, acceptable forms 3d6 , 4D8, 3d8 +8 !!note space!!, or d12 etc");
                int i1 = 0;
                int i2 = 0;
                int i3 = 0;
                return (i1, i2, i3);      // should do someting with nullable values instead here 
            }
            
        }
    }
}

    



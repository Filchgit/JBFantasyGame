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

        public RollingDie(int sidesCount, int timesRoll = 1)
        {
            this.timesRoll = timesRoll;
            this.sidesCount = sidesCount;
        }

        public int GetSidesCount()
        {
            return sidesCount;
        }

        public int Roll()
        {
            dtot = 0;
            for (int i = 0; i < timesRoll; i++)
            { dtot = faceup.Next(1, sidesCount + 1) + dtot; }
            return dtot;
        }

        public override string ToString()
        {
            return String.Format("Rolling a die with {0} sides,{1} times.", sidesCount, timesRoll);
        }


        public static (int i1, int i2) Diecheck(string diecheck)
        {
            //String diecheck = RollDieDM.Text;                                         // I think diecheck should be split off into RollingDie as a function
            string rex = "^([0-9]*)[D-d]([0-9]+)";
            if (Regex.IsMatch(diecheck, rex) == true)
            {
                string[] splitdie = diecheck.Split(new Char[] { 'D', 'd' });
                int i1;
                int i2;
                if (splitdie[0] != "")
                {
                    i2 = Int32.Parse(splitdie[0]);
                    i1 = Int32.Parse(splitdie[1]);                             // need to have a check for someting is put after the 3d6 like 3d10Fred
                    return (i1, i2);
                }
                else
                {
                    i1 = Int32.Parse(splitdie[1]);
                    i2 = 1;

                    return (i1, i2);
                }
            }
            //       RollingDie thisRoll = new RollingDie(i1, i2);
            // MessageBox.Show($"{thisRoll.Roll() } {RollDieDM.Text }");   // we will make this talk out to a rolling chat box in a sec

           else
            {
                MessageBox.Show($"Not a valid input to roll dice, should be in the form of 3d6 , 4D8, 1d20 or even d20");
                int i1 = 0;
                int i2 = 0;
                return (i1, i2);      // should do someting with nullable values instead here 
            }
            
        }
    }
}

    



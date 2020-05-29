using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace JBFantasyGame
{
        class RollingDie
        {
            private static readonly Random faceup =new Random();                           //change this to add number of times the dice is thrown
            private int sidesCount;
            private int timesRoll;
            private int dtot;
        
            public RollingDie(int sidesCount, int timesRoll=1)
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
            { dtot= faceup.Next(1, sidesCount + 1) + dtot; }
            return dtot;
              }
                   
        public override string ToString()
            {
                return String.Format("Rolling a die with {0} sides,{1} times.", sidesCount, timesRoll);
            }

        }

      
}

    



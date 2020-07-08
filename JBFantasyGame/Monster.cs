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
        protected String monsterType;                                  // think I will use type for class of character
        public String MonsterType
        {
            get { return monsterType; }
            set
            {
                monsterType = value;
            }
        }

    }                                        
}

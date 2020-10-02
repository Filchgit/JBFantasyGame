using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class Ability
    {
        protected string abil_Name;
        public string Abil_Name
        { get { return abil_Name; }
            set { abil_Name = value; }
        }
        protected int abil_Level;
        public int Abil_Level
        { get { return abil_Level; }
        set { abil_Level = value; }
        }
        protected string descrOfAbility;
        public string DescrOfAbility
        { get { return descrOfAbility; }
            set { descrOfAbility = value; }
        }
        protected bool abilIsActive;
        public bool AbilIsActive
        { get { return abilIsActive; } 
        set { abilIsActive = value; }
        }
        protected double manaCost;
        public double ManaCost
        { get { return manaCost; } 
          set { manaCost = value; }
        }
        protected double manaRegenCost;
        public double ManaRegenCost
        { get { return manaRegenCost; }
        set { manaRegenCost = value; }
        }
        protected int hpCost;
        public int HpCost
        { get { return hpCost; }
          set { hpCost = value; }
        }
        protected int durationMax;
        public int DurationMax
        { get { return durationMax; }
          set { durationMax = value; }
        }
        protected int durationElapsed;
        public int DurationElapsed
        { get { return durationElapsed; }
          set { durationElapsed = value; }
        }
        protected int noOfEntitiesAffectedMax;
        public int NoOfEntitiesAffectedMax
        { get { return noOfEntitiesAffectedMax; }
          set { noOfEntitiesAffectedMax = value; }
        }
        protected string targetEntitiesAffected;
        public string TargetEntitiesAffected
        { get { return targetEntitiesAffected; }
        set { targetEntitiesAffected = value; }
        }
        protected string hpEffect;
        public string HpEffect                          //string as it will be a dieRoll range
        {
            get { return hpEffect; }
            set { hpEffect = value; }
        }
        protected double targetRange;
        public double TargetRange
        { get { return targetRange; } 
          set { targetRange = value; }
        }
        protected string saveType;
        public string SaveType
        { get { return saveType; }
        set { saveType = value; }
        }

    }

}

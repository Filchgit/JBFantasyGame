using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace JBFantasyGame
{
    public class Party : List<Fant_Entity>
    {
        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Party() => name = "The Party has no name.";
      
        public Party  Partynew( Party myParty)
        {
            bool addchar = true;
           
            while (addchar)
            {
                WriteLine("Would you like to add add another member to the party? y/n");
                char c = ReadKey().KeyChar;
                if (c == 'y')
                {
                    WriteLine("\bWhat would you like this character to be named? :");
                    string newName = ReadLine();
                    Character newguy = new Character();
                    newguy.Name = newName;
                    myParty.Add(newguy);
                }
                else if (c == 'n')
                {
                    addchar = false;
                    WriteLine("\b \n");
                    break;
                }
                else { Write("\b"); }

            }
            return (Party)myParty;
        }
    }
 }



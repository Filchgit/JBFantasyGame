using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for DMUpdateChar.xaml
    /// </summary>
    public partial class DMUpdateChar : Window
    {
        public Character characterUpdated;
        public DMUpdateChar()
        {

            InitializeComponent();
            characterUpdated = (Character)MainWindow.characterSelected;
            DMUpdateCharName.Text = characterUpdated.Name;
            DMUpdateCharType.Text = characterUpdated.CharType;
            DMUpdateCharExp.Text = characterUpdated.Exp.ToString();
            DMUpdateCharStr.Text = characterUpdated.Str.ToString();
            DMUpdateCharInte.Text = characterUpdated.Inte.ToString();
            DMUpdateCharWis.Text = characterUpdated.Wis.ToString();
            DMUpdateCharCon.Text = characterUpdated.Con.ToString();
            DMUpdateCharDex.Text = characterUpdated.Dex.ToString();
            DMUpdateCharChr.Text = characterUpdated.Chr.ToString();
            DMUpdateCharPartyName.Text = characterUpdated.PartyName.ToString();
            DMUpdateCharMaxHp.Text = characterUpdated.MaxHp.ToString();                  // this gets overidden on recalculate

        }

        private void UpdateChar_Click(object sender, RoutedEventArgs e)
        {
            characterUpdated.Name = DMUpdateCharName.Text;
            characterUpdated.CharType = DMUpdateCharType.Text;
            characterUpdated.Exp = Int32.Parse(DMUpdateCharExp.Text);
            characterUpdated.Str = Int32.Parse(DMUpdateCharStr.Text);
            characterUpdated.Inte = Int32.Parse(DMUpdateCharInte.Text);
            characterUpdated.Wis = Int32.Parse(DMUpdateCharWis.Text);
            characterUpdated.Con = Int32.Parse(DMUpdateCharCon.Text);
            characterUpdated.Dex = Int32.Parse(DMUpdateCharDex.Text);
            characterUpdated.Chr = Int32.Parse(DMUpdateCharChr.Text);
            string oldpartyname = characterUpdated.PartyName;
            string thisParty = DMUpdateCharPartyName.Text;

            characterUpdated.PartyName = DMUpdateCharPartyName.Text;

            string chartype = characterUpdated.CharType;
            if (characterUpdated.CharType is "Fighter")
            {
                Fighter.FighterInitialize(characterUpdated);
            }
            else if (characterUpdated.CharType is "Cleric")
            {
                Cleric.ClericInitialize(characterUpdated);
            }
            else if (characterUpdated.CharType is "Mage")
            {
                Mage.MageInitialize(characterUpdated);
            }

            else if (characterUpdated.CharType is "Rogue")
            {
                Rogue.RogueInitialize(characterUpdated);
            }
            if (oldpartyname != thisParty)
            {
                foreach (Party checkParty in MainWindow.Parties)     
                {
                    if (checkParty.Name == oldpartyname)
                        checkParty.Remove(characterUpdated);
                }
                foreach (Party checkParty in MainWindow.Parties)         // adds and removes correctly from parties that are there at least.
                {
                    if (checkParty.Name == thisParty)
                    {
                        checkParty.Add(characterUpdated);
                    }
                }
                // MainWindow.characterSelected = characterUpdated;
                Close();
            }
            Close();
        }
    }
}
       

        
   
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowCharWin : Window
    {
       
        public ShowCharWin(Character thischaracter)
        {
            InitializeComponent();
   
            CharacterDetails = thischaracter;
            DataContext = CharacterDetails;

        
            //TextBlock ShowCharname = new TextBlock() ;
            ShowCharname.Text = thischaracter.Name;
            // HP.Text = thischaracter.Hp.ToString();
            
            ShowCharStr.Text = thischaracter.Str.ToString();  
            ShowCharInt.Text = thischaracter.Inte.ToString();
            ShowCharWis.Text = thischaracter.Wis.ToString();
            ShowCharDex.Text = thischaracter.Dex.ToString();
            ShowCharCon.Text = thischaracter.Con.ToString();
            ShowCharChr.Text = thischaracter.Chr.ToString();
            ShowCharLvl.Text = thischaracter.Lvl.ToString();
            ShowCharExp.Text = thischaracter.Exp.ToString();
            ShowGroup.Text = thischaracter.PartyName.ToString();

            CharInv.ItemsSource = thischaracter.Inventory ;
            CharInv.DisplayMemberPath = "Name";
        }
        public Character CharacterDetails
        {
            get { return (Character)GetValue(CharacterDetailsProperty); }
            set { SetValue(CharacterDetailsProperty, value); }
        }
        public static readonly DependencyProperty CharacterDetailsProperty = DependencyProperty.Register("CharacterDetails",
                               typeof(Character),
                               typeof(ShowCharWin),
                               new PropertyMetadata(null));

    }
}

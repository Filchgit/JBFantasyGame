using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowCharWin : Window
    {
        public ShowCharWin(Character thischaracter)
        {
            InitializeComponent();
            //TextBlock ShowCharname = new TextBlock() ;
            ShowCharname.Text = thischaracter.Name;
            HP.Text = thischaracter.Hp.ToString();
            ShowCharStr.Text = thischaracter.Str.ToString();  
            ShowCharInt.Text = thischaracter.Inte.ToString();
            ShowCharWis.Text = thischaracter.Wis.ToString();
            ShowCharDex.Text = thischaracter.Dex.ToString();
            ShowCharCon.Text = thischaracter.Con.ToString();
            ShowCharChr.Text = thischaracter.Chr.ToString();


        }
    }
}

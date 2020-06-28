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
    /// Interaction logic for GlobalItemAdd.xaml
    /// </summary>
    public partial class GlobalItemAdd : Window
    {
        public GlobalItemAdd()
        {
            InitializeComponent();
        }
        // not positive we need all these blank textInput fields I may leave them so we can error check fields I think
        // thischaracter.Name = Nameinput.Text; 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PhysObj anotherPhysObj = new PhysObj();
            anotherPhysObj.Name = GlobalItemNameInput.Text;
            anotherPhysObj.ObjType = GlobalItemObjTypeInput.Text;
            anotherPhysObj.Damage =  GlobalItemDamageInput.Text;
            anotherPhysObj.ACEffect = GlobalItemACEffectInput.Text;
           
            bool ISequip;
            string str = GlobalItemIsEquippedInput.Text;    // might make this a choice between two box.
            ISequip = str == "True";
            MainWindow.GlobalItems.Add(anotherPhysObj);     // doesn't actuallly update item list on previous page, ok as this really quick and dirty at this stage 
            
        }
        private void GlobalItemNameInput_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void GlobalItemObjTypeInput_TextInput(object sender, TextCompositionEventArgs e)
        {

        }
        private void GlobalItemDamageInput_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void GlobalItemIsEquippedInput_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void GlobalItemACEffectInput_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

       
    }
}

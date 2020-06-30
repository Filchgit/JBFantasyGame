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
using System.Collections.ObjectModel;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowCharWin : Window
    {
        private Character showcharacter;                              //seeing if I can use an object model - data grid  
        private DispatcherTimer dispatcherTimer = null;

        public ShowCharWin(Character thischaracter)
        {
            InitializeComponent();
            showcharacter = thischaracter;
                                                                        // currently updating character sheets on a timer might see if I can do this from a global event later 
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(3.0);
            dispatcherTimer.Tick += OnTimerTick;
            dispatcherTimer.Start();
        }
        private void OnTimerTick(object sender, EventArgs e)                 // there was a tip to make sure that lengthy operations
        {                                                                    // are not done on tick event as it runs on UI thread, and 
            ShowCharClass.Text = showcharacter.CharType.ToString();          // as such may block UI from responding. Don't think this is lengthy.
            ShowCharname.Text = showcharacter.Name.ToString();
            ShowCharHP.Text = showcharacter.Hp.ToString();
            ShowCharStr.Text = showcharacter.Str.ToString();  
            ShowCharInt.Text = showcharacter.Inte.ToString();
            ShowCharWis.Text = showcharacter.Wis.ToString();
            ShowCharDex.Text = showcharacter.Dex.ToString();
            ShowCharCon.Text = showcharacter.Con.ToString();
            ShowCharChr.Text = showcharacter.Chr.ToString();
            ShowCharLvl.Text = showcharacter.Lvl.ToString();
            ShowCharExp.Text = showcharacter.Exp.ToString();
            ShowGroup.Text = showcharacter.PartyName.ToString();
            ShowCharHiton20.Text = showcharacter.HitOn20.ToString();
            ShowCharAC.Text = showcharacter.AC.ToString();

            PhysObjects = new ObservableCollection<PhysObj>               //all this bit is databinding my inventory grid to 
            { };                                                          // the PhysObjects ObservableCollection
             foreach (PhysObj physthing in showcharacter.Inventory)       //  can't make binding to way to source ; at least
             {                                                            //I can't work out how to atm; so updating time atm.
              if (PhysObjects.Contains(physthing))
            { continue; }
            else 
                 { PhysObjects.Add(physthing); }
            }
            PersonalInventory.ItemsSource = PhysObjects;
        }
        public ObservableCollection<PhysObj> PhysObjects
        {
            get { return (ObservableCollection<PhysObj>)GetValue(PhysObjectsProperty); }
            set { SetValue(PhysObjectsProperty, value); }
        }
        public static readonly DependencyProperty PhysObjectsProperty =
            DependencyProperty.Register("PhysObjects",
                     typeof(ObservableCollection<PhysObj>),
                     typeof(JBFantasyGame.ShowCharWin),
                     new PropertyMetadata(null));
 
        private void PersonalInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           PersonalInventory.SelectionChanged += PersonalInventory_SelectionChanged;       
        }

        private void Delete1st_Click(object sender, RoutedEventArgs e)
        {
            showcharacter.Inventory.RemoveAt (0);
        }

    }
}


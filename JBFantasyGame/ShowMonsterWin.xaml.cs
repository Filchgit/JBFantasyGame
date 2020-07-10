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
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for ShowMonsterWin.xaml
    /// </summary>
    public partial class ShowMonsterWin : Window
    {
        private Monster showmonster;
        private DispatcherTimer dispatcherTimer = null;
        public ShowMonsterWin(Monster thismonster)
        {
            InitializeComponent();
            showmonster = thismonster;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5.0);
            dispatcherTimer.Tick += OnTimerTick;
            dispatcherTimer.Start();
        }
        private void OnTimerTick(object sender, EventArgs e)                 // there was a tip to make sure that lengthy operations    
        { UpdateShowMonsterWin(); }
        private void UpdateShowMonsterWin()
        {
            ShowMonsterType.Text = showmonster.MonsterType.ToString(); 
            TBPartyName.Text = showmonster.PartyName;

            ShowMonsterName.Text = showmonster.Name;
            TBLevel.Text = showmonster.Lvl.ToString() ;
            TBAC.Text = showmonster.AC.ToString();
            TBHitDie.Text = showmonster.AC.ToString();
            TBMaxHP.Text = showmonster.MaxHp.ToString();
            TBHP.Text = showmonster.Hp.ToString();
            TBHitOn20.Text = showmonster.HitOn20.ToString();
            TBInitMod.Text = showmonster.InitMod.ToString(); 
            TBNoOfAtt.Text = showmonster.NoOfAtt.ToString();  




            PhysObjects = new ObservableCollection<PhysObj>               //all this bit is databinding my inventory grid to 
            { };                                                          // the PhysObjects ObservableCollection
            foreach (PhysObj physthing in showmonster.Inventory)       //  can't make binding to way to source ; at least
            {                                                            //I can't work out how to atm; so updating time atm.
                PhysObjects.Add(physthing);
            }

            PersonalInventory.ItemsSource = PhysObjects;
            //   showmonster.AC = showmonster.ACRecalc(showmonster);    not sure if I will do ACRecalc for monsters yet 
            //ShowCharAC.Text = showcharacter.AC.ToString();

            MeleeTargets = new ObservableCollection<Target>
            { };
            foreach (Target _aTarget in showmonster.MeleeTargets)
            {
                MeleeTargets.Add(_aTarget);
            }
            ViableMeleeTargets.ItemsSource = MeleeTargets;
        }
        public ObservableCollection<PhysObj> PhysObjects
        {
            get { return (ObservableCollection<PhysObj>)GetValue(PhysObjectsProperty); }
            set { SetValue(PhysObjectsProperty, value); }
        }
        public static readonly DependencyProperty PhysObjectsProperty =
            DependencyProperty.Register("PhysObjects",
                     typeof(ObservableCollection<PhysObj>),
                     typeof(JBFantasyGame.ShowMonsterWin),
                     new PropertyMetadata(null));

        public ObservableCollection<Target> MeleeTargets
        {
            get { return (ObservableCollection<Target>)GetValue(MeleeTargetsProperty); }
            set { SetValue(MeleeTargetsProperty, value); }
        }
        public static readonly DependencyProperty MeleeTargetsProperty =
            DependencyProperty.Register("MeleeTargets",
            typeof(ObservableCollection<Target>),
            typeof(JBFantasyGame.ShowMonsterWin),
            new PropertyMetadata(null));
        private void ViableMeleeTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViableMeleeTargets.SelectionChanged += ViableMeleeTargets_SelectionChanged;
        }
        private void MeleeThisEnt_Click(object sender, RoutedEventArgs e)
        {
            Target thisTargetAttack = (Target)ViableMeleeTargets.SelectedItem;
            showmonster.MyTargetParty = thisTargetAttack.PartyName;
            showmonster.MyTargetEnt = thisTargetAttack.Name;
        }
        private void Delete1st_Click(object sender, RoutedEventArgs e)
        {
            PhysObj removethis = (PhysObj)PersonalInventory.SelectedItem;
            showmonster.Inventory.Remove(removethis);
            UpdateShowMonsterWin();
        }
        private void EquipThisButt_Click(object sender, RoutedEventArgs e)          // Obviously can put a lot of type checking in here and then conditions
        {
            PhysObj equipthis = (PhysObj)PersonalInventory.SelectedItem;
            if (equipthis.IsEquipped == true)
            { equipthis.IsEquipped = false; }
            else { equipthis.IsEquipped = true; }
            UpdateShowMonsterWin();
        }
        private void PersonalInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PersonalInventory.SelectionChanged += PersonalInventory_SelectionChanged;
        }

        private void StopTimerUpdate_Click(object sender, RoutedEventArgs e)
        {dispatcherTimer.Stop();}

        private void UpdateRestartTimer_Click(object sender, RoutedEventArgs e)
        {
            showmonster.Name = ShowMonsterName.Text;

            showmonster.MonsterType = ShowMonsterType.Text;    //for now will change to dropdownbox
                                                               // PartyName not to change from here TBPartyName.Text = showmonster.PartyName;

            showmonster.Name = ShowMonsterName.Text;
            showmonster.Lvl= Int32.Parse(TBLevel.Text);
            showmonster.AC = Int32.Parse(TBAC.Text);
            showmonster.HitDie = TBHitDie.Text;
            showmonster.MaxHp = Int32.Parse(TBMaxHP.Text); 
            TBHP.Text = showmonster.Hp.ToString();
            TBHitOn20.Text = showmonster.HitOn20.ToString();
            TBInitMod.Text = showmonster.InitMod.ToString();
            TBNoOfAtt.Text = showmonster.NoOfAtt.ToString();



            dispatcherTimer.Start();}
    }
}

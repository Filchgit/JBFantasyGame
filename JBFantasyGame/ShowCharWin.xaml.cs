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
        private string nextRound = "";

        public ShowCharWin(Character thischaracter)
        {
            InitializeComponent();

            showcharacter = thischaracter;                     //}
            UpdateShowCharWin();                                      // currently updating character sheets on a timer might see if I can do this from a global event later 
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5.0);
            dispatcherTimer.Tick += OnTimerTick;
            dispatcherTimer.Start();
        }
        private void OnTimerTick(object sender, EventArgs e)                 // there was a tip to make sure that lengthy operations
        { UpdateShowCharWin(); }
        public void UpdateShowCharWin()
        {
                                                                             // are not done on tick event as it runs on UI thread, and 
            ShowCharClass.Text = showcharacter.CharType.ToString();          // as such may block UI from responding. Don't think this is lengthy.
            ShowCharname.Text = showcharacter.Name.ToString();
            ShowCharHP.Text = showcharacter.Hp.ToString();
            ShowCharMaxHP.Text = showcharacter.MaxHp.ToString();
            ShowCharStr.Text = showcharacter.Str.ToString();
            ShowCharInt.Text = showcharacter.Inte.ToString();
            ShowCharWis.Text = showcharacter.Wis.ToString();
            ShowCharDex.Text = showcharacter.Dex.ToString();
            ShowCharCon.Text = showcharacter.Con.ToString();
            ShowCharChr.Text = showcharacter.Chr.ToString();
            ShowCharMaxMana.Text = showcharacter.MaxMana.ToString();
            ShowCharMaxManaRegen.Text = showcharacter.MaxManaRegen.ToString();
            ShowCharLvl.Text = showcharacter.Lvl.ToString();
            ShowCharExp.Text = showcharacter.Exp.ToString();
            ShowGroup.Text = showcharacter.PartyName.ToString();
            ShowCharHiton20.Text = showcharacter.HitOn20.ToString();
            ShowCharNextRound.Text = nextRound;


            PhysObjects = new ObservableCollection<PhysObj>               //all this bit is databinding my inventory grid to 
            { };                                                          // the PhysObjects ObservableCollection
            foreach (PhysObj physthing in showcharacter.Inventory)       //  can't make binding to way to source ; at least
            {                                                            //I can't work out how to atm; so updating time atm.
                PhysObjects.Add(physthing);
            }

            PersonalInventory.ItemsSource = PhysObjects;
            Abilities = new ObservableCollection<Ability>
            { };

            foreach (Ability thisAbility in showcharacter.Abilities)
            {
                Abilities.Add(thisAbility);
            }

            showcharacter.AC = showcharacter.ACRecalc(showcharacter);
            ShowCharAC.Text = showcharacter.AC.ToString();
            SpecialActions.ItemsSource = Abilities;

            MeleeTargets = new ObservableCollection<Target>
            { };
            foreach (Target _aTarget in showcharacter.MeleeTargets)
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
                     typeof(JBFantasyGame.ShowCharWin),
                     new PropertyMetadata(null));

        public ObservableCollection<Ability> Abilities
        {
            get { return (ObservableCollection<Ability>)GetValue(AbilitiesProperty); }
            set { SetValue(AbilitiesProperty, value); }
        }
        public static readonly DependencyProperty AbilitiesProperty =
            DependencyProperty.Register("Abilities",
                typeof(ObservableCollection<Ability>),
                typeof(JBFantasyGame.ShowCharWin),
                new PropertyMetadata(null));
        private void PersonalInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PersonalInventory.SelectionChanged += PersonalInventory_SelectionChanged;
        }
        public ObservableCollection<Target> MeleeTargets
        {
            get { return (ObservableCollection<Target>)GetValue(MeleeTargetsProperty); }
            set { SetValue(MeleeTargetsProperty, value); }
        }
        public static readonly DependencyProperty MeleeTargetsProperty =
            DependencyProperty.Register("MeleeTargets",
            typeof(ObservableCollection<Target>),
            typeof(JBFantasyGame.ShowCharWin),
            new PropertyMetadata(null));
        private void SpecialActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpecialActions.SelectionChanged += SpecialActions_SelectionChanged;
        }




        private void ViableMeleeTargets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViableMeleeTargets.SelectionChanged += ViableMeleeTargets_SelectionChanged;
        }
        private void Delete1st_Click(object sender, RoutedEventArgs e)
        {
            PhysObj removethis = (PhysObj)PersonalInventory.SelectedItem;
            showcharacter.Inventory.Remove(removethis);
            UpdateShowCharWin();
        }

        private void EquipThisButt_Click(object sender, RoutedEventArgs e)          // Obviously can put a lot of type checking in here and then conditions
        {
            PhysObj equipthis = (PhysObj)PersonalInventory.SelectedItem;
            if (equipthis.IsEquipped == true)
            { equipthis.IsEquipped = false; }
            else { equipthis.IsEquipped = true; }
            UpdateShowCharWin();
        }

        private void MeleeThisEnt_Click(object sender, RoutedEventArgs e)
        {
            nextRound = "";
            foreach (Ability nullAbility in showcharacter.Abilities)                     // as you can only attack or use Special ability
            { nullAbility.AbilIsActive = false; }
            Target thisTargetAttack = (Target)ViableMeleeTargets.SelectedItem;
            showcharacter.MyTargetParty = thisTargetAttack.PartyName;
            showcharacter.MyTargetEnt = thisTargetAttack.Name;
            nextRound = $"{showcharacter.Name} plans to attack {thisTargetAttack.Name} next round.";      //can add detail later as to equipped weapons etc 
        }

        private void UseAbility_Click(object sender, RoutedEventArgs e)
        {
            showcharacter.MyTargetParty = null;
            showcharacter.MyTargetEnt = null;

            foreach (Ability nullAbility in showcharacter.Abilities)            // quick thing to null abilities, for changing mind
            { nullAbility.AbilIsActive = false; }
            Ability useThisAbility = (Ability)SpecialActions.SelectedItem;
            useThisAbility.AbilIsActive = true;

            List<Target> Targets = new List<Target>();
            int checkNoOfItems = ViableMeleeTargets.SelectedItems.Count;
            string targetList = "";

            for (int i = 0; i < checkNoOfItems; i++)
            {
                if (checkNoOfItems > i && useThisAbility.NoOfEntitiesAffectedMax >= i)    
                {
                    Targets.Add((Target)ViableMeleeTargets.SelectedItems[i]);
                    targetList += Targets[i].Name + "|" + Targets[i].PartyName + "|";
                }
            }
            string listOfTargets = "";
            if (checkNoOfItems == 1 || useThisAbility.NoOfEntitiesAffectedMax ==1)
            { listOfTargets = $"{Targets[0].Name}"; }
            if (checkNoOfItems >= 2 && useThisAbility.NoOfEntitiesAffectedMax >=2)
            { listOfTargets = $"{Targets[0].Name} and {Targets[1].Name}"; }
            if (checkNoOfItems > 2 && useThisAbility.NoOfEntitiesAffectedMax >= 3)
            {
                for (int j = 2; j < checkNoOfItems && j < useThisAbility.NoOfEntitiesAffectedMax; j++)
                { listOfTargets = $" {Targets[j].Name }, {listOfTargets}"; }
            }
            useThisAbility.TargetEntitiesAffected = targetList;

            if (useThisAbility.Abil_Name == "MageThrow")
                {
                    nextRound = $"{showcharacter.Name} intends to use Mage Throw next round versus {listOfTargets}";
                }
            if (useThisAbility.Abil_Name == "HealOverTime")
            {
                nextRound = $"{showcharacter.Name} intends to Heal over time {listOfTargets}";
                //   if (checkNoOfItems == 2)
                //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name} and {Targets[1].Name}"; }
                //   if (checkNoOfItems == 3)
                //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name}, {Targets[1].Name} and {Targets[2].Name}"; }

                //   useThisAbility.TargetEntitiesAffected = targetList;
            }

         UpdateShowCharWin();
                
        }
    }
}



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
        private string nextRound = "";
        private Monster showmonster;
        private DispatcherTimer dispatcherTimer = null;
        public ShowMonsterWin(Monster thismonster)
        {
            InitializeComponent();
            showmonster = thismonster;
            
            UpdateShowMonsterWin();
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
            TBHitDie.Text = showmonster.HitDie.ToString();
            TBMaxHP.Text = showmonster.MaxHp.ToString();
            TBHP.Text = showmonster.Hp.ToString();
            TBHitOn20.Text = showmonster.HitOn20.ToString();
            TBInitMod.Text = showmonster.InitMod.ToString(); 
            TBNoOfAtt.Text = showmonster.NoOfAtt.ToString();
            AttDam1.Text = showmonster.DamPerAtt1;
            AttDam2.Text = showmonster.DamPerAtt2;
            AttDam3.Text = showmonster.DamPerAtt3;
            MaxMana.Text = showmonster.MaxMana.ToString();
            CurrMana.Text = showmonster.CurrentMana.ToString();
            MaxManaRegen.Text = showmonster.MaxManaRegen.ToString();
            ManaRegen.Text = showmonster.ManaRegen.ToString();
            XPOnDefeat.Text = showmonster.XPOnDefeat.ToString();
            DefeatXPMult.Text = showmonster.DefeatMult.ToString();

            ShowMonstNextRound.Text = nextRound;

            SpecialActions.ItemsSource = Abilities;
            Abilities = new ObservableCollection<Ability>
            { };
            foreach (Ability thisAbility in showmonster.Abilities)
            {
                Abilities.Add(thisAbility);
            }
           

            PhysObjects = new ObservableCollection<PhysObj >               //all this bit is databinding my inventory grid to 
            { };                                                          // the PhysObjects ObservableCollection
            foreach (PhysObj  physthing in showmonster.Inventory)       //  can't make binding to way to source ; at least
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
        public ObservableCollection<PhysObj > PhysObjects
        {
            get { return (ObservableCollection<PhysObj >)GetValue(PhysObjectsProperty); }
            set { SetValue(PhysObjectsProperty, value); }
        }
        public static readonly DependencyProperty PhysObjectsProperty =
            DependencyProperty.Register("PhysObjects",
                     typeof(ObservableCollection<PhysObj >),
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
            nextRound = "";
            foreach (Ability nullAbility in showmonster.Abilities)                     // as you can only attack or use Special ability
            { nullAbility.AbilIsActive = false; }
            Target thisTargetAttack = (Target)ViableMeleeTargets.SelectedItem;
            showmonster.MyTargetParty = thisTargetAttack.PartyName;
            showmonster.MyTargetEnt = thisTargetAttack.Name;
            nextRound = $"{showmonster.Name} plans to attack {thisTargetAttack.Name} next round.";      //can add detail later as to equipped weapons etc 
        }
        private void Delete1st_Click(object sender, RoutedEventArgs e)
        {
            PhysObj  removethis = (PhysObj )PersonalInventory.SelectedItem;
            showmonster.Inventory.Remove(removethis);
            UpdateShowMonsterWin();
        }
       
        private void EquipThisButt_Click(object sender, RoutedEventArgs e)          // Obviously can put a lot of type checking in here and then conditions
        {
            PhysObj equipthis = (PhysObj )PersonalInventory.SelectedItem;
            if (equipthis.IsEquipped == true)
            { equipthis.IsEquipped = false; }
            else { equipthis.IsEquipped = true; }
            UpdateShowMonsterWin();
        }
        private void PersonalInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PersonalInventory.SelectionChanged += PersonalInventory_SelectionChanged;
        }
        public ObservableCollection<Ability> Abilities
        {
            get { return (ObservableCollection<Ability>)GetValue(AbilitiesProperty); }
            set { SetValue(AbilitiesProperty, value); }
        }
        public static readonly DependencyProperty AbilitiesProperty =
            DependencyProperty.Register("Abilities",
                typeof(ObservableCollection<Ability>),
                typeof(JBFantasyGame.ShowMonsterWin),
                new PropertyMetadata(null));

        private void SpecialActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SpecialActions.SelectionChanged += SpecialActions_SelectionChanged;
        }
        private void StopTimerUpdate_Click(object sender, RoutedEventArgs e)
        {dispatcherTimer.Stop();}

        private void UpdateRestartTimer_Click(object sender, RoutedEventArgs e)
        {
            showmonster.Name = ShowMonsterName.Text;
            showmonster.Lvl = Int32.Parse(TBLevel.Text);

            if (showmonster.MonsterType != ShowMonsterType.Text)   //for now will change to dropdownbox
            {
                if (ShowMonsterType.Text == "Troll")
                { Troll.TrollInitialize(showmonster); }

                TBLevel.Text = showmonster.Lvl.ToString();             // need to put to textboxes etc the values that have been changed, otherwise they will change back
                showmonster.MonsterType = ShowMonsterType.Text;
                TBAC.Text = showmonster.AC.ToString();
                TBHitDie.Text = showmonster.HitDie.ToString();
                TBMaxHP.Text = showmonster.MaxHp.ToString();
                TBHP.Text = showmonster.Hp.ToString();
                TBHitOn20.Text = showmonster.HitOn20.ToString();
                TBInitMod.Text = showmonster.InitMod.ToString();
                TBNoOfAtt.Text = showmonster.NoOfAtt.ToString();
                AttDam1.Text = showmonster.DamPerAtt1;
                AttDam2.Text = showmonster.DamPerAtt2;
                AttDam3.Text = showmonster.DamPerAtt3;
                MaxMana.Text = showmonster.MaxMana.ToString();
                CurrMana.Text = showmonster.CurrentMana.ToString();
                MaxManaRegen.Text = showmonster.MaxManaRegen.ToString();
                ManaRegen.Text = showmonster.ManaRegen.ToString();
                XPOnDefeat.Text = showmonster.XPOnDefeat.ToString();
                DefeatXPMult.Text = showmonster.DefeatMult.ToString();

            }                                                       // PartyName not to change from here TBPartyName.Text = showmonster.PartyName;

            showmonster.Name = ShowMonsterName.Text;
            showmonster.Lvl= Int32.Parse(TBLevel.Text);
            showmonster.AC = Int32.Parse(TBAC.Text);
            showmonster.HitDie = TBHitDie.Text;
            showmonster.MaxHp = Int32.Parse(TBMaxHP.Text);
            showmonster.Hp = Int32.Parse(TBHP.Text);
            showmonster.HitOn20 = Int32.Parse(TBHitOn20.Text);
            showmonster.InitMod = Int32.Parse(TBInitMod.Text);
            showmonster.NoOfAtt = Int32.Parse(TBNoOfAtt.Text);
            showmonster.DamPerAtt1 = AttDam1.Text;
            showmonster.DamPerAtt2 = AttDam2.Text;
            showmonster.DamPerAtt3 = AttDam3.Text;
            showmonster.MaxMana = double.Parse(MaxMana.Text);
            showmonster.CurrentMana = double.Parse(CurrMana.Text);
            showmonster.MaxManaRegen = double.Parse(MaxManaRegen.Text);
            showmonster.ManaRegen = double.Parse(ManaRegen.Text);
            showmonster.XPOnDefeat = double.Parse(XPOnDefeat.Text);
            showmonster.DefeatMult = double.Parse(DefeatXPMult.Text);



            ShowMonstNextRound.Text = nextRound;

            dispatcherTimer.Start();}

        private void UseAbility_Click(object sender, RoutedEventArgs e)
        {
            
                showmonster.MyTargetParty = null;
                showmonster.MyTargetEnt = null;

                foreach (Ability nullAbility in showmonster.Abilities)            // quick thing to null abilities, for changing mind
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
                if (checkNoOfItems == 1 || useThisAbility.NoOfEntitiesAffectedMax == 1)
                { listOfTargets = $"{Targets[0].Name}"; }
                if (checkNoOfItems >= 2 && useThisAbility.NoOfEntitiesAffectedMax >= 2)
                { listOfTargets = $"{Targets[0].Name} and {Targets[1].Name}"; }
                if (checkNoOfItems > 2 && useThisAbility.NoOfEntitiesAffectedMax >= 3)
                {
                    for (int j = 2; j < checkNoOfItems && j < useThisAbility.NoOfEntitiesAffectedMax; j++)
                    { listOfTargets = $" {Targets[j].Name }, {listOfTargets}"; }
                }
                useThisAbility.TargetEntitiesAffected = targetList;

                if (useThisAbility.Abil_Name == "MageThrow")
                {
                    nextRound = $"{showmonster.Name} intends to use Mage Throw next round versus {listOfTargets}";
                }
                if (useThisAbility.Abil_Name == "HealOverTime")
                {
                    nextRound = $"{showmonster.Name} intends to Heal over time {listOfTargets}";
                    //   if (checkNoOfItems == 2)
                    //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name} and {Targets[1].Name}"; }
                    //   if (checkNoOfItems == 3)
                    //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name}, {Targets[1].Name} and {Targets[2].Name}"; }

                    //   useThisAbility.TargetEntitiesAffected = targetList;
                }

                UpdateShowMonsterWin();

            
        }

        private void UseMonstAbility_Click(object sender, RoutedEventArgs e)
        {
            
                showmonster.MyTargetParty = null;
                showmonster.MyTargetEnt = null;

                foreach (Ability nullAbility in showmonster.Abilities)            // quick thing to null abilities, for changing mind
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
                if (checkNoOfItems == 1 || useThisAbility.NoOfEntitiesAffectedMax == 1)
                { listOfTargets = $"{Targets[0].Name}"; }
                if (checkNoOfItems >= 2 && useThisAbility.NoOfEntitiesAffectedMax >= 2)
                { listOfTargets = $"{Targets[0].Name} and {Targets[1].Name}"; }
                if (checkNoOfItems > 2 && useThisAbility.NoOfEntitiesAffectedMax >= 3)
                {
                    for (int j = 2; j < checkNoOfItems && j < useThisAbility.NoOfEntitiesAffectedMax; j++)
                    { listOfTargets = $" {Targets[j].Name }, {listOfTargets}"; }
                }
                useThisAbility.TargetEntitiesAffected = targetList;

                if (useThisAbility.Abil_Name == "MageThrow")
                {
                    nextRound = $"{showmonster.Name} intends to use Mage Throw next round versus {listOfTargets}";
                }
                if (useThisAbility.Abil_Name == "HealOverTime")
                {
                    nextRound = $"{showmonster.Name} intends to Heal over time {listOfTargets}";
                    //   if (checkNoOfItems == 2)
                    //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name} and {Targets[1].Name}"; }
                    //   if (checkNoOfItems == 3)
                    //   { nextRound = $"{showcharacter.Name} intends to Heal over time {Targets[0].Name}, {Targets[1].Name} and {Targets[2].Name}"; }

                    //   useThisAbility.TargetEntitiesAffected = targetList;
                }

                UpdateShowMonsterWin();

            
        }
    }
}

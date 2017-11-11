namespace Troll_Warlord
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Threading;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    using SharpDX;

    using log4net;
    using PlaySharp.Toolkit.Helper.Annotations;
    using PlaySharp.Toolkit.Logging;

    using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;
    using Ensage.SDK.Abilities.npc_dota_hero_troll_warlord;

    [PublicAPI]
    public class TrollWarlord : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private Hero EnemyHero { get; set; }

        private ITargetSelectorManager TargetSelector { get; }

        public TrollWarlordConfiguration Config { get; }

        public TrollWarlord(Key key, TrollWarlordConfiguration config, IServiceContext context) : base(context, key)
        {
            this.Config = config;
            this.context = context;
            this.TargetSelector = context.TargetSelector;
        }

        [ItemBinding]
        public item_medallion_of_courage Medalion1 { get; private set; }

        [ItemBinding]
        public item_diffusal_blade DiffBlade { get; private set; }

        [ItemBinding]
        public item_solar_crest Medallion2 { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding]
        public item_heavens_halberd Heavens_Halberd { get; private set; }

        [ItemBinding]
        public item_invis_sword Invis_Sword { get; private set; }

        [ItemBinding]
        public item_silver_edge Silver_Edge { get; private set; }

        [ItemBinding]
        public item_blink Blink { get; private set; }

        [ItemBinding]
        public item_mask_of_madness Mask_Of_Madness { get; private set; }

        [ItemBinding]
        public item_sheepstick SheepStick { get; private set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        public troll_warlord_berserkers_rage QAbi { get;  set; }
        public troll_warlord_whirling_axes_melee WAbi { get; set; }
        public troll_warlord_whirling_axes_ranged EAbi { get; set; }
        public troll_warlord_battle_trance RAbi { get; set; }


        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (Config.Target.Value.SelectedValue.Contains("Lock") && Context.TargetSelector.IsActive
                    && (!CanExecute || EnemyHero == null || !EnemyHero.IsValid || !EnemyHero.IsAlive))
                {
                    EnemyHero = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }
                else if (Config.Target.Value.SelectedValue.Contains("Default") && Context.TargetSelector.IsActive)
                {
                    EnemyHero = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }

                var Silenced = UnitExtensions.IsSilenced(base.Owner);
                var SliderData = this.Config.UseBlink.Item.GetValue<Slider>().Value;
                var IsInvis = Ensage.SDK.Extensions.UnitExtensions.IsInvisible(this.Owner);

                

                if (EnemyHero != null && !EnemyHero.IsInvulnerable())
                {
                    if (!Silenced && this.Config.AbilityToggler.Value.IsEnabled(this.QAbi.Ability.Name) && this.Owner.IsMelee && !IsInvis  && this.Owner.IsInAttackRange(EnemyHero, 350) && !this.Owner.IsInRange(EnemyHero, 150))
                    {
                        this.QAbi.Ability.ToggleAbility();
                        await Await.Delay(this.GetAbilityDelay(base.Owner, QAbi), token);
                    }

                    if (!Silenced && this.Config.AbilityToggler.Value.IsEnabled(this.QAbi.Ability.Name) && !IsInvis && this.Owner.IsRanged  && this.Owner.IsInRange(EnemyHero, 150))
                    {
                        this.QAbi.Ability.ToggleAbility();
                        await Await.Delay(this.GetAbilityDelay(base.Owner, QAbi), token);
                    }

                    if (!Silenced && !this.Owner.IsInAttackRange(EnemyHero) && !IsInvis && this.Owner.IsRanged)
                    {
                        this.QAbi.Ability.ToggleAbility();
                        await Await.Delay(this.GetAbilityDelay(base.Owner, QAbi), token);
                    }

                    if (this.Blink != null && this.Blink.CanBeCasted && Owner.Distance2D(EnemyHero) <= 1200 + SliderData && !IsInvis && !(this.Owner.Distance2D(EnemyHero) <= 400) && this.Config.ItemToggler2.Value.IsEnabled(this.Blink.Item.Name))
                    {
                        var l = (this.Owner.Distance2D(EnemyHero) - SliderData) / SliderData;
                        var posA = this.Owner.Position;
                        var posB = EnemyHero.Position;
                        var x = (posA.X + (l * posB.X)) / (1 + l);
                        var y = (posA.Y + (l * posB.Y)) / (1 + l);
                        var position = new Vector3((int)x, (int)y, posA.Z);

                        this.Blink.UseAbility(position);
                        await Await.Delay(Blink.GetCastDelay(position), token);
                    }


                    if (!UnitExtensions.IsMagicImmune(EnemyHero))
                    {
                        if (!Silenced && this.WAbi != null && this.WAbi.CanBeCasted && !IsInvis && this.WAbi.CanHit(EnemyHero) && this.Config.AbilityToggler.Value.IsEnabled(this.WAbi.Ability.Name))
                        {
                            this.WAbi.UseAbility();
                            await Await.Delay(WAbi.GetCastDelay(), token);
                        }

                        if (!Silenced && this.EAbi != null && this.EAbi.CanBeCasted && !IsInvis && this.EAbi.CanHit(EnemyHero) && this.Config.AbilityToggler.Value.IsEnabled(this.EAbi.Ability.Name))
                        {

                            this.EAbi.UseAbility(EnemyHero);
                            await Await.Delay(EAbi.GetCastDelay(), token);
                        }


                        if (!Silenced && this.RAbi != null && this.RAbi.CanBeCasted && !IsInvis && this.Owner.IsAttacking() && this.Config.AbilityToggler.Value.IsEnabled(this.RAbi.Ability.Name))
                        {
                            this.RAbi.UseAbility();
                            await Await.Delay(EAbi.GetCastDelay(), token);
                        }

                        if (this.BloodThorn != null && this.BloodThorn.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && !this.Owner.IsAttacking() && this.Config.ItemToggler.Value.IsEnabled(this.BloodThorn.Item.Name))
                        {
                            this.BloodThorn.UseAbility(EnemyHero);
                            await Await.Delay(BloodThorn.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Medalion1 != null && this.Medalion1.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && !this.Owner.IsAttacking() && this.Config.ItemToggler.Value.IsEnabled(this.Medalion1.Item.Name))
                        {
                            this.Medalion1.UseAbility(EnemyHero);
                            await Await.Delay(Medalion1.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Medallion2 != null && this.Medallion2.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && !this.Owner.IsAttacking() && this.Config.ItemToggler.Value.IsEnabled(this.Medallion2.Item.Name))
                        {
                            this.Medallion2.UseAbility(EnemyHero);
                            await Await.Delay(Medallion2.GetCastDelay(EnemyHero), token);
                        }

                        if (this.SheepStick != null && this.SheepStick.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) &&  this.Config.ItemToggler.Value.IsEnabled(this.SheepStick.Item.Name))
                        {
                            this.SheepStick.UseAbility(EnemyHero);
                            await Await.Delay(SheepStick.GetCastDelay(EnemyHero), token);
                        }

                        if (this.DiffBlade != null && this.DiffBlade.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && this.Config.ItemToggler.Value.IsEnabled(DiffBlade.Item.Name))
                        {
                            this.DiffBlade.UseAbility(EnemyHero);
                            await Await.Delay(DiffBlade.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Orchid != null && this.Orchid.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && this.Config.ItemToggler.Value.IsEnabled(Orchid.Item.Name))
                        {
                            this.Orchid.UseAbility(EnemyHero);
                            await Await.Delay(Orchid.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Mjollnir != null && this.Mjollnir.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && this.Config.ItemToggler2.Value.IsEnabled(Mjollnir.Item.Name))
                        {
                            this.Mjollnir.UseAbility(this.Owner);
                            await Await.Delay(Mjollnir.GetCastDelay(), token);
                        }

                        if (this.Heavens_Halberd != null && this.Heavens_Halberd.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && this.Config.ItemToggler2.Value.IsEnabled(Heavens_Halberd.Item.Name))
                        {
                            this.Heavens_Halberd.UseAbility(EnemyHero);
                            await Await.Delay(Heavens_Halberd.GetCastDelay(), token);
                        }

                        if (this.Invis_Sword != null && this.Invis_Sword.CanBeCasted && !IsInvis && !this.Owner.IsInRange(EnemyHero, 1000) && !this.Owner.IsAttacking() && this.Config.ItemToggler2.Value.IsEnabled(Invis_Sword.Item.Name))
                        {
                            this.Invis_Sword.UseAbility();
                            await Await.Delay(Invis_Sword.GetCastDelay(), token);
                        }

                        if (this.Silver_Edge != null && this.Silver_Edge.CanBeCasted && !IsInvis && !this.Owner.IsInRange(EnemyHero, 1000) && !this.Owner.IsAttacking() && this.Config.ItemToggler2.Value.IsEnabled(Silver_Edge.Item.Name))
                        {
                            this.Silver_Edge.UseAbility();
                            await Await.Delay(Silver_Edge.GetCastDelay(), token);
                        }
                    }

                    if (this.Mask_Of_Madness != null && this.Mask_Of_Madness.CanBeCasted && !IsInvis && this.Config.ItemToggler2.Value.IsEnabled(Mask_Of_Madness.Item.Name))
                    {
                        if (Config.MomUsage.Value.SelectedValue.Contains("Melee Form") && this.Owner.IsMelee && !this.EAbi.CanBeCasted && !this.RAbi.CanBeCasted)
                        {
                            this.Mask_Of_Madness.UseAbility();
                            await Await.Delay(Mask_Of_Madness.GetCastDelay(), token);
                        }
                        else if (Config.MomUsage.Value.SelectedValue.Contains("Ranged Form") && this.Owner.IsRanged && !this.WAbi.CanBeCasted && !this.RAbi.CanBeCasted)
                        {
                            this.Mask_Of_Madness.UseAbility();
                            await Await.Delay(Mask_Of_Madness.GetCastDelay(), token);
                        }

                    }

                    if (EnemyHero != null && (EnemyHero.IsInvulnerable() || UnitExtensions.IsAttackImmune(EnemyHero)))
                    {
                        Orbwalker.Move(Game.MousePosition);
                    }
                    else if (EnemyHero != null)
                    {
                        Orbwalker.OrbwalkTo(EnemyHero);
                    }
                }
                else
                {
                    Orbwalker.Move(Game.MousePosition);
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private int GetAbilityDelay(Unit unit, Ability ability)
        {
            return (int)(((ability.FindCastPoint() + this.Owner.GetTurnTime(unit)) * 1000.0) + Game.Ping) + 50;
        }

        private int GetAbilityDelay(Vector3 pos, Ability ability)
        {
            return (int)(((ability.FindCastPoint() + this.Owner.GetTurnTime(pos)) * 1000.0) + Game.Ping) + 50;
        }

        protected override void OnActivate()


        {
            this.QAbi = this.context.AbilityFactory.GetAbility<troll_warlord_berserkers_rage>();
            this.WAbi = this.context.AbilityFactory.GetAbility<troll_warlord_whirling_axes_melee>();
            this.EAbi = this.context.AbilityFactory.GetAbility<troll_warlord_whirling_axes_ranged>();
            this.RAbi = this.context.AbilityFactory.GetAbility<troll_warlord_battle_trance>();

            this.Context.Inventory.Attach(this);

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.Context.Inventory.Detach(this);
        }
    }
}
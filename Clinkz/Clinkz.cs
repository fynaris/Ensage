namespace Clinkz
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
    using Ensage.SDK.Abilities;
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

    [PublicAPI]
    public class Clinkz : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private Hero EnemyHero { get; set; }

        private ITargetSelectorManager TargetSelector { get; }

        public ClinkzConfiguration Config { get; }

        public Clinkz(Key key, ClinkzConfiguration config, IServiceContext context) : base(context, key)
        {
            this.Config = config;
            this.context = context;
            this.TargetSelector = context.TargetSelector;
        }

        public DiffusalBlade DiffBlade
        {
            get
            {
                return DiffBlade1 ?? (DiffusalBlade)DiffBlade2;
            }
        }

        [ItemBinding]
        public item_medallion_of_courage Medalion1 { get; private set; }

        [ItemBinding]
        public item_solar_crest Medallion2 { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }


        [ItemBinding]
        public item_sheepstick SheepStick { get; private set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        [ItemBinding]
        public item_diffusal_blade DiffBlade1 { get; private set; }

        [ItemBinding]
        public item_diffusal_blade_2 DiffBlade2 { get; private set; }


        private Ability QAbi { get; set; }
        private Ability WAbi { get; set; }


        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {      
                if (Config.TargetItem.Value.SelectedValue.Contains("Lock") && Context.TargetSelector.IsActive
                    && (!CanExecute || EnemyHero == null || !EnemyHero.IsValid || !EnemyHero.IsAlive))
                {
                    EnemyHero = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }
                else if (Config.TargetItem.Value.SelectedValue.Contains("Default") && Context.TargetSelector.IsActive)
                {
                    EnemyHero = Context.TargetSelector.Active.GetTargets().FirstOrDefault() as Hero;
                }

                var Silenced = UnitExtensions.IsSilenced(base.Owner);

                if (EnemyHero != null)
                {             
                    if (!Silenced)
                    {
                        if (this.Config.AbilityToggler.Value.IsEnabled(this.QAbi.Name) && this.QAbi.CanBeCasted() && this.Owner.IsInAttackRange(EnemyHero))
                        {
                            this.QAbi.UseAbility();
                            await Await.Delay(this.GetAbilityDelay(base.Owner, QAbi), token);
                        }

                        if (this.Config.AbilityToggler.Value.IsEnabled(this.WAbi.Name) && !this.WAbi.IsAutoCastEnabled && this.WAbi.CanBeCasted() && this.Owner.IsInAttackRange(EnemyHero))
                        {
                            this.WAbi.ToggleAutocastAbility();
                            await Await.Delay(this.GetAbilityDelay(base.Owner, WAbi), token);
                        }
                    }

                    if (!UnitExtensions.IsMagicImmune(EnemyHero)
                        && !EnemyHero.IsInvulnerable()
                        && !UnitExtensions.HasModifier(EnemyHero, "modifier_winter_wyvern_winters_curse"))
                    {
                        if (this.BloodThorn != null &&
                            this.BloodThorn.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.BloodThorn.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled(this.BloodThorn.ToString()))
                        {
                            this.BloodThorn.UseAbility(EnemyHero);
                            await Await.Delay(BloodThorn.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Medalion1 != null &&
                            this.Medalion1.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.Medalion1.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled(this.Medalion1.ToString()))
                        {
                            this.Medalion1.UseAbility(EnemyHero);
                            await Await.Delay(Medalion1.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Medallion2 != null &&
                            this.Medallion2.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.Medallion2.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled(this.Medallion2.ToString()))
                        {
                            this.Medallion2.UseAbility(EnemyHero);
                            await Await.Delay(Medallion2.GetCastDelay(EnemyHero), token);
                        }

                        if (this.SheepStick != null &&
                            this.SheepStick.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.SheepStick.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled(this.SheepStick.ToString()))
                        {
                            this.SheepStick.UseAbility(EnemyHero);
                            await Await.Delay(SheepStick.GetCastDelay(EnemyHero), token);
                        }


                        if (this.DiffBlade != null &&
                            this.DiffBlade.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.DiffBlade.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled("item_diffusal_blade_2"))
                        {
                            this.DiffBlade.UseAbility(EnemyHero);
                            await Await.Delay(DiffBlade.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Orchid != null &&
                            this.Orchid.CanBeCasted && this.Owner.IsInAttackRange(EnemyHero) &&
                            this.Orchid.CanHit(EnemyHero) && !this.Owner.IsAttacking() &&
                            this.Config.ItemToggler.Value.IsEnabled(Orchid.ToString()))
                        {
                            this.Orchid.UseAbility(EnemyHero);
                            await Await.Delay(Orchid.GetCastDelay(EnemyHero), token);
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
            this.QAbi = UnitExtensions.GetAbilityById(this.Owner, AbilityId.clinkz_strafe);
            this.WAbi = UnitExtensions.GetAbilityById(this.Owner, AbilityId.clinkz_searing_arrows);

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
namespace Dark_Willow
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
    using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Prediction;
    using Ensage.SDK.Prediction.Collision;

    [PublicAPI]
    public class DarkWillow : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private Hero EnemyHero { get; set; }

        private Modifier Modifier { get; set; }

        private IPrediction Prediction { get; }


        private ITargetSelectorManager TargetSelector { get; }

        public DarkWillowConfiguration Config { get; }

        public DarkWillow(Key key, DarkWillowConfiguration config, IServiceContext context) : base(context, key)
        {
            this.Config = config;
            this.context = context;
            this.TargetSelector = context.TargetSelector;
            this.Prediction = context.Prediction;
        }

        [ItemBinding]
        public item_medallion_of_courage Medalion1 { get; private set; }

        [ItemBinding]
        public item_solar_crest Medallion2 { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding]
        public item_blink Blink { get; private set; }

        [ItemBinding]
        public item_veil_of_discord Veil_Of_Discord { get; private set; }

        [ItemBinding]
        public item_rod_of_atos Rod_Of_Atos { get; private set; }

        [ItemBinding]
        public item_sheepstick SheepStick { get; private set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        public dark_willow_bramble_maze QAbi { get; set; }
      //  public dark_willow_shadow_realm WAbi { get; set; }
        public dark_willow_cursed_crown EAbi { get; set; }
      //  public dark_willow_terrorize RAbi { get; set; }
        public dark_willow_bedlam DAbi { get; set; }


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
              

                var WAbiCasted = Ensage.SDK.Extensions.UnitExtensions.HasModifier(Owner, "modifier_dark_willow_shadow_realm_buff");
                


                var QAbiTargets = EntityManager<Hero>.Entities.OrderBy(x => x == EnemyHero).Where(x => x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion && !UnitExtensions.IsMagicImmune(x)).ToList();

                if (EnemyHero != null && !EnemyHero.IsInvulnerable() && !UnitExtensions.IsMagicImmune(EnemyHero))
                {

                    if (this.QAbi.CanBeCasted && this.QAbi != null && !Silenced && this.Config.AbilityToggler.Value.IsEnabled(this.QAbi.Ability.Name) && this.QAbi.CanHit(EnemyHero) && !IsInvis)
                    {
                        QAbi.UseAbility(EnemyHero.Position);
                      ///  await Await.Delay(QAbi.GetCastDelay(EnemyHero.Position), token);
                        await Await.Delay(this.GetAbilityDelay(this.Owner, QAbi), token);
                    }
                
                    if (!Silenced && this.EAbi != null && this.Config.AbilityToggler.Value.IsEnabled(this.EAbi.Ability.Name) && this.EAbi.CanBeCasted && !IsInvis && this.EAbi.CanHit(EnemyHero))
                    {   
                        this.EAbi.Ability.UseAbility(EnemyHero);
                        await Await.Delay(this.GetAbilityDelay(this.Owner, EAbi), token);
                    }

                    if (!Silenced && this.DAbi != null && !IsInvis  &&    this.Owner.IsInRange(EnemyHero, 300) && !IsInvis && this.DAbi.CanBeCasted && this.Config.AbilityToggler.Value.IsEnabled(this.DAbi.Ability.Name))
                    {
                        this.DAbi.Ability.UseAbility();
                        await Await.Delay(this.GetAbilityDelay(this.Owner, DAbi), token);
                    }

  //
  //
  //
  //
  //
  //


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

                        if (this.BloodThorn != null && this.BloodThorn.CanBeCasted && !IsInvis  && !this.Owner.IsAttacking() && this.Config.ItemToggler.Value.IsEnabled(this.BloodThorn.Item.Name))
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

                        if (this.SheepStick != null && this.SheepStick.CanBeCasted && !IsInvis && this.Config.ItemToggler.Value.IsEnabled(this.SheepStick.Item.Name))
                        {
                            this.SheepStick.UseAbility(EnemyHero);
                            await Await.Delay(SheepStick.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Orchid != null && this.Orchid.CanBeCasted && !IsInvis && this.Config.ItemToggler.Value.IsEnabled(Orchid.Item.Name))
                        {
                            this.Orchid.UseAbility(EnemyHero);
                            await Await.Delay(Orchid.GetCastDelay(EnemyHero), token);
                        }

                        if (this.Mjollnir != null && this.Mjollnir.CanBeCasted && !IsInvis && this.Owner.IsInAttackRange(EnemyHero) && this.Config.ItemToggler2.Value.IsEnabled(Mjollnir.Item.Name))
                        {
                            this.Mjollnir.UseAbility(this.Owner);
                            await Await.Delay(Mjollnir.GetCastDelay(), token);
                        }

                        if (this.Veil_Of_Discord != null && this.Veil_Of_Discord.CanBeCasted && !IsInvis && this.Config.ItemToggler2.Value.IsEnabled(Veil_Of_Discord.Item.Name))
                        {
                            this.Veil_Of_Discord.UseAbility(EnemyHero.Position);
                            await Await.Delay(Veil_Of_Discord.GetCastDelay(), token);
                        }

                        if (this.Rod_Of_Atos != null && this.Rod_Of_Atos.CanBeCasted && !IsInvis && this.Config.ItemToggler2.Value.IsEnabled(Rod_Of_Atos.Item.Name))
                        {
                            this.Rod_Of_Atos.UseAbility(EnemyHero);
                            await Await.Delay(Rod_Of_Atos.GetCastDelay(), token);
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
            this.QAbi = this.context.AbilityFactory.GetAbility<dark_willow_bramble_maze>();
          //  this.WAbi = this.context.AbilityFactory.GetAbility<dark_willow_shadow_realm>();
            this.EAbi = this.context.AbilityFactory.GetAbility<dark_willow_cursed_crown>();
            this.DAbi = this.context.AbilityFactory.GetAbility<dark_willow_bedlam>();
        //    this.RAbi = this.context.AbilityFactory.GetAbility<dark_willow_terrorize>();

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
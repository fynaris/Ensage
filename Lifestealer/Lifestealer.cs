namespace Lifestealer
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Ensage;
    using Ensage.Heroes;
    using Ensage.Common.Enums;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Threading;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Prediction;
    using Ensage.SDK.Prediction.Collision;
    using Ensage.SDK.TargetSelector;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities;
    using Ensage.SDK.Service;
    using log4net;
    using PlaySharp.Toolkit.Logging;
    using PlaySharp.Toolkit.Helper.Annotations;
    using SharpDX;
    using AbilityId = Ensage.AbilityId;
    using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

    [PublicAPI]
    public class Lifestealer : KeyPressOrbwalkingModeAsync
    {
        //private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private IPrediction Prediction { get; }

        private ITargetSelectorManager TargetSelector { get; }

        public LifestealerConfiguration Config { get; }

        public Lifestealer(Key key,LifestealerConfiguration config, IServiceContext context): base(context, key)
        {
            this.Config = config;
            this.context = context;
            this.TargetSelector = context.TargetSelector;
            this.Prediction = context.Prediction;
        }


        [ItemBinding]
        public item_blink BlinkDagger { get; private set; }

        [ItemBinding]
        public item_abyssal_blade Basher { get; private set; }

        [ItemBinding]
        public item_heavens_halberd Heaven { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding]
        public item_sheepstick SheepStick { get; private set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        [ItemBinding]
        public item_diffusal_blade DiffBlade1 { get; private set; }

        [ItemBinding]
        public item_diffusal_blade_2 DiffBlade2 { get; private set; }


        private Ability Rage { get; set; }
        private Ability OpenWounds { get; set; }


        public override async Task ExecuteAsync(CancellationToken token)
        {
            var sliderValue = this.Config.UseBlinkPrediction.Item.GetValue<Slider>().Value;

            var myHero = ObjectManager.LocalHero;
            var enemyHero = this.TargetSelector.Active.GetTargets().FirstOrDefault();

            var Silenced = Ensage.SDK.Extensions.UnitExtensions.IsSilenced(myHero);
            var Stunned = Ensage.SDK.Extensions.UnitExtensions.IsStunned(myHero);

            if (!Silenced && !Stunned)
            {
                if ((this.BlinkDagger != null) && (this.BlinkDagger.Item.CanBeCasted(enemyHero) && enemyHero != null && base.Owner.Distance2D(enemyHero) <= 1200 + sliderValue && !(base.Owner.Distance2D(enemyHero) <= 400) && this.Config.ItemToggler.Value.IsEnabled(this.BlinkDagger.Item.Name)))
                {
                    var l = (this.Owner.Distance2D(enemyHero) - sliderValue) / sliderValue;
                    var posA = this.Owner.Position;
                    var posB = enemyHero.Position;
                    var x = (posA.X + (l * posB.X)) / (1 + l);
                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
                    var position = new Vector3((int)x, (int)y, posA.Z);

                    this.BlinkDagger.UseAbility(position);
                    await Await.Delay(this.GetItemDelay(position) + (int)Game.Ping, token);
                }

                if (enemyHero != null && this.Config.AbilityToggler.Value.IsEnabled(this.Rage.Name) && myHero.IsAttacking() && this.Rage.CanBeCasted())
                {
                   this.Rage.UseAbility();
                   await Await.Delay(this.GetAbilityDelay(myHero, Rage), token);
                }

                if (enemyHero != null && this.Config.AbilityToggler.Value.IsEnabled(this.OpenWounds.Name) && this.OpenWounds.CanBeCasted(enemyHero))
                {
                    this.OpenWounds.UseAbility(enemyHero);
                    await Await.Delay(this.GetAbilityDelay(enemyHero, OpenWounds), token);
                }
            }

            if (this.BloodThorn != null &&
                this.BloodThorn.Item.IsValid &&
                enemyHero != null && 
                this.BloodThorn.Item.CanBeCasted(enemyHero) &&
                this.Config.ItemToggler.Value.IsEnabled(this.BloodThorn.Item.Name))
            {
                this.BloodThorn.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.Basher != null &&
                (this.Basher.Item.IsValid) &&
                enemyHero != null && myHero.IsAttacking() &&
                this.Basher.Item.CanBeCasted(enemyHero) &&
                this.Config.ItemToggler.Value.IsEnabled("item_abyssal_blade"))
            {
                this.Basher.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.DiffBlade1 != null &&
              (this.DiffBlade1.Item.IsValid) &&
              enemyHero != null &&
              this.DiffBlade1.Item.CanBeCasted(enemyHero) &&
              this.Config.ItemToggler.Value.IsEnabled("item_diffusal_blade"))
            {
                this.DiffBlade1.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.DiffBlade2 != null &&
              (this.DiffBlade2.Item.IsValid) &&
              enemyHero != null &&
              this.DiffBlade2.Item.CanBeCasted(enemyHero) &&
              this.Config.ItemToggler.Value.IsEnabled("item_diffusal_blade_2"))
            {
                this.DiffBlade2.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.Orchid != null &&
                this.Orchid.Item.IsValid &&
                enemyHero != null &&
                this.Orchid.Item.CanBeCasted(enemyHero) &&
                this.Config.ItemToggler.Value.IsEnabled("item_orchid"))
            {    
                this.Orchid.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.Heaven != null &&
                this.Heaven.Item.IsValid &&
                enemyHero != null && myHero.IsAttacking() &&
                this.Heaven.Item.CanBeCasted(enemyHero) &&
                this.Config.ItemToggler.Value.IsEnabled("item_heavens_halberd"))
            {
                this.Heaven.UseAbility(enemyHero);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (this.Mjollnir != null &&
                this.Mjollnir.Item.IsValid && myHero.IsAttacking() &&
                enemyHero != null &&
                this.Mjollnir.Item.CanBeCasted() &&
                this.Config.ItemToggler.Value.IsEnabled("item_mjollnir"))
            {
                this.Mjollnir.UseAbility(base.Owner);
                await Await.Delay(this.GetItemDelay(enemyHero), token);
            }

            if (enemyHero != null && !base.Owner.IsValidOrbwalkingTarget(enemyHero))
            {
                Orbwalker.Move(Game.MousePosition);
                await Await.Delay(50, token);
            }
            else
            {
                Orbwalker.OrbwalkTo(enemyHero);
            }

            await Await.Delay(50, token);
        }

        protected int GetAbilityDelay(Unit unit, Ability ability)
        {
            return (int)(((ability.FindCastPoint() + this.Owner.GetTurnTime(unit)) * 1000.0) + Game.Ping) + 50;
        }

        protected int GetAbilityDelay(Vector3 pos, Ability ability)
        {
            return (int)(((ability.FindCastPoint() + this.Owner.GetTurnTime(pos)) * 1000.0) + Game.Ping) + 50;
        }

        protected int GetItemDelay(Unit unit)
        {
            return (int)((this.Owner.GetTurnTime(unit) * 1000.0) + Game.Ping) + 100;
        }

        protected int GetItemDelay(Vector3 pos)
        {
            return (int)((this.Owner.GetTurnTime(pos) * 1000.0) + Game.Ping) + 100;
        }

        private void GameDispatcher_OnIngameUpdate(EventArgs args)
        {
         //  if (!this.Config.KillStealEnabled.Value)
         //   {
         //       return;
           // }

            if (!Game.IsPaused && Owner.IsAlive && !UnitExtensions.IsChanneling(Owner))
            {
                //Await.Block("MyKillstealer", KillStealAsync);
            }
        }

        protected override void OnActivate()
        {
            GameDispatcher.OnIngameUpdate += GameDispatcher_OnIngameUpdate;
            this.Rage = UnitExtensions.GetAbilityById(this.Owner, AbilityId.life_stealer_rage);
            this.OpenWounds = UnitExtensions.GetAbilityById(this.Owner, AbilityId.life_stealer_open_wounds);

            this.Context.Inventory.Attach(this);

            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            GameDispatcher.OnIngameUpdate -= GameDispatcher_OnIngameUpdate;
            base.OnDeactivate();
            this.Context.Inventory.Detach(this);
        }
    }
}
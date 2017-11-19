using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace Sven
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Threading;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_sven;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Renderer.Particle;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    using SharpDX;

    internal class Core : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IParticleManager _particleManager;
        private readonly Configuration _configuration;
        private readonly IUpdateHandler _targetParticleUpdateHandler;
        private readonly ITargetSelectorManager _targetSelector;



        private Unit _target;


        public Core(IServiceContext context, Configuration configuration) : base(context, configuration.ComboKey)
        {
            _targetSelector = context.TargetSelector;
            _configuration = configuration;

            _particleManager = Context.Particle;
            _targetParticleUpdateHandler = UpdateManager.Subscribe(UpdateTargetParticle, 0, false);
        }



        [ItemBinding]
        public item_mask_of_madness Mask { get; set; }

        [ItemBinding]
        public item_blink Blink { get; set; }

        [ItemBinding]
        public item_heavens_halberd Halberd { get; set; }

        [ItemBinding]
        public item_abyssal_blade Abyssal { get; set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; set; }

        [ItemBinding]
        public item_bloodthorn Bloodthorn { get; set; }

        [ItemBinding]
        public item_orchid Orchid { get; set; }

        [ItemBinding]
        public item_sheepstick SheepStick { get; set; }

        [ItemBinding]
        public item_diffusal_blade DiffusalBlade { get; set; }


        public sven_storm_bolt Bolt { get; set; }
        public sven_warcry Warcry { get; set; }
        public sven_gods_strength GodsStrength { get; set; }


        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {


                _target = _targetSelector.Active.GetTargets().FirstOrDefault();
                var blinkPdValue = _configuration.UseBlink.Item.GetValue<Slider>().Value;


                if (_target == null || _target.IsAttackImmune() || _target.IsInvulnerable())
                {
                    Orbwalker.OrbwalkTo(null);
                    return;
                }


                if (_configuration.AbilityManager.Value.IsEnabled(Warcry.Ability.Name) && Warcry.CanBeCasted && Owner.IsInRange(_target, 1500))
                {

                    Warcry.UseAbility();
                    await Await.Delay(Warcry.GetCastDelay(Owner), token);
                }


                if (_configuration.AbilityManager.Value.IsEnabled(GodsStrength.Ability.Name) && GodsStrength.CanBeCasted && Owner.IsInRange(_target, 1500))
                {
                    GodsStrength.UseAbility();
                    await Await.Delay(GodsStrength.GetCastDelay(Owner), token);
                }


                if (Mjollnir != null && Mjollnir.CanBeCasted && !Owner.IsAttacking() && Owner.IsInRange(_target, 1500) &&
                     _configuration.ItemManager.Value.IsEnabled(Mjollnir.Ability.Name))
                {
                    Mjollnir.UseAbility(Owner);
                    await Await.Delay(Mjollnir.GetCastDelay(Owner), token);
                }


                if (Blink != null && Blink.Item.IsValid && Blink.CanBeCasted && _target != null &&
                    Owner.Distance2D(_target) <= 1200 + blinkPdValue && !(Owner.Distance2D(_target) <= 150) &&
                    _configuration.ItemManager.Value.IsEnabled(Blink.Ability.Name))
                {
                    var l = (Owner.Distance2D(_target) - blinkPdValue) / blinkPdValue;
                    var posA = Owner.Position;
                    var posB = _target.Position;
                    var x = (posA.X + (l * posB.X)) / (1 + l);
                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
                    var position = new Vector3((int)x, (int)y, posA.Z);

                    Blink.UseAbility(position);
                    await Await.Delay(Blink.GetCastDelay(position), token);
                }


                if (_configuration.AbilityManager.Value.IsEnabled(Bolt.Ability.Name) && Bolt.CanBeCasted && Owner.IsInRange(_target, 600) && !(_target.IsStunned() || _target.IsHexed()))
                {

                    Bolt.UseAbility(_target);
                    await Await.Delay(Bolt.GetCastDelay(Owner), token);
                }
          
                if (DiffusalBlade != null && DiffusalBlade.CanBeCasted && !Owner.IsAttacking() && !_target.IsEthereal() &&
                    _configuration.ItemManager.Value.IsEnabled(DiffusalBlade.Ability.Name) && !_target.IsMagicImmune() &&
                    Owner.IsInRange(_target, 600))
                {
                    DiffusalBlade.Ability.UseAbility(_target);
                    await Await.Delay(DiffusalBlade.GetCastDelay(_target), token);

                }

                if (Bloodthorn != null && Bloodthorn.CanBeCasted && !Owner.IsAttacking() && !_target.IsMagicImmune() &&
                    _configuration.ItemManager.Value.IsEnabled(Bloodthorn.Ability.Name) && Owner.IsInRange(_target, 600) && !_target.IsSilenced())
                {
                    Bloodthorn.UseAbility(_target);
                    await Await.Delay(Bloodthorn.GetCastDelay(_target), token);
                }

                if (Orchid != null && Orchid.CanBeCasted && !Owner.IsAttacking() && !_target.IsMagicImmune() &&
                    _configuration.ItemManager.Value.IsEnabled(Orchid.Ability.Name) && Owner.IsInRange(_target, 600) && !_target.IsSilenced())
                {
                    Orchid.UseAbility(_target);
                    await Await.Delay(Orchid.GetCastDelay(_target), token);
                }


                if (SheepStick != null && SheepStick.CanBeCasted && !Owner.IsAttacking() && !_target.IsMagicImmune() &&
                _configuration.ItemManagerTwo.Value.IsEnabled(SheepStick.Ability.Name) && !(_target.IsStunned() || _target.IsHexed()) &&
                Owner.IsInRange(_target, 600))
                {
                    SheepStick.UseAbility(_target);
                    await Await.Delay(SheepStick.GetCastDelay(_target), token);
                }


                if (Mjollnir != null && Mjollnir.CanBeCasted && !Owner.IsAttacking() && _target.IsAttacking() &&
                Owner.IsInRange(_target, 600) && _configuration.ItemManagerTwo.Value.IsEnabled(Mjollnir.Ability.Name))
                {
                    Mjollnir.UseAbility(Owner);
                    await Await.Delay(Mjollnir.GetCastDelay(Owner), token);
                }

                if (Halberd != null && Halberd.CanBeCasted && !Owner.IsAttacking() && _target.IsAttacking() &&
                    Owner.IsInRange(_target, 600) && _configuration.ItemManagerTwo.Value.IsEnabled(Halberd.Ability.Name))
                {
                    Halberd.UseAbility(_target);
                    await Await.Delay(Halberd.GetCastDelay(_target), token);
                }

                if (Abyssal != null && Abyssal.CanBeCasted && !Owner.IsAttacking() && !(_target.IsStunned() || _target.IsHexed()) &&
                    Owner.IsInRange(_target, 600) && _configuration.ItemManagerTwo.Value.IsEnabled(Abyssal.Ability.Name))
                {
                    Abyssal.Ability.CastStun(_target);
                    await Await.Delay(Abyssal.GetCastDelay(_target), token);
                }


                if (Mask != null && Mask.CanBeCasted && !Owner.IsAttacking() &&
                    Owner.IsInRange(_target, 600) && _configuration.ItemManager.Value.IsEnabled(Mask.Ability.Name) && !Bolt.CanBeCasted && !Warcry.CanBeCasted && !GodsStrength.CanBeCasted)
                {
                    Mask.UseAbility();
                    await Await.Delay(Mask.GetCastDelay(Owner), token);
                }







                if (_target != null && (_target.IsInvulnerable() || _target.IsAttackImmune()))
                {
                    Orbwalker.Move(Game.MousePosition);
                }
                else if (_target != null)
                {
                    Orbwalker.OrbwalkTo(_target);
                }
                else
                {
                    Orbwalker.Move(Game.MousePosition);
                }

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Context.Inventory.Attach(this);

            MenuKey.PropertyChanged += MenuKeyOnPropertyChanged;

            Bolt = Context.AbilityFactory.GetAbility<sven_storm_bolt>();
            Warcry = Context.AbilityFactory.GetAbility<sven_warcry>();
            GodsStrength = Context.AbilityFactory.GetAbility<sven_gods_strength>();
        }

        protected override void OnDeactivate()
        {
            Context.Inventory.Detach(this);

            MenuKey.PropertyChanged -= MenuKeyOnPropertyChanged;
            UpdateManager.Unsubscribe(UpdateTargetParticle);
            base.OnDeactivate();
        }

        private void MenuKeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (MenuKey)
            {
                if (_configuration.DrawTargetParticle)
                {
                    _targetParticleUpdateHandler.IsEnabled = true;
                }
            }
            else
            {
                if (_configuration.DrawTargetParticle)
                {
                    _particleManager.Remove("Target");
                    _targetParticleUpdateHandler.IsEnabled = false;
                }
            }
        }
        private void UpdateTargetParticle()
        {
            if (_target == null || !_target.IsValid || !_target.IsVisible)
            {
                _particleManager.Remove("Target");
                return;
            }
            _particleManager.DrawTargetLine(Owner, "Target", _target.Position, Color.DeepPink);
        }
    }
}
namespace Sven
{
    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Configuration : IDisposable
    {
        public readonly MenuFactory MFactory;

        public Configuration(string ownerName)
        {

            var itemTab = new List<AbilityId>
            {
                AbilityId.item_mask_of_madness,
                AbilityId.item_blink,
                AbilityId.item_bloodthorn,
                AbilityId.item_orchid,
                AbilityId.item_diffusal_blade,
            };

            var itemTabTwo = new List<AbilityId>
            {

                AbilityId.item_mjollnir,
                AbilityId.item_sheepstick,
                AbilityId.item_heavens_halberd,
                AbilityId.item_abyssal_blade
            };


            var spellTab = new List<AbilityId>
            {
                AbilityId.sven_storm_bolt,
                AbilityId.sven_warcry,
                AbilityId.sven_gods_strength
            };



            MFactory = MenuFactory.CreateWithTexture("Sven", ownerName);



            DrawTargetParticle = MFactory.Item("Draw target particle", true);
            
            UseBlink = MFactory.Item("Blink Prediction", new Slider(150, 0, 600));
            ComboKey = MFactory.Item("Combo Key", new KeyBind(32));
            

            var itemMenu = MFactory.Menu("Item Manager");
            ItemManager = itemMenu.Item("Item Toggler:  ", new AbilityToggler(itemTab.ToDictionary(x => x.ToString(), x => true)));
            ItemManagerTwo = itemMenu.Item("", new AbilityToggler(itemTabTwo.ToDictionary(x => x.ToString(), x => true)));

            var abilityMenu = MFactory.Menu("Ability Manager");
            AbilityManager = abilityMenu.Item("Ability Toggler:  ", new AbilityToggler(spellTab.ToDictionary(x => x.ToString(), x => true)));

        }



        public MenuItem<KeyBind> ComboKey { get; }

        public MenuItem<bool> DrawTargetParticle { get; }

       
        public MenuItem<AbilityToggler> ItemManager { get; }

        public MenuItem<AbilityToggler> ItemManagerTwo { get; }

        public MenuItem<AbilityToggler> AbilityManager { get; }

        public MenuItem<Slider> UseBlink { get; }

        public void Dispose()
        {
            MFactory.Dispose();
        }
    }
}
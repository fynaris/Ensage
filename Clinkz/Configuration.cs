namespace Clinkz
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
            MFactory = MenuFactory.CreateWithTexture("Clinkz", ownerName);
            DrawTargetParticle = MFactory.Item("Draw target particle", true);
            ComboKey = MFactory.Item("Combo Key", new KeyBind(32));

            var itemTabOne = new List<AbilityId>
            {
                AbilityId.item_medallion_of_courage,
                AbilityId.item_solar_crest,
                AbilityId.item_sheepstick,
                AbilityId.item_diffusal_blade
            };

            var itemTabTwo = new List<AbilityId>
            {
                AbilityId.item_orchid,
                AbilityId.item_bloodthorn,
                AbilityId.item_mjollnir,
               
            };

            var spells = new List<AbilityId>
            {
                AbilityId.clinkz_death_pact,
                AbilityId.clinkz_searing_arrows,
                AbilityId.clinkz_strafe
            };

    
            Abilitys = MFactory.Item("Ability Toggler: ", new AbilityToggler(spells.ToDictionary(x => x.ToString(), x => true)));
            ItemTabOne = MFactory.Item("Item Toggler:  ", new AbilityToggler(itemTabOne.ToDictionary(x => x.ToString(), x => true)));
            ItemTabTwo = MFactory.Item("", new AbilityToggler(itemTabTwo.ToDictionary(x => x.ToString(), x => true)));
        }


        public MenuItem<KeyBind> ComboKey { get; }

        public MenuItem<bool> DrawTargetParticle { get; }

        

        public MenuItem<AbilityToggler> ItemTabOne { get; }
        public MenuItem<AbilityToggler> ItemTabTwo { get; }


        public MenuItem<AbilityToggler> Abilitys { get; }

        public void Dispose()
        {
            MFactory.Dispose();
        }
    }
}
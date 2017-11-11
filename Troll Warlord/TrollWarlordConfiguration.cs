namespace Troll_Warlord
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    public class TrollWarlordConfiguration
    {
        private bool _disposed;

        public TrollWarlordConfiguration()
        {

            var itemDict1 = new Dictionary<string, bool>
            {
                {"item_bloodthorn", true},
                {"item_orchid", true},
                {"item_diffusal_blade", true },
                {"item_solar_crest", true},
                {"item_medallion_of_courage", true},
                {"item_sheepstick", true }
           


            };

            var itemDict2 = new Dictionary<string, bool>
            {
           
                {"item_silver_edge", true },
                {"item_invis_sword", true },
                {"item_blink", true },
                {"item_mask_of_madness", true },
                {"item_mjollnir", true },
                {"item_heavens_halberd", true }


            };

            var spellDict = new Dictionary<string, bool>
            {
                {"troll_warlord_berserkers_rage", true},
                {"troll_warlord_whirling_axes_melee", true },
                {"troll_warlord_whirling_axes_ranged", true },
                {"troll_warlord_battle_trance", true }
            };

            this.Menu = MenuFactory.Create("Troll Warlord");
            this.Key = this.Menu.Item("Combo Key", new KeyBind(32));
            this.Target = Menu.Item("Target", new StringList("Lock", "Default"));
            this.MomUsage = Menu.Item("Use Mask Of Mandess Only When In", new StringList("Melee Form", "Ranged Form"));
            this.UseBlink = this.Menu.Item("Blink Prediction", new Slider(150, 150, 500));

            this.AbilityToggler = this.Menu.Item("Ability Toggler", new AbilityToggler(spellDict));

      

            this.ItemToggler = this.Menu.Item("Item Toggler", new AbilityToggler(itemDict1));
            this.ItemToggler2 = this.Menu.Item("", new AbilityToggler(itemDict2));

           

        }

        public MenuFactory Menu { get; }
        public MenuItem<AbilityToggler> AbilityToggler { get; }
        public MenuItem<Slider> UseBlink { get; }
        public MenuItem<KeyBind> Key { get; }
        public MenuItem<AbilityToggler> ItemToggler { get; }
        public MenuItem<AbilityToggler> ItemToggler2 { get; }
        public MenuItem<StringList> Target { get; }
        public MenuItem<StringList> MomUsage { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Menu.Dispose();
            }

            this._disposed = true;
        }
    }
}
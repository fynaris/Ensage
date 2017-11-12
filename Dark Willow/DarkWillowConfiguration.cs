namespace Dark_Willow
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    public class DarkWillowConfiguration
    {
        private bool _disposed;

        public DarkWillowConfiguration()
        {

            var itemDict1 = new Dictionary<string, bool>
            {
                {"item_bloodthorn", true},
                {"item_orchid", true},
                {"item_solar_crest", true},
                {"item_medallion_of_courage", true},
                {"item_sheepstick", true }



            };

            var itemDict2 = new Dictionary<string, bool>
            {

                {"item_rod_of_atos", true },
                {"item_blink", true },
                {"item_mjollnir", true },
                {"item_veil_of_discord", true }


            };

            var spellDict = new Dictionary<string, bool>
            {
                {"dark_willow_bramble_maze", true },
               //  {"dark_willow_shadow_realm", true },
                {"dark_willow_cursed_crown", true},
                {"dark_willow_bedlam", true }
             //   {"dark_willow_terrorize", true }
            };

            this.Menu = MenuFactory.Create("Dark Willow");
            this.Key = this.Menu.Item("Combo Key", new KeyBind(32));
            this.Target = Menu.Item("Target", new StringList("Lock", "Default"));
            this.UseBlink = this.Menu.Item("Blink Prediction", new Slider(475, 475, 500));

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
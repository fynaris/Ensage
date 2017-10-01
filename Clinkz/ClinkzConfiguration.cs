namespace Clinkz
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    public class ClinkzConfiguration
    {
        private bool _disposed;

        public ClinkzConfiguration()
        {

            var itemDict = new Dictionary<string, bool>
            {
                {"item_bloodthorn", true},
                {"item_orchid", true},
                {"item_diffusal_blade_2", true },
                {"item_solar_crest", true},
                {"item_medallion_of_courage", true},
                {"item_sheepstick", true }


            };

            var spellDict = new Dictionary<string, bool>
            {
                {"clinkz_strafe", true},
                {"clinkz_searing_arrows", true }
            };

            this.Menu = MenuFactory.Create("Clinkz");
            this.Key = this.Menu.Item("Combo Key", new KeyBind(32));
            this.TargetItem = Menu.Item("Target", new StringList("Lock", "Default"));
            this.Key.Item.Tooltip = "Hold this key to start combo mode.";
            this.AbilityToggler = this.Menu.Item("Ability Toggler", new AbilityToggler(spellDict));
            this.ItemToggler = this.Menu.Item("Item Toggler", new AbilityToggler(itemDict));
        }

        public MenuFactory Menu { get; }
        public MenuItem<AbilityToggler> AbilityToggler { get; }
        public MenuItem<KeyBind> Key { get; }
        public MenuItem<AbilityToggler> ItemToggler { get; }
        public MenuItem<StringList> TargetItem { get; }

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

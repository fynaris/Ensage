namespace OneKeyKing
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    public class OneKeyKingConfiguration
    {
        private bool _disposed;

        public OneKeyKingConfiguration()
        {

            var itemDict = new Dictionary<string, bool>
            {
                {"item_bloodthorn", true},
                {"item_blink", true},
                {"item_orchid", true},
                {"item_mjollnir", true},
                {"item_abyssal_blade", true},
                {"item_heavens_halberd", true },
                {"item_diffusal_blade_2", true },
                {"item_solar_crest", true},
                {"item_medallion_of_courage", true}
             

            };

            var spellDict = new Dictionary<string, bool>
            {
                {"skeleton_king_hellfire_blast", true}
            };

            this.Menu = MenuFactory.Create("OneKeyKing");
            this.Key = this.Menu.Item("Combo Key", new KeyBind(32));
            this.TargetItem = Menu.Item("Target", new StringList("Lock", "Default"));
            this.Key.Item.Tooltip = "Hold this key to start combo mode.";
            this.UseBlinkPrediction = this.Menu.Item("Blink Prediction", new Slider(150, 0, 600));
            this.UseBlinkPrediction.Item.Tooltip = "Will blink to set distance from targeted hero. Set to 0 if you want to disable it.";
            this.AbilityToggler = this.Menu.Item("Ability Toggler", new AbilityToggler(spellDict));
            this.ItemToggler = this.Menu.Item("Item Toggler", new AbilityToggler(itemDict));
        }

        public MenuFactory Menu { get; }
        public MenuItem<Slider> UseBlinkPrediction { get; }
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

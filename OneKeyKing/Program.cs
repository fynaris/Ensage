namespace OneKeyKing
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows.Input;
    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin(name: "OneKeyKing", mode: StartupMode.Auto, author: "Jerffelly", version: "2.0.0.0", units: HeroId.npc_dota_hero_skeleton_king)]

    public class Program : Plugin
    {
        private readonly IServiceContext context;

        [ImportingConstructor]
        public Program([Import] IServiceContext context)
        {
            this.context = context;
        }

        public OneKeyKingConfiguration Config { get; private set; }
        public OneKeyKing OrbwalkerMode { get; private set; }

        protected override void OnActivate()
        {
            this.Config = new OneKeyKingConfiguration();
            this.Config.Key.Item.ValueChanged += this.HotkeyChanged;

            this.OrbwalkerMode = new OneKeyKing(this.Config.Key, this.Config, this.context);

            this.context.Orbwalker.RegisterMode(this.OrbwalkerMode);
        }

        protected override void OnDeactivate()
        {
            this.context.Orbwalker.UnregisterMode(this.OrbwalkerMode);
            this.Config.Key.Item.ValueChanged -= this.HotkeyChanged;
            this.Config.Dispose();
        }

        private void HotkeyChanged(object sender, OnValueChangeEventArgs e)
        {
            var keyCode = e.GetNewValue<KeyBind>().Key;
            if (keyCode == e.GetOldValue<KeyBind>().Key)
            {
                return;
            }

            var key = KeyInterop.KeyFromVirtualKey((int)keyCode);
            this.OrbwalkerMode.Key = key;
        }
    }
}
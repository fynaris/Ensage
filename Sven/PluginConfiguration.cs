namespace Sven
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Sven", HeroId.npc_dota_hero_sven)]
    internal class PluginConfiguration : Plugin
    {
        private readonly IServiceContext _context;
        private readonly Unit _owner;
        private Core _core;
        private Configuration _configuration;

        [ImportingConstructor]
        public PluginConfiguration(IServiceContext context)
        {
            _context = context;
            _owner = context.Owner;
        }

        protected override void OnActivate()
        {
            _configuration = new Configuration(_owner.Name);
            _core = new Core(_context, _configuration);
            _context.Orbwalker.RegisterMode(_core);
        }

        protected override void OnDeactivate()
        {
            _context.Orbwalker.UnregisterMode(_core);
            _configuration.Dispose();
        }
    }
}
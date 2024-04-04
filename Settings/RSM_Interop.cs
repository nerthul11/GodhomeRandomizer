using GodhomeRandomizer.Manager;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace GodhomeRandomizer.Settings
{
    internal static class RSM_Interop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new GodhomeSettingsProxy());
        }
    }

    internal class GodhomeSettingsProxy : RandoSettingsProxy<GodhomeRandomizerSettings, string>
    {
        public override string ModKey => GodhomeRandomizer.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(GodhomeRandomizer.Instance.GetVersion());

        public override void ReceiveSettings(GodhomeRandomizerSettings settings)
        {
            if (settings != null)
            {
                ConnectionMenu.Instance!.Apply(settings);
            }
            else
            {
                ConnectionMenu.Instance!.Disable();
            }
        }

        public override bool TryProvideSettings(out GodhomeRandomizerSettings settings)
        {
            settings = GodhomeManager.GlobalSettings;
            return settings.Enabled;
        }
    }
}
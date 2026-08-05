using ConnectionSettingsRando;
using GodhomeRandomizer.Manager;

namespace GodhomeRandomizer.Interop
{
    internal static class CSR_Interop
    {
        public static void Hook()
        {
            CSR.Register(
            GodhomeRandomizer.Instance.GetName(),
            () => GodhomeManager.GlobalSettings,
            s => SettingsRandomizer.CopyTo(s, GodhomeManager.GlobalSettings));
        }
    }
}
using Harmony;

namespace CountersPlus.Harmony
{
    /// <summary>
    /// Apply and remove all of our Harmony patches through this class
    /// </summary>
    internal class CountersPlusPatcehs
    {
        internal static bool IsPatched { get; private set; }

        private static HarmonyInstance Instance;
        private static string InstanceId = "com.caeden117.beatsaber.countersplus";

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (Instance == null)
                {
                    Instance = HarmonyInstance.Create(InstanceId);
                }

                Instance.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (Instance != null && IsPatched)
            {
                Instance.UnpatchAll(InstanceId);
                IsPatched = false;
            }
        }
    }
}

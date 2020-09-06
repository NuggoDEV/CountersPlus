using CountersPlus.ConfigModels;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Zenject;

namespace CountersPlus.Harmony
{
    /*
     * Because of some counter's reliance on the Base Game HUD,
     * we need to ensure that it still spawns. We would then yoink their HUD elements as usual,
     * then we hide the HUD as if it never spawned.
     */
    [HarmonyPatch(typeof(GameplayCoreSceneSetup))]
    [HarmonyPatch("InstallBindings")]
    internal class GameplayCoreSceneSetupPatch
    {
        public static bool IsOverridingBaseGameHUD = false;

        private static MethodInfo skipHUDMethod => SymbolExtensions.GetMethodInfo(() => ShouldSkip(null, null));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            object containerOperand = null;

            List<CodeInstruction> codes = original.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (containerOperand is null)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].opcode == OpCodes.Call)
                    {
                        containerOperand = codes[i + 1].operand;
                    }
                }
                else
                {
                    if (codes[i].opcode == OpCodes.Ldloc_3 &&
                        codes[i + 1].opcode == OpCodes.Callvirt &&
                        codes[i + 2].opcode == OpCodes.Brtrue_S)
                    {
                        codes.RemoveAt(i + 1);
                        codes.InsertRange(i + 1, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Callvirt, containerOperand),
                            new CodeInstruction(OpCodes.Call, skipHUDMethod),
                        });
                        break;
                    }
                }
            }
            return codes;
        }

        public static bool ShouldSkip(PlayerSpecificSettings specificSettings, DiContainer container)
        {
            ProgressConfigModel progress = container.Resolve<ProgressConfigModel>();
            ScoreConfigModel score = container.Resolve<ScoreConfigModel>();
            bool result = specificSettings.noTextsAndHuds && !(
                (progress.Enabled && progress.Mode == ProgressMode.BaseGame) ||
                score.Enabled);
            IsOverridingBaseGameHUD = specificSettings.noTextsAndHuds && !result;
            return result;
        }
    }
}

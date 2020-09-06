using CountersPlus.ConfigModels;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CountersPlus.Harmony
{
    /*
     * This patch ensures that only the HUD elements we want are spawned in, then the child objects
     * of CoreGameHUDController are hidden.
     */
    [HarmonyPatch(typeof(CoreGameHUDController))]
    [HarmonyPatch("Start")]
    internal class CoreGameHUDControllerPatch
    {
        private static MethodInfo ProgressMethod = SymbolExtensions.GetMethodInfo(() => ShouldEnableProgressPanel(false));
        private static MethodInfo ScoreMethod = SymbolExtensions.GetMethodInfo(() => ShouldEnableScorePanel(false));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            List<CodeInstruction> codes = original.ToList();
            int callvirtsFound = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt)
                {
                    callvirtsFound++;
                    switch (callvirtsFound)
                    {
                        case 1:
                            codes.Insert(i, new CodeInstruction(OpCodes.Call, ProgressMethod));
                            break;
                        case 2:
                        case 3:
                            codes.Insert(i, new CodeInstruction(OpCodes.Call, ScoreMethod)); 
                            break;
                    }
                    i++; // Increment i again so we do not enter an infinite loop.
                }
            }
            return codes;
        }

        private static bool ShouldEnableProgressPanel(bool original)
        {
            if (!GameplayCoreSceneSetupPatch.IsOverridingBaseGameHUD) return original;
            ProgressConfigModel progressConfig = Plugin.MainConfig.ProgressConfig;
            return original || (progressConfig.Enabled && progressConfig.Mode == ProgressMode.BaseGame);
        }

        private static bool ShouldEnableScorePanel(bool original)
        {
            if (!GameplayCoreSceneSetupPatch.IsOverridingBaseGameHUD) return original;
            ScoreConfigModel scoreConfig = Plugin.MainConfig.ScoreConfig;
            return original || scoreConfig.Enabled;
        }

        private static void Postfix(CoreGameHUDController __instance)
        {
            if (GameplayCoreSceneSetupPatch.IsOverridingBaseGameHUD)
                SharedCoroutineStarter.instance.StartCoroutine(RemoveAfterOneFrame(__instance));
        }

        private static IEnumerator RemoveAfterOneFrame(CoreGameHUDController coreGameHUD)
        {
            yield return new WaitForEndOfFrame();
            foreach (Transform child in coreGameHUD.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}

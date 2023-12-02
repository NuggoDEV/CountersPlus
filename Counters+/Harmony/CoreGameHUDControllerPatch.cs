using CountersPlus.ConfigModels;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyObj = HarmonyLib.Harmony;

namespace CountersPlus.Harmony
{
    /*
     * This patch ensures that only the HUD elements we want are spawned in, then the child objects
     * of CoreGameHUDController are hidden.
     */
    [HarmonyPatch(typeof(CoreGameHUDController))]
    [HarmonyPatch("Initialize")]
    internal class CoreGameHUDControllerPatch
    {
        private static MethodInfo ProgressMethod = SymbolExtensions.GetMethodInfo(() => ShouldEnableProgressPanel(false));
        private static MethodInfo ScoreMethod = SymbolExtensions.GetMethodInfo(() => ShouldEnableScorePanel(false));
        private static MethodInfo SkipMethod = SymbolExtensions.GetMethodInfo(() => ShouldSkip(false));

        private static bool isOverriding = false;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            List<CodeInstruction> codes = original.ToList();
            int callvirtsFound = 0;
            bool insertedCheck = false;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt)
                {
                    callvirtsFound++;
                    switch (callvirtsFound)
                    {
                        case 1:
                            if (!insertedCheck)
                            {
                                insertedCheck = true;
                                codes.InsertRange(i - 4, new List<CodeInstruction>()
                                {
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Call, SkipMethod),
                                    new CodeInstruction(OpCodes.Not),
                                    new CodeInstruction(OpCodes.And)
                                });
                            }
                            i += 3; // Push it forward the same length as the amount of code we entered.
                            break;
                        case 2:
                            codes.Insert(i, new CodeInstruction(OpCodes.Call, ProgressMethod));
                            break;
                        case 3:
                        case 4:
                            codes.Insert(i, new CodeInstruction(OpCodes.Call, ScoreMethod)); 
                            break;
                    }
                    i++; // Increment i again so we do not enter an infinite loop.
                }
            }
            return codes;
        }

        public static bool ShouldSkip(bool original)
        {
            HUDConfigModel hudConfig = Plugin.MainConfig.HUDConfig;
            ProgressConfigModel progress = Plugin.MainConfig.ProgressConfig;
            ScoreConfigModel score = Plugin.MainConfig.ScoreConfig;
            isOverriding = original && (
                (progress.Enabled && progress.Mode == ProgressMode.BaseGame && CheckIgnoreOption(hudConfig, progress)) ||
                (score.Enabled && CheckIgnoreOption(hudConfig, score)));
            return isOverriding;
        }

        private static bool CheckIgnoreOption(HUDConfigModel hud, ConfigModel model)
        {
            if (model.CanvasID == -1) return hud.MainCanvasSettings.IgnoreNoTextAndHUDOption;
            return hud.OtherCanvasSettings[model.CanvasID].IgnoreNoTextAndHUDOption;
        }

        private static bool ShouldEnableProgressPanel(bool original)
        {
            if (!isOverriding) return original;
            ProgressConfigModel progressConfig = Plugin.MainConfig.ProgressConfig;
            return original || (progressConfig.Enabled && progressConfig.Mode == ProgressMode.BaseGame);
        }

        private static bool ShouldEnableScorePanel(bool original)
        {
            if (!isOverriding) return original;
            ScoreConfigModel scoreConfig = Plugin.MainConfig.ScoreConfig;
            return original || scoreConfig.Enabled;
        }

        private static void Postfix(CoreGameHUDController __instance)
        {
            if (isOverriding)
                Utils.SharedCoroutineStarter.instance.StartCoroutine(RemoveAfterOneFrame(__instance));
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

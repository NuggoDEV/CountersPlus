using CountersPlus.ConfigModels;
using CountersPlus.Installers;
using CountersPlus.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyObj = HarmonyLib.Harmony;

namespace CountersPlus.Harmony
{
    public class MenuTransitionsHelperPatch
    {
        private static CanvasUtility canvasUtility;
        private static HUDConfigModel hudConfig;
        private static MenuUIInstaller menuInstaller;

        private static MethodInfo menuTransitions = typeof(MenuTransitionsHelper).GetMethod("RestartGame",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        private static MethodInfo transpiler = SymbolExtensions.GetMethodInfo(() => Transpiler(null));
        private static MethodInfo modifyAction = SymbolExtensions.GetMethodInfo(() => ModifyAndAddAction(null));

        public static void Patch(HarmonyObj obj, CanvasUtility canvas, HUDConfigModel hud, MenuUIInstaller menuUIInstaller)
        {
            if (obj.GetPatchedMethods().Any(x => x == menuTransitions)) obj.Unpatch(menuTransitions, transpiler);

            canvasUtility = canvas;
            hudConfig = hud;
            menuInstaller = menuUIInstaller;

            obj.Patch(menuTransitions, null, null, new HarmonyMethod(transpiler));
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_1)
                {
                    codes.Insert(i, new CodeInstruction(OpCodes.Call, modifyAction));
                    break;
                }
            }
            return codes;
        }

        private static Action ModifyAndAddAction(Action action)
        {
            if (action is null)
            {
                action = new Action(RefreshCanvases);
            }
            else
            {
                action -= RefreshCanvases;
                action += RefreshCanvases;
            }
            return action;
        }

        private static void RefreshCanvases()
        {
            canvasUtility.RefreshAllCanvases(hudConfig);
            menuInstaller.RemoveButton();
            menuInstaller.AddButton();
        }
    }
}

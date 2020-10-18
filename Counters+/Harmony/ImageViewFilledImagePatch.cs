using HarmonyLib;
using HMUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace CountersPlus.Harmony
{
    [HarmonyPatch(typeof(ImageView))]
    [HarmonyPatch("GenerateFilledSprite")]
    internal static class ImageViewFilledImagePatch
    {
        // This is Beat Games' incorrect AddQuad method, which completely forgets about the curvedUIRadius parameter.
        private static MethodInfo incorrectQuadOverload = typeof(ImageView).GetMethod("AddQuad",
            BindingFlags.Static | BindingFlags.NonPublic,
            Type.DefaultBinder,
            new Type[] { typeof(VertexHelper), typeof(Vector3[]), typeof(Color32), typeof(Vector3[]) },
            Array.Empty<ParameterModifier>());

        // This is our custom AddQuad method, which correctly applies curvature to the created vertices.
        private static MethodInfo correctQuadOverload = SymbolExtensions.GetMethodInfo(() => AddQuad(null, null, new Color32(0, 0, 0, 0), null, 0));
        
        private static List<CodeInstruction> replacementCode = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_3), // We load the "curvedUIRadius" field from IL
            new CodeInstruction(OpCodes.Call, correctQuadOverload) // We then call our CORRECT AddQuad method.
        };

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if ((codes[i].operand as MethodInfo) == incorrectQuadOverload) // Did we find an instance of the old, incorrect method?
                {
                    codes.RemoveAt(i); // If so, throw that sucker out
                    codes.InsertRange(i, replacementCode); // And replace it with our good one, with curvedUIRadius included.
                }
            }
            return codes;
        }

        private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs, float curvedUIRadius)
        {
            int currentVertCount = vertexHelper.currentVertCount;
            Vector2 uv = new Vector2(curvedUIRadius, 0f);
            for (int i = 0; i < 4; i++)
            {
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i], Vector2.zero, uv, Vector2.zero, Vector3.zero, Vector4.zero);
            }
            vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }
    }
}

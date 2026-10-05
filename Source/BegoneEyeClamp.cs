using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(AnimalVisuals), "SetAnimalMaterialVariation")]
    public static class EyeColorFinalOverridePatch
    {
        static void Prefix(ref AnimalSkinVariationRuntime dataInst)
        {
            Color existingEyeColor = dataInst.patternColorEye;

            if (!dataInst.geneticsOn)
            {
                dataInst.geneticsOn = true;

                dataInst.patternColorA = Color.clear;
                dataInst.patternColorB = Color.clear;
                dataInst.patternColorSecondary = Color.clear;

                dataInst.patternsFeathers = new Vector3(0f, 1f, 0f);
                dataInst.baseOverride = 0f;
            }

            if (existingEyeColor.a > 0f || existingEyeColor != Color.clear)
            {
                dataInst.patternColorEye = existingEyeColor;
            }
        }
    }
}

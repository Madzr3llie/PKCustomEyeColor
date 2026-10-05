using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;
using VLib;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(AnimalSkinVariationRuntime), nameof(AnimalSkinVariationRuntime.Mix))]
    public static class EyeColorBreedingMixPatch
    {
        private const float MixChance = 0.20f;

        static void Postfix(
            ref AnimalSkinVariationRuntime __result,
            AnimalSkinVariationRuntime a,
            AnimalSkinVariationRuntime b,
            ref Unity.Mathematics.Random rand)
        {
            float roll = rand.NextFloat(0f, 1f);

            if (roll < MixChance)
            {
                float blendFactor = rand.NextFloat(0.20f, 0.80f);

                Color.RGBToHSV(
                    a.patternColorEye,
                    out float hA,
                    out float sA,
                    out float vA);

                Color.RGBToHSV(
                    b.patternColorEye,
                    out float hB,
                    out float sB,
                    out float vB);

                float blendedH =
                    Mathf.LerpAngle(
                        hA * 360f,
                        hB * 360f,
                        blendFactor) / 360f;

                if (blendedH < 0f)
                    blendedH += 1f;

                float blendedS =
                    Mathf.Lerp(sA, sB, blendFactor);

                float blendedV =
                    Mathf.Max(vA, vB);

                Color blendedEye =
                    Color.HSVToRGB(
                        blendedH,
                        blendedS,
                        blendedV,
                        true);

                __result.patternColorEye =
                    blendedEye;
            }
        }
    }
}

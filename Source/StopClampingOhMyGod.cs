using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(AnimalSkinVariationRuntime), nameof(AnimalSkinVariationRuntime.ClampValues))]
    public static class StopClampingOhMyGod
    {
        private static Color _tempEyeColor;

        static void Prefix(ref AnimalSkinVariationRuntime __instance)
        {
            _tempEyeColor = __instance.patternColorEye;
        }
        static void Postfix(ref AnimalSkinVariationRuntime __instance)
        {
            if (_tempEyeColor != default && _tempEyeColor != Color.clear)
            {
                __instance.patternColorEye = _tempEyeColor;
            }
            else if (__instance.patternColorEye == default)
            {
                __instance.patternColorEye = Color.white;
            }
        }
    }
}

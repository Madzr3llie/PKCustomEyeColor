using EyeColorSlider;
using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(NurseryMenuV2), "RandomizeAnimalSkinCustomization")]
    public static class EyeRandomizePatch
    {
        private static Color _preservedEyeColor;

        static bool Prefix(NurseryMenuV2 __instance)
        {
            if (EyeColorSliderState.EyeRandomizeButton != null &&
                UnityEngine.EventSystems.EventSystem.current != null)
            {
                var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

                if (selected == EyeColorSliderState.EyeRandomizeButton.gameObject)
                {
                    EyeColorSliderApplyPatch.RandomizeEyeColor(__instance);
                    return false;
                }
            }

            var t = Traverse.Create(__instance);
            VirtualAnimal virtualAnimal = (VirtualAnimal)t.Property("CurrentPreviewVirtualAnimal").GetValue();
            if (virtualAnimal != null)
            {
                _preservedEyeColor = virtualAnimal.variationRuntimeData.patternColorEye;
            }

            return true;
        }

        static void Postfix(NurseryMenuV2 __instance)
        {
            var t = Traverse.Create(__instance);
            VirtualAnimal virtualAnimal = (VirtualAnimal)t.Property("CurrentPreviewVirtualAnimal").GetValue();
            if (virtualAnimal != null && _preservedEyeColor != default && _preservedEyeColor != Color.clear)
            {
                var runtimeData = virtualAnimal.variationRuntimeData;
                runtimeData.patternColorEye = _preservedEyeColor;
                virtualAnimal.variationRuntimeData = runtimeData;
            }
        }
    }
}
using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(NurseryMenuV2), "SetCustomizationRect")]
    public static class EyeColorSliderRefreshPatch
    {
        static void Postfix(NurseryMenuV2 __instance)
        {
            PKSlider hueSlider = EyeColorSliderState.EyeHueSlider;
            PKSlider saturationSlider =
                EyeColorSliderState.EyeSaturationSlider;
            PKSlider valueSlider = EyeColorSliderState.EyeValueSlider;

            if (hueSlider == null ||
                saturationSlider == null ||
                valueSlider == null)
            {
                return;
            }

            var t = Traverse.Create(__instance);

            VirtualAnimal virtualAnimal = (VirtualAnimal)t
                .Property("CurrentPreviewVirtualAnimal")
                .GetValue();

            if (virtualAnimal == null)
                return;

            hueSlider.interactable = true;
            saturationSlider.interactable = true;
            valueSlider.interactable = true;

            if (EyeColorSliderState.EyeRandomizeButton != null)
            {
                EyeColorSliderState.EyeRandomizeButton.interactable =
                    virtualAnimal.SkinType == AnimalSkinType.Base;
            }

            int animalHash = virtualAnimal.GetHashCode();

            if (EyeColorSliderState.LastSelectedAnimalHash != animalHash)
            {
                EyeColorSliderState.LastSelectedAnimalHash = animalHash;

                Color currentEye =
                    virtualAnimal.variationRuntimeData.patternColorEye;

                float rawMax = Mathf.Max(
                    currentEye.r,
                    Mathf.Max(currentEye.g, currentEye.b));

                Color normalizedColor = rawMax > 0f
                    ? currentEye / rawMax
                    : Color.black;

                Color.RGBToHSV(
                    normalizedColor,
                    out float h,
                    out float s,
                    out _);

                if (s > 0.01f && rawMax > 0.01f)
                {
                    EyeColorSliderState.EyeHue = h;
                }

                float adjustedValue =
                    Mathf.Pow(rawMax / 1.5f, 1f / 1.5f);

                float minBrightness = 0.30f;

                float unscaledValue = Mathf.InverseLerp(
                    minBrightness,
                    1.0f,
                    adjustedValue);

                EyeColorSliderState.EyeSaturation = s;
                EyeColorSliderState.EyeValue =
                    Mathf.Clamp01(unscaledValue);
            }

            EyeColorSliderApplyPatch.SetSliderSilently(
                hueSlider,
                EyeColorSliderState.EyeHue);

            EyeColorSliderApplyPatch.SetSliderSilently(
                saturationSlider,
                EyeColorSliderState.EyeSaturation);

            EyeColorSliderApplyPatch.SetSliderSilently(
                valueSlider,
                EyeColorSliderState.EyeValue);

            EyeColorSliderApplyPatch.UpdateSliderHandleText(
                hueSlider,
                EyeColorSliderState.EyeHue);

            EyeColorSliderApplyPatch.UpdateSliderHandleText(
                saturationSlider,
                EyeColorSliderState.EyeSaturation);

            EyeColorSliderApplyPatch.UpdateSliderHandleText(
                valueSlider,
                EyeColorSliderState.EyeValue);
        }
    }
}
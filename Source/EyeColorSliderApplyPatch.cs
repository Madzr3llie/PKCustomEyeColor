using EyeColorSlider;
using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

public static class EyeColorSliderApplyPatch
{
    public static void ApplyEyeColor(NurseryMenuV2 menu)
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

        EyeColorSliderState.EyeHue =
            Mathf.Clamp01(hueSlider.Value);

        EyeColorSliderState.EyeSaturation =
            Mathf.Clamp01(saturationSlider.Value);

        EyeColorSliderState.EyeValue =
            Mathf.Clamp01(valueSlider.Value);

        UpdateSliderHandleText(
            hueSlider,
            EyeColorSliderState.EyeHue);

        UpdateSliderHandleText(
            saturationSlider,
            EyeColorSliderState.EyeSaturation);

        UpdateSliderHandleText(
            valueSlider,
            EyeColorSliderState.EyeValue);

        var t = Traverse.Create(menu);

        VirtualAnimal virtualAnimal = (VirtualAnimal)t
            .Property("CurrentPreviewVirtualAnimal")
            .GetValue();

        if (virtualAnimal == null)
            return;

        float adjustedValue = Mathf.Lerp(
            0.30f,
            1.0f,
            EyeColorSliderState.EyeValue);

        float mappedValue =
            Mathf.Pow(adjustedValue, 1.5f) * 1.5f;

        Color eyeColor = Color.HSVToRGB(
            EyeColorSliderState.EyeHue,
            EyeColorSliderState.EyeSaturation,
            Mathf.Clamp01(mappedValue));

        if (mappedValue > 1.0f)
        {
            eyeColor *= mappedValue;
        }

        var runtimeData = virtualAnimal.variationRuntimeData;
        runtimeData.patternColorEye = eyeColor;
        virtualAnimal.variationRuntimeData = runtimeData;

        Game.AnimalPreviewComponent.UpdateAnimalPreview(
            virtualAnimal,
            false);
    }

    public static void RandomizeEyeColor(NurseryMenuV2 menu)
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

        float h = UnityEngine.Random.value;
        float s = UnityEngine.Random.value;
        float v = UnityEngine.Random.Range(0.35f, 1.0f);

        EyeColorSliderState.EyeHue = h;
        EyeColorSliderState.EyeSaturation = s;
        EyeColorSliderState.EyeValue = v;

        SetSliderSilently(hueSlider, h);
        SetSliderSilently(saturationSlider, s);
        SetSliderSilently(valueSlider, v);

        ApplyEyeColor(menu);
    }

    public static void SetSliderSilently(PKSlider slider, float value)
    {
        if (slider == null)
            return;

        value = Mathf.Clamp(
            value,
            slider.minValue,
            slider.maxValue);

        if (slider.XYValue.x != slider.minValue ||
            !Mathf.Approximately(slider.Value, value))
        {
            slider.Set(
                new Vector2(slider.minValue, value),
                false);
        }
    }

    public static void UpdateSliderHandleText(PKSlider slider, float val)
    {
        if (slider == null)
            return;

        slider.SetText(val.ToString("F2"), false);
    }
}
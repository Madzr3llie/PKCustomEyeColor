using EyeColorSlider;
using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

[HarmonyPatch(typeof(NurseryMenuV2), "UpdateAnimalPreview")]
public static class EyeColorSliderApplyPatch
{
    public static void ApplyEyeColor(NurseryMenuV2 menu)
    {
        PKSlider hueSlider = EyeColorSliderState.EyeHueSlider;
        PKSlider saturationSlider = EyeColorSliderState.EyeSaturationSlider;
        PKSlider valueSlider = EyeColorSliderState.EyeValueSlider;

        if (hueSlider == null || saturationSlider == null || valueSlider == null)
            return;

        EyeColorSliderState.EyeHue = Mathf.Clamp01(hueSlider.Value);
        EyeColorSliderState.EyeSaturation = Mathf.Clamp01(saturationSlider.Value);
        EyeColorSliderState.EyeValue = Mathf.Clamp01(valueSlider.Value);

        UpdateSliderHandleText(hueSlider, EyeColorSliderState.EyeHue);
        UpdateSliderHandleText(saturationSlider, EyeColorSliderState.EyeSaturation);
        UpdateSliderHandleText(valueSlider, EyeColorSliderState.EyeValue);

        var t = Traverse.Create(menu);
        VirtualAnimal virtualAnimal = (VirtualAnimal)t.Property("CurrentPreviewVirtualAnimal").GetValue();

        if (virtualAnimal == null) return;

        float minBrightness = 0.30f;
        float adjustedValue = Mathf.Lerp(minBrightness, 1.0f, EyeColorSliderState.EyeValue);

        float mappedValue = Mathf.Pow(adjustedValue, 1.5f) * 1.5f;

        Color eyeColor = Color.HSVToRGB(
            EyeColorSliderState.EyeHue,
            EyeColorSliderState.EyeSaturation,
            Mathf.Clamp01(mappedValue)
        );

        if (mappedValue > 1.0f)
        {
            eyeColor *= mappedValue;
        }

        var runtimeData = virtualAnimal.variationRuntimeData;
        runtimeData.patternColorEye = eyeColor;
        virtualAnimal.variationRuntimeData = runtimeData;
    }

    public static void RandomizeEyeColor(NurseryMenuV2 menu)
    {
        PKSlider hueSlider = EyeColorSliderState.EyeHueSlider;
        PKSlider saturationSlider = EyeColorSliderState.EyeSaturationSlider;
        PKSlider valueSlider = EyeColorSliderState.EyeValueSlider;

        if (hueSlider == null || saturationSlider == null || valueSlider == null)
            return;

        float h = UnityEngine.Random.value;
        float s = UnityEngine.Random.value;
        float v = UnityEngine.Random.Range(0.35f, 1.0f);

        EyeColorSliderState.EyeHue = h;
        EyeColorSliderState.EyeSaturation = s;
        EyeColorSliderState.EyeValue = v;

        hueSlider.Value = h;
        saturationSlider.Value = s;
        valueSlider.Value = v;

        UpdateSliderHandleText(hueSlider, h);
        UpdateSliderHandleText(saturationSlider, s);
        UpdateSliderHandleText(valueSlider, v);

        var t = Traverse.Create(menu);
        VirtualAnimal virtualAnimal = (VirtualAnimal)t.Property("CurrentPreviewVirtualAnimal").GetValue();

        if (virtualAnimal == null) return;

        float mappedValue = Mathf.Pow(v, 1.5f) * 1.5f;
        Color eyeColor = Color.HSVToRGB(h, s, Mathf.Clamp01(mappedValue));
        if (mappedValue > 1.0f) eyeColor *= mappedValue;

        var runtimeData = virtualAnimal.variationRuntimeData;
        runtimeData.patternColorEye = eyeColor;
        virtualAnimal.variationRuntimeData = runtimeData;

        Game.AnimalPreviewComponent.UpdateAnimalPreview(virtualAnimal, false);
    }

    public static void UpdateSliderHandleText(PKSlider slider, float val)
    {
        if (slider == null) return;

        string formatted = val.ToString("F2");

        var traverse = HarmonyLib.Traverse.Create(slider);
        TMPro.TextMeshProUGUI handleText = traverse.Field("handleText").GetValue() as TMPro.TextMeshProUGUI;

        if (handleText == null)
        {
            Transform handle = slider.transform.Find("Handle Area/Handle") ?? slider.transform.Find("Handle");
            if (handle != null)
            {
                Component[] tmps = handle.GetComponentsInChildren(typeof(TMPro.TextMeshProUGUI), true);
                if (tmps.Length > 0 && tmps[0] != null)
                {
                    handleText = tmps[0] as TMPro.TextMeshProUGUI;
                    traverse.Field("handleText").SetValue(handleText);
                }
            }
        }

        if (handleText != null)
        {
            handleText.gameObject.SetActive(true);
            handleText.text = formatted;
        }

        slider.SetText(formatted, false);
    }
}

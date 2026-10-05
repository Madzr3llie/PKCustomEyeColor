using HarmonyLib;
using PrehistoricKingdom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EyeColorSlider
{
    public static class EyeColorSliderState
    {
        public static PKSlider EyeSlider;
        public static PKSlider EyeHueSlider;
        public static PKSlider EyeSaturationSlider;
        public static PKSlider EyeValueSlider;
        public static Button EyeRandomizeButton;

        public static float EyeHue = 0f;
        public static float EyeSaturation = 1f;
        public static float EyeValue = 1f;

        public static int LastSelectedAnimalHash = 0;
    }

    [HarmonyPatch(typeof(NurseryMenuV2), "Init")]
    public static class EyeColorSliderInitPatch
    {
        static void Postfix(NurseryMenuV2 __instance)
        {
            var t = Traverse.Create(__instance);

            PKSlider hueSlider = (PKSlider)t.Field("hueSlider").GetValue();

            Transform colorVariation = hueSlider.transform.parent.parent;

            GameObject cloneSection = UnityEngine.Object.Instantiate(
                colorVariation.gameObject,
                colorVariation.parent);

            cloneSection.name = "EyeColorVariation";

            cloneSection.transform.SetSiblingIndex(colorVariation.GetSiblingIndex() + 1);

            Toggle customizeSkinToggle = (Toggle)t.Field("customizeSkinToggle").GetValue();

            cloneSection.SetActive(customizeSkinToggle.isOn);

            customizeSkinToggle.onValueChanged.AddListener(isOn =>
            {
                cloneSection.SetActive(isOn);
            });

            Component[] fitters = cloneSection.GetComponentsInChildren(typeof(ContentSizeFitter), true);
            for (int i = 0; i < fitters.Length; i++)
            {
                UnityEngine.Object.Destroy(fitters[i]);
            }

            Component[] layouts = cloneSection.GetComponentsInChildren(typeof(LayoutElement), true);
            for (int i = 0; i < layouts.Length; i++)
            {
                UnityEngine.Object.Destroy(layouts[i]);
            }

            Component[] groups = cloneSection.GetComponentsInChildren(typeof(HorizontalOrVerticalLayoutGroup), true);
            for (int i = 0; i < groups.Length; i++)
            {
                UnityEngine.Object.Destroy(groups[i]);
            }

            Transform pattern = cloneSection.transform.Find("Pattern");

            if (pattern != null)
            {
                RectTransform patternRect = (RectTransform)pattern.GetComponent(typeof(RectTransform));
                float patternHeight = patternRect.rect.height;
                int patternIndex = pattern.GetSiblingIndex();

                for (int i = patternIndex + 1; i < cloneSection.transform.childCount; i++)
                {
                    Transform child = cloneSection.transform.GetChild(i);
                    RectTransform row = (RectTransform)child.GetComponent(typeof(RectTransform));

                    if (row != null)
                        row.anchoredPosition += Vector2.up * patternHeight;
                }

                RectTransform cloneRect = (RectTransform)cloneSection.GetComponent(typeof(RectTransform));
                cloneRect.sizeDelta = new Vector2(
                    cloneRect.sizeDelta.x,
                    cloneRect.sizeDelta.y - patternHeight);

                UnityEngine.Object.Destroy(pattern.gameObject);
            }

            Transform headerTextTransform = cloneSection.transform.Find("Header/Text") ?? cloneSection.transform.Find("Title");
            TextMeshProUGUI titleText = headerTextTransform != null ? headerTextTransform.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI : null;
            if (titleText != null)
            {
                titleText.text = "Eye Color";
            }

            Component[] sliderComponents = cloneSection.GetComponentsInChildren(typeof(PKSlider), true);
            foreach (Component comp in sliderComponents)
            {
                PKSlider slider = (PKSlider)comp;
                if (slider.transform.parent.name == "Hue")
                {
                    EyeColorSliderState.EyeSlider = slider;
                    EyeColorSliderState.EyeHueSlider = slider;
                }

                if (slider.transform.parent.name == "Saturation")
                    EyeColorSliderState.EyeSaturationSlider = slider;

                if (slider.transform.parent.name == "Value")
                    EyeColorSliderState.EyeValueSlider = slider;
            }

            Component[] buttonComponents = cloneSection.GetComponentsInChildren(typeof(Button), true);
            foreach (Component comp in buttonComponents)
            {
                Button button = (Button)comp;
                if (button.name == "Randomize")
                    EyeColorSliderState.EyeRandomizeButton = button;
            }

            SetupSlider(EyeColorSliderState.EyeHueSlider);
            SetupSlider(EyeColorSliderState.EyeSaturationSlider);
            SetupSlider(EyeColorSliderState.EyeValueSlider);

            if (EyeColorSliderState.EyeRandomizeButton != null)
            {
                EyeColorSliderState.EyeRandomizeButton.onClick = new Button.ButtonClickedEvent();
                EyeColorSliderState.EyeRandomizeButton.onClick.AddListener(() => { EyeColorSliderApplyPatch.RandomizeEyeColor(__instance); });
            }

            EyeColorSliderState.EyeHueSlider.OnValueChanged.AddListener(() =>
            {
                EyeColorSliderState.EyeHueSlider.SetText(EyeColorSliderState.EyeHueSlider.Value.ToString("F2"), false);
                EyeColorSliderApplyPatch.ApplyEyeColor(__instance);
            });

            EyeColorSliderState.EyeSaturationSlider.OnValueChanged.AddListener(() =>
            {
                EyeColorSliderState.EyeSaturationSlider.SetText(EyeColorSliderState.EyeSaturationSlider.Value.ToString("F2"), false);
                EyeColorSliderApplyPatch.ApplyEyeColor(__instance);
            });

            EyeColorSliderState.EyeValueSlider.OnValueChanged.AddListener(() =>
            {
                EyeColorSliderState.EyeValueSlider.SetText(EyeColorSliderState.EyeValueSlider.Value.ToString("F2"), false);
                EyeColorSliderApplyPatch.ApplyEyeColor(__instance);
            });

            RectTransform parentRect = (RectTransform)colorVariation.parent.GetComponent(typeof(RectTransform));
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
            Canvas.ForceUpdateCanvases();
        }

        private static void SetupSlider(PKSlider slider)
        {
            if (slider == null) return;

            slider.OnValueChanged.RemoveAllListeners();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.minMaxHandles = false;

            Transform handleTransform = slider.transform.Find("Handle Area/Handle") ?? slider.transform.Find("Handle");
            if (handleTransform != null)
            {
                Component[] tmps = handleTransform.GetComponentsInChildren(typeof(TextMeshProUGUI), true);
                if (tmps.Length > 0 && tmps[0] != null)
                {
                    Traverse.Create(slider).Field("handleText").SetValue(tmps[0]);
                    ((TextMeshProUGUI)tmps[0]).gameObject.SetActive(true);
                }
            }

            slider.SetText(slider.Value.ToString("F2"), true);
        }
    }
}

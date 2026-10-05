using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(AnimalPreviewComponent))]
    public static class NurseryPreviewZoomPatch
    {
        private const float DefaultZoom = 4f;
        private const float ZoomSpeed = 0.5f;
        private const float MinZoom = 1f;
        private const float MaxZoom = 4f;

        private static float currentZoom = DefaultZoom;

        [HarmonyPatch("CreateAnimalPreview")]
        [HarmonyPostfix]
        static void ResetZoom()
        {
            currentZoom = DefaultZoom;
        }

        [HarmonyPatch("SetPreviewEnvironment")]
        [HarmonyPrefix]
        static void ApplyZoom(AnimalPreviewComponent __instance)
        {
            if (__instance.currentAnimalPreview == null)
                return;

            NurseryMenuV2 nursery =
                UnityEngine.Object.FindObjectOfType<NurseryMenuV2>();

            if (nursery != null && nursery.IsGalleryOpen)
                return;

            float scroll = Input.GetKey(KeyCode.LeftShift) ? Input.mouseScrollDelta.y : 0f;

            if (Mathf.Abs(scroll) > 0.001f)
            {
                currentZoom -= scroll * ZoomSpeed;

                currentZoom = Mathf.Clamp(
                    currentZoom,
                    MinZoom,
                    MaxZoom);
            }

            __instance.cameraDistMult = currentZoom;
        }
    }
}

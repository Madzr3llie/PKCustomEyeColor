using HarmonyLib;
using PrehistoricKingdom;
using UnityEngine;

namespace EyeColorSlider
{
    [HarmonyPatch(typeof(AnimalPreviewComponent))]
    public static class NurseryPreviewPanPatch
    {
        private const float PanSpeed = 0.01f;

        private static Vector2 panOffset = Vector2.zero;

        [HarmonyPatch("CreateAnimalPreview")]
        [HarmonyPostfix]
        static void ResetPan()
        {
            panOffset = Vector2.zero;
        }

        [HarmonyPatch("SetPreviewEnvironment")]
        [HarmonyPostfix]
        static void ApplyPan()
        {
            if (Game.AnimalPreviewCamera == null)
                return;

            Transform cameraTransform =
                Game.AnimalPreviewCamera.transform;

            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                panOffset.x += mouseX * PanSpeed;
                panOffset.y += mouseY * PanSpeed;
            }

            cameraTransform.localPosition += new Vector3(
                panOffset.x,
                panOffset.y,
                0f);
        }
    }
}

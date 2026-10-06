using MelonLoader;

[assembly: MelonInfo(typeof(EyeColorSlider.Core), "EyeColorSlider", "1.0.1", "maddi", null)]
[assembly: MelonGame("Blue Meridian", "Prehistoric Kingdom")]

namespace EyeColorSlider
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}
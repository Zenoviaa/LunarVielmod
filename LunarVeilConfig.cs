using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Stellamod
{
	public class LunarVeilServerConfig : ModConfig
	{
		// ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
		// ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviors
		public override ConfigScope Mode => ConfigScope.ServerSide;

		// The things in brackets are known as "Attributes".





	}

    public class LunarVeilClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Visual")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.                                       // [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.
        [DefaultValue(true)] // This sets the configs default value.
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool VanillaTexturesToggle;

        [DefaultValue(true)] // This sets the configs default value.// Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        [ReloadRequired]
        public bool VanillaUIRespritesToggle;
        
        // To see the implementation of this option, see ExampleWings.cs

        [DefaultValue(true)] // This sets the configs default value. // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool VanillaParticlesToggle;

        [DefaultValue(true)]
        public bool VanillaBiomesPaletteShadersToggle;

        [DefaultValue(true)]
        public bool LiquidsToggle;

        [DefaultValue(true)] // This sets the configs default value. // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool SkiesToggle;

        [DefaultValue(true)] // This sets the configs default value.// Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool ParticlesToggle;

        [Header("Effects")]
        [DefaultValue(true)]
        public bool ShakeToggle;

        [Range(0f, 100f)]
        public float CameraSmoothness = 100;

        [DefaultValue(false)]
        public bool LowDetailShadersToggle;

        [Header("UI")]
        [Range(0f, 100f)]
        public float EnchantMenuX = 50;
        [Range(0f, 100f)]
        public float EnchantMenuY = 50;

        [Range(0f, 100f)]
        public float StaminaMeterX = 50;
        [Range(0f, 100f)]
        public float StaminaMeterY = 3;
        [Range(0f, 100f)]
        public float DashMeterX = 50;
        [Range(0f, 100f)]
        public float DashMeterY = 50;
    }
}
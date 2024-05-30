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

        [Header("ReplaceVanillaTextures")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.                                       // [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.
        [DefaultValue(true)] // This sets the configs default value.
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool VanillaTexturesToggle; // To see the implementation of this option, see ExampleWings.cs

        [Header("BiomeParticles")]
        [DefaultValue(true)] // This sets the configs default value. // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool VanillaParticlesToggle;

        [Header("ModdedBiomeParticles")]

        [DefaultValue(true)] // This sets the configs default value.// Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool ParticlesToggle;


        [Header("Screenshake")]
        [DefaultValue(true)]
        public bool ShakeToggle;
    }
}
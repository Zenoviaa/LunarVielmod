using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
	public class IshtarBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/IshtarBiomeUnderground1");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/IshtarBiomeUnderground1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/IshtarBiomeUnderground1");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/IshtarBiomeUnderground1");
		}
	}
}
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class MorrowUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/MorrowBiomeUnderground0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/MorrowBiomeUnderground1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/MorrowBiomeUnderground2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/MorrowBiomeUnderground3");
		}
	}
}
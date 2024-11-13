using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Effects
{
    internal class SpecialEffectsPlayer : ModPlayer
    {
        private bool _init;
        private Color[] _abyssPalette;
        public bool hasSpiritPendant;
        public bool hasSunGlyph;
        private Color[] MonotonePalette(Color startingColor, Color endingColor, float steps)
        {
            var palette = new Color[(int)steps];
            for(int i = 0; i < steps; i++)
            {
                palette[i] = Color.Lerp(startingColor, endingColor, (float)i / steps);
            }
            return palette;
        }
        private Color[] RandomPalette(float steps)
        {
            var palette = new Color[(int)steps];
            for (int i = 0; i < steps; i++)
            {
                palette[i] = new Color(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
            }
            return palette;
        }
        private void LoadPalettes()
        {
            _abyssPalette = PalFileImporter.ReadPalette("Effects/Abyss");
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSpiritPendant = false;
            hasSunGlyph = false;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (!_init)
            {
                LoadPalettes();
                _init = true;
            }
            PaletteShaderSystem paletteShaderSystem = ModContent.GetInstance<PaletteShaderSystem>();
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            if (Player.InModBiome<AurelusBiome>() || Player.InModBiome<AbyssBiome>())
            {
                paletteShaderSystem.UsePalette(_abyssPalette);
                float darkness = 2f;
                if (hasSpiritPendant)
                {
                    darkness -= 0.5f;
                }
                if (hasSunGlyph)
                {
                    darkness -= 0.5f;
                }

                screenShaderSystem.VignetteScreen(darkness);
            }
            else
            {

                if (paletteShaderSystem.DisablePalette(_abyssPalette))
                {
                    screenShaderSystem.UnVignetteScreen();
                }
              
            }
        }
    }
}

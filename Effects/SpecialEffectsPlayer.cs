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
        private Color[] _alcadPalette;
        private Color[] _underworldPalette;
        private Color[] _desertPalette;
        private Color[] _witchTownPalette;
        private Color[] _rustyPalette;
        private Color[] _bloodyPalette;
        private Color[] _dungeonPalette;
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
            _alcadPalette = PalFileImporter.ReadPalette("Effects/RoyalCapital");
            _underworldPalette = PalFileImporter.ReadPalette("Effects/Hell");
            _desertPalette = PalFileImporter.ReadPalette("Effects/maggot24");
            _witchTownPalette = PalFileImporter.ReadPalette("Effects/Witchtown");
            _rustyPalette = PalFileImporter.ReadPalette("Effects/Rusty");
            _bloodyPalette = PalFileImporter.ReadPalette("Effects/bloodmoon21");
            _dungeonPalette = PalFileImporter.ReadPalette("Effects/Dungeon");
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

            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();

            if (Player.InModBiome<AurelusBiome>() ||
                Player.InModBiome<AbyssBiome>() || 
                Player.InModBiome<EngineerShop>() || 
                Player.InModBiome<IshtarBiome>())
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

            if (Player.ZoneDungeon && clientConfig.VanillaBiomesPaletteShadersToggle)
            {
                paletteShaderSystem.UsePalette(_dungeonPalette);
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

            if ((Player.ZoneUnderworldHeight && clientConfig.VanillaBiomesPaletteShadersToggle) || Player.InModBiome<CindersparkBiome>())
            {
                paletteShaderSystem.UsePalette(_underworldPalette);
                screenShaderSystem.VignetteScreen(1f);
            }
            else
            {

                if (paletteShaderSystem.DisablePalette(_underworldPalette))
                {
                    screenShaderSystem.UnVignetteScreen();
                }
            }

            if (Player.InModBiome<AlcadziaBiome>())
            {
                paletteShaderSystem.UsePalette(_alcadPalette);
                screenShaderSystem.VignetteScreen(1f);
            }
            else
            {

                if (paletteShaderSystem.DisablePalette(_alcadPalette))
                {
                    screenShaderSystem.UnVignetteScreen();
                }
            }

            if (Player.InModBiome<GovheilCastle>() || 
                Player.InModBiome<AcidBiome>())
            {
                paletteShaderSystem.UsePalette(_witchTownPalette);
            }
            else
            {

                if (paletteShaderSystem.DisablePalette(_witchTownPalette))
                {

                }
            }

            
            if (Player.InModBiome<BloodCathedral>() && !Main.dayTime)
            {
                paletteShaderSystem.UsePalette(_bloodyPalette);
            }
            else
            {

                if (paletteShaderSystem.DisablePalette(_bloodyPalette))
                {

                }
            }

            if (Player.ZoneDesert && clientConfig.VanillaBiomesPaletteShadersToggle)
            {
                paletteShaderSystem.UsePalette(_desertPalette);
                screenShaderSystem.VignetteScreen(1f);
            }
            else
            {
                if (paletteShaderSystem.DisablePalette(_desertPalette))
                {
                    screenShaderSystem.UnVignetteScreen();
                }
            }
        }  
    }
}

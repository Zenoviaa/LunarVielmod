using Microsoft.Xna.Framework;
using Stellamod.Effects;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Lights
{
    internal class SpecialEffectsPlayer : ModPlayer
    {
        private bool _init;
        private float _vignetteOpacity;
        private float _vignetteStrength;

        private float _targetVignetteStrength;
        private float _targetVignetteOpacity;

        private Color[] _abyssPalette;
        private Color[] _alcadPalette;
        private Color[] _underworldPalette;
        private Color[] _desertPalette;
        private Color[] _witchTownPalette;
        private Color[] _rustyPalette;
        private Color[] _bloodyPalette;
        private Color[] _dungeonPalette;

        private MyPlayer MyPlayer => Player.GetModPlayer<MyPlayer>();

        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        private string DarknessVignette => "LunarVeil:DarknessVignette";

        public bool hasSpiritPendant;
        public bool hasSunGlyph;
        public float darkness;
        public float darknessCurve;
        private Color[] MonotonePalette(Color startingColor, Color endingColor, float steps)
        {
            var palette = new Color[(int)steps];
            for (int i = 0; i < steps; i++)
            {
                palette[i] = Color.Lerp(startingColor, endingColor, i / steps);
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
            string rootDirectory = "Common/Lights/Palettes";
            _abyssPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Abyss");
            _alcadPalette = PalFileImporter.ReadPalette($"{rootDirectory}/RoyalCapital");
            _underworldPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Hell");
            _desertPalette = PalFileImporter.ReadPalette($"{rootDirectory}/maggot24");
            _witchTownPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Witchtown");
            _rustyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Rusty");
            _bloodyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/bloodmoon21");
            _dungeonPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Dungeon");
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSpiritPendant = false;
            hasSunGlyph = false;
            darkness = 0;
            darknessCurve = 0f;
            _targetVignetteOpacity = 1f;
        }

        private void TogglePaletteShader(string name, bool isActive)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            if (isActive)
            {
                if (!FilterManager[name].IsActive())
                {
                    FilterManager.Activate(name);
                }
            } else if (!isActive)
            {
                if (FilterManager[name].IsActive())
                {
                    FilterManager.Deactivate(name);
                }
            }
        }

        private void SpecialBiomeEffects()
        {
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            bool abyssPaletteActive = MyPlayer.ZoneAbyss || MyPlayer.ZoneAurelus || MyPlayer.ZoneMechanics || MyPlayer.ZoneIshtar;
            if (abyssPaletteActive)
            {
                darkness += 2;
            }
            TogglePaletteShader("LunarVeil:PaletteAbyss", abyssPaletteActive);

            bool hellPaletteActive = ((clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneUnderworldHeight)
                || MyPlayer.ZoneCinder || MyPlayer.ZoneDrakonic);
            if (hellPaletteActive)
            {
                darkness += 1;
            }
            TogglePaletteShader("LunarVeil:PaletteHell", hellPaletteActive);

            bool royalCapitalPaletteActive = MyPlayer.ZoneAlcadzia;
            TogglePaletteShader("LunarVeil:PaletteRoyalCapital", royalCapitalPaletteActive);

            bool dungeonPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDungeon;
            TogglePaletteShader("LunarVeil:PaletteDungeon", dungeonPaletteActive);

            bool desertPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDesert;
            TogglePaletteShader("LunarVeil:PaletteDesert", desertPaletteActive);

            bool bloodPaletteActive = MyPlayer.ZoneBloodCathedral && !Main.dayTime;
            TogglePaletteShader("LunarVeil:PaletteBloodCathedral", bloodPaletteActive);

            CalculateDarkness();
            TogglePaletteShader("LunarVeil:DarknessVignette", darkness != 0);

            var shaderData = FilterManager["LunarVeil:DarknessCurve"].GetShader();
            shaderData.UseProgress(darknessCurve);
            TogglePaletteShader("LunarVeil:DarknessCurve", darknessCurve != 0);
        }
        
        private void CalculateDarkness()
        {
            if (hasSpiritPendant)
            {
                darkness -= 0.5f;
            }
            if (hasSunGlyph)
            {
                darkness -= 0.5f;
            }
            if (darkness <= 0)
            {
                darkness = 0;
            }
            _targetVignetteStrength = darkness;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            //Darkness
            if (!_init)
            {
                LoadPalettes();
                _init = true;
            }

            SpecialBiomeEffects();
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            bool useVignette = darkness != 0;
            if (useVignette)
            {
                if (!FilterManager[DarknessVignette].IsActive())
                {
                    FilterManager.Activate(DarknessVignette);
                }

                _vignetteStrength = MathHelper.Lerp(_vignetteStrength, _targetVignetteStrength, 0.1f);
                _vignetteOpacity = MathHelper.Lerp(_vignetteOpacity, _targetVignetteOpacity, 0.1f);
                var shaderData = FilterManager[DarknessVignette].GetShader();
                shaderData.UseProgress(_vignetteStrength);
                shaderData.UseOpacity(_vignetteOpacity);
            }
            else
            {
                if (_vignetteStrength != 0)
                {
                    _vignetteOpacity = MathHelper.Lerp(_targetVignetteOpacity, 0, 0.1f);
                    _vignetteStrength = MathHelper.Lerp(_vignetteStrength, 0, 0.1f);
                    var shaderData = FilterManager[DarknessVignette].GetShader();
                    shaderData.UseProgress(_vignetteStrength);
                    shaderData.UseOpacity(_vignetteOpacity);
                }
                else
                {
                    if (FilterManager[DarknessVignette].IsActive())
                    {
                        FilterManager.Deactivate(DarknessVignette);
                    }
                }
            }
        }
    }
}

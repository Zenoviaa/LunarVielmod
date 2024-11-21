using Microsoft.Xna.Framework;
using Stellamod.Effects;
using Stellamod.Helpers;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
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
        private float _blurLerp;

        private Color[] _abyssPalette;
        private Color[] _alcadPalette;
        private Color[] _underworldPalette;
        private Color[] _desertPalette;
        private Color[] _witchTownPalette;
        private Color[] _rustyPalette;
        private Color[] _bloodyPalette;
        private Color[] _dungeonPalette;
        private Color[] _desertTopPalette;

        private MyPlayer MyPlayer => Player.GetModPlayer<MyPlayer>();

        private FilterManager FilterManager => Terraria.Graphics.Effects.Filters.Scene;

        private string DarknessVignette => "LunarVeil:DarknessVignette";

        public bool hasSpiritPendant;
        public bool hasSunGlyph;
        public float darkness;
        public float darknessCurve;
        public float whiteCurve;
        public float blackCurve;
        public float blurStrength;

        public Vector2 rippleCenter;
        public float rippleCount;
        public float rippleRadius;
        public float rippleSize;
        public float rippleSpeed;
        public float rippleDistortStrength;
        public float rippleTimer;

        //Progress Variables
        public float abyssPaletteProgress;
        public float hellPaletteProgress;
        public float dungeonPaletteProgress;
        public float royalPaletteProgress;
        public float desertPaletteProgress;
        public float bloodCathedralPaletteProgress;
        public float darknessCurveProgress = 1f;
        public float desertTopPaletteProgress;
        public float rustyPaletteProgress;
        private void LoadPalettes()
        {
            string rootDirectory = "Common/Lights/Palettes";
            _abyssPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Abyss");
            _alcadPalette = PalFileImporter.ReadPalette($"{rootDirectory}/RoyalCapital");
            _underworldPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Hell");
            _desertPalette = PalFileImporter.ReadPalette($"{rootDirectory}/maggot24");
            _desertTopPalette = PalFileImporter.ReadPalette($"{rootDirectory}/DesertTop");
            _witchTownPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Witchtown");
            _rustyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Rusty");
            _bloodyPalette = PalFileImporter.ReadPalette($"{rootDirectory}/BloodHound");
            _dungeonPalette = PalFileImporter.ReadPalette($"{rootDirectory}/Dungeon");
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSpiritPendant = false;
            hasSunGlyph = false;
            darkness = 0;
            darknessCurve = 0.79f;
           // blurStrength = 0;


            //Curve based
            float progress = (float)(Main.LocalPlayer.position.ToTileCoordinates().Y - Main.worldSurface) / 1000;
            progress = MathHelper.Clamp(progress, 0, 1);
            darknessCurve = MathHelper.Lerp(0f, darknessCurve, progress * darknessCurveProgress);

            whiteCurve = 0f;
            blackCurve = 1f;

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
            ScreenShaderData screenShaderData;
            bool abyssPaletteActive = MyPlayer.ZoneAbyss || MyPlayer.ZoneAurelus || MyPlayer.ZoneMechanics || MyPlayer.ZoneIshtar;
            if (abyssPaletteActive)
            {
                darkness += 2;
            }

            float speed = 0.05f;
            if (abyssPaletteActive)
            {
                abyssPaletteProgress += speed;
            }
            else
            {
                abyssPaletteProgress -= speed;
            }
            abyssPaletteProgress = MathHelper.Clamp(abyssPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteAbyss"].GetShader();
            screenShaderData.UseProgress(abyssPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteAbyss", abyssPaletteProgress != 0);

            bool rustyPaletteActive = (MyPlayer.ZoneGovheil || MyPlayer.ZoneAcid);
            if (rustyPaletteActive)
            {
                rustyPaletteProgress += speed;
            }
            else
            {
                rustyPaletteProgress -= speed;
            }
            rustyPaletteProgress = MathHelper.Clamp(rustyPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteVirulent"].GetShader();
            screenShaderData.UseProgress(rustyPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteVirulent", rustyPaletteProgress != 0);

            bool hellPaletteActive = ((clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneUnderworldHeight)
                || MyPlayer.ZoneCinder || MyPlayer.ZoneDrakonic);
            if (hellPaletteActive)
            {
                darkness += 1;
            }
            if (hellPaletteActive)
            {
                hellPaletteProgress += speed;
            }
            else
            {
                hellPaletteProgress -= speed;
            }
            hellPaletteProgress = MathHelper.Clamp(hellPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteHell"].GetShader();
            screenShaderData.UseProgress(hellPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteHell", hellPaletteProgress != 0);

            bool royalCapitalPaletteActive = MyPlayer.ZoneAlcadzia;
            if (royalCapitalPaletteActive)
            {
                royalPaletteProgress += speed;
            }
            else
            {
                royalPaletteProgress -= speed;
            }
            royalPaletteProgress = MathHelper.Clamp(royalPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteRoyalCapital"].GetShader();
            screenShaderData.UseProgress(royalPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteRoyalCapital", royalPaletteProgress != 0);

            bool dungeonPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDungeon;
            if (dungeonPaletteActive)
            {
                dungeonPaletteProgress += speed;
            }
            else
            {
                dungeonPaletteProgress -= speed;
            }
            dungeonPaletteProgress = MathHelper.Clamp(dungeonPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteDungeon"].GetShader();
            screenShaderData.UseProgress(dungeonPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteDungeon", dungeonPaletteProgress != 0);

            bool desertPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle
                && (Player.ZoneDesert || Player.GetModPlayer<MyPlayer>().ZoneAshotiTemple)
                && !(Player.ZoneCrimson || Player.ZoneCorrupt)
                && Player.ZoneUndergroundDesert;
            if (desertPaletteActive)
            {
                desertPaletteProgress += speed;
            }
            else
            {
                desertPaletteProgress -= speed;
            }
            desertPaletteProgress = MathHelper.Clamp(desertPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteDesert"].GetShader();
            screenShaderData.UseProgress(desertPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteDesert", desertPaletteProgress != 0);

            bool desertTopPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle
                 && (Player.ZoneDesert || Player.GetModPlayer<MyPlayer>().ZoneAshotiTemple || Player.GetModPlayer<MyPlayer>().ZoneColloseum)
                 && !(Player.ZoneCrimson || Player.ZoneCorrupt)
                 && !Player.ZoneUndergroundDesert;
            if (desertTopPaletteActive)
            {
                desertTopPaletteProgress += speed;
            }
            else
            {
                desertTopPaletteProgress -= speed;
            }
            desertTopPaletteProgress = MathHelper.Clamp(desertTopPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteDesertTop"].GetShader();
            screenShaderData.UseProgress(desertTopPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteDesertTop", desertTopPaletteProgress != 0);


            bool bloodPaletteActive = MyPlayer.ZoneBloodCathedral && !Main.dayTime;
            if (bloodPaletteActive)
            {
                bloodCathedralPaletteProgress += speed;
            }
            else
            {
                bloodCathedralPaletteProgress -= speed;
            }
            bloodCathedralPaletteProgress = MathHelper.Clamp(bloodCathedralPaletteProgress, 0f, 1f);
            screenShaderData = FilterManager["LunarVeil:PaletteBloodHound"].GetShader();
            screenShaderData.UseProgress(bloodCathedralPaletteProgress);
            TogglePaletteShader("LunarVeil:PaletteBloodHound", bloodCathedralPaletteProgress != 0);

            CalculateDarkness();
            TogglePaletteShader("LunarVeil:DarknessVignette", darkness != 0);



            bool evilAreaActive = Player.ZoneCrimson || Player.ZoneCorrupt;
            if (evilAreaActive && darknessCurve < 0.5f)
            {
                darknessCurve = 0.5f;
            }

            screenShaderData = FilterManager["LunarVeil:DarknessCurve"].GetShader();
            screenShaderData.UseProgress(darknessCurve);
            screenShaderData.Shader.Parameters["blackCurve"].SetValue(blackCurve);
            screenShaderData.Shader.Parameters["whiteCurve"].SetValue(whiteCurve);
            TogglePaletteShader("LunarVeil:DarknessCurve", darknessCurve != 0);


            if (hellPaletteActive || desertPaletteActive)
            {
                darknessCurveProgress -= speed;
            }
            else
            {
                darknessCurveProgress += speed;
            }
            darknessCurveProgress = MathHelper.Clamp(darknessCurveProgress, 0f, 1f);

            blurStrength -= 0.05f;
            if(blurStrength <= 0f)
            {
                blurStrength = 0f;
            }
            bool blurActive = blurStrength != 0;
            if (blurActive)
            {
                _blurLerp = MathHelper.Lerp(_blurLerp, 1f, 0.1f);
            }
            else
            {
                _blurLerp = MathHelper.Lerp(_blurLerp, 0f, 0.1f);
            }

            screenShaderData = FilterManager["LunarVeil:Blur"].GetShader();
            screenShaderData.UseProgress(blurStrength * _blurLerp);
            TogglePaletteShader("LunarVeil:Blur", blurActive);

            rippleTimer--;
            if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive() && rippleTimer > 0)
            {
                Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", rippleCenter).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(rippleCenter);

            }

            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive() && rippleTimer == 0)
            {
                float progress = (180f - rippleTimer) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                progress = 1f - progress;
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(rippleDistortStrength * (1 - progress / 3f));
            }
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

        }

        public override void PostUpdate()
        {
            base.PostUpdate();
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

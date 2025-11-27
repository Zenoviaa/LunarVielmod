using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Biomes;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Palettes
{
    public class SpecialEffectsPlayer : ModPlayer
    {
        private bool _init;
        private float _vignetteOpacity;
        private float _vignetteStrength;

        private float _targetVignetteStrength;
        private float _targetVignetteOpacity;
        private float _blurLerp;

        private float _blackWhiteLerp;

        private MyPlayer MyPlayer => Player.GetModPlayer<MyPlayer>();

        private FilterManager FilterManager => Filters.Scene;

        private string DarknessVignette => "LunarVeil:DarknessVignette";

        public bool hasSpiritPendant;
        public bool hasSunGlyph;
        public float darkness;
        public float darknessCurve;
        public float whiteCurve;
        public float blackCurve;
        public float blurStrength;


        public float blackWhiteStrength;
        public float blackWhiteThreshold;


        public Vector2 rippleCenter;
        public float rippleCount;
        public float rippleRadius;
        public float rippleSize;
        public float rippleSpeed;
        public float rippleDistortStrength;
        public float rippleTimer;

        //Progress Variables
        public float darknessCurveProgress = 1f;
        public float[] paletteUseProgress = new float[16];

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSpiritPendant = false;
            hasSunGlyph = false;
            darkness = 0;
            darknessCurve = 0;

            //Curve based
            float progress = (float)(Player.position.ToTileCoordinates().Y - Main.worldSurface) / 1000;
            progress = MathHelper.Clamp(progress, 0, 1);
            darknessCurve = MathHelper.Lerp(0f, darknessCurve, progress * darknessCurveProgress);
            whiteCurve = 0f;
            blackCurve = 1f;

            _targetVignetteOpacity = 0.5f;
        }

        private void TogglePaletteShader(string name, bool isActive)
        {
            if (!ShaderRegistry.ScreenShaders.Contains(name))
                return;
            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            if (isActive)
            {
                if (!FilterManager[name].IsActive())
                {
                    FilterManager.Activate(name);
                }
            }
            else if (!isActive)
            {
                if (FilterManager[name].IsActive())
                {
                    FilterManager.Deactivate(name);
                }
            }
        }

        public void UsePaletteShader(string palFile, bool isActive, ref float progress)
        {
            OldUsePaletteShader(palFile, isActive, ref progress);
        }

        private void OldUsePaletteShader(string palFile, bool isActive, ref float progress)
        {
            float speed = 0.05f;
            if (isActive)
            {
                progress += speed;
            }
            else
            {
                progress -= speed;
            }
            progress = MathHelper.Clamp(progress, 0f, 1f);

            string screenShaderName = $"LunarVeil:{palFile}";
            if (!ShaderRegistry.ScreenShaders.Contains(screenShaderName))
            {
                return;
            }


            if (progress > 0)
            {
                ScreenShaderData screenShaderData = FilterManager[screenShaderName].GetShader();
                screenShaderData.UseProgress(progress);
                screenShaderData.Shader.Parameters["ColorSpectrumTexture"].SetValue(PaletteHelper.GetColorSpectrum(palFile));
            }

            TogglePaletteShader(screenShaderName, progress != 0);
        }
        private void SpecialBiomeEffects()
        {
            //This code should only run on each client
            if (Main.netMode == NetmodeID.Server)
                return;
            if (Main.myPlayer != Player.whoAmI)
                return;

            LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
            ScreenShaderData screenShaderData;
            bool abyssPaletteActive = (MyPlayer.ZoneAbyss || MyPlayer.ZoneAurelus || MyPlayer.ZoneMechanics || MyPlayer.ZoneIshtar) && clientConfig.PaletteShadersToggle;
            bool rustyPaletteActive = (MyPlayer.ZoneGovheil || MyPlayer.ZoneAcid) && clientConfig.PaletteShadersToggle;
            bool hellPaletteActive = ((clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneUnderworldHeight) || (MyPlayer.ZoneCinder || MyPlayer.ZoneDrakonic) && clientConfig.PaletteShadersToggle);

            if (Player.GetModPlayer<MyPlayer>().ZoneWonder)
                hellPaletteActive = false;

            bool royalCapitalPaletteActive = MyPlayer.ZoneAlcadzia && clientConfig.PaletteShadersToggle;

            bool dungeonPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle && Player.ZoneDungeon;
            bool desertPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle
               && (Player.ZoneDesert || Player.GetModPlayer<MyPlayer>().ZoneAshotiTemple)
               && !(Player.ZoneCrimson || Player.ZoneCorrupt)
               && Player.ZoneUndergroundDesert;

            bool desertTopPaletteActive = clientConfig.VanillaBiomesPaletteShadersToggle
           && (Player.ZoneDesert || Player.GetModPlayer<MyPlayer>().ZoneAshotiTemple || Player.GetModPlayer<MyPlayer>().ZoneColloseum)
           && !(Player.ZoneCrimson || Player.ZoneCorrupt)
           && !Player.ZoneUndergroundDesert;


            bool fablePaletteActive = Player.GetModPlayer<MyPlayer>().ZoneFable && clientConfig.PaletteShadersToggle;
            bool mistyPaletteActive = Player.GetModPlayer<BiomePlayer>().ZoneMistyDungeon && clientConfig.PaletteShadersToggle;
            bool bloodPaletteActive = MyPlayer.ZoneBloodCathedral && !Main.dayTime && clientConfig.PaletteShadersToggle;
            //  bloodPaletteActive |= NPC.AnyNPCs(ModContent.NPCType<SanguineSingularity>());

            bool sanguinePaletteActive = NPC.AnyNPCs(ModContent.NPCType<SanguineSingularity>());
            if (abyssPaletteActive)
            {
                darkness += 2;
            }

            if (hellPaletteActive)
            {
                darkness += 1;
            }


            UsePaletteShader("Abyss.pal", abyssPaletteActive, ref paletteUseProgress[0]);
            UsePaletteShader("VilepipesNGarden.pal", rustyPaletteActive, ref paletteUseProgress[1]);
            UsePaletteShader("Hell.pal", hellPaletteActive, ref paletteUseProgress[2]);
            UsePaletteShader("RoyalCapital.pal", royalCapitalPaletteActive, ref paletteUseProgress[3]);
            UsePaletteShader("Dungeon.pal", dungeonPaletteActive, ref paletteUseProgress[4]);
            UsePaletteShader("Desert.pal", desertPaletteActive, ref paletteUseProgress[5]);
            UsePaletteShader("DesertTop.pal", desertTopPaletteActive, ref paletteUseProgress[6]);
            UsePaletteShader("Fable.pal", fablePaletteActive, ref paletteUseProgress[7]);
            UsePaletteShader("IllurianMistyDungeon.pal", mistyPaletteActive, ref paletteUseProgress[8]);
            UsePaletteShader("BloodHound.pal", bloodPaletteActive, ref paletteUseProgress[9]);
            UsePaletteShader("SanguineSingularity.pal", sanguinePaletteActive, ref paletteUseProgress[10]);



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


            if (hellPaletteActive || desertPaletteActive || desertTopPaletteActive)
            {
                darknessCurveProgress -= 0.1f;
            }
            else
            {
                darknessCurveProgress += 0.1f;
            }
            darknessCurveProgress = MathHelper.Clamp(darknessCurveProgress, 0f, 1f);

            blurStrength -= 0.05f;
            if (blurStrength <= 0f)
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


            bool blackWhiteActive = blackWhiteStrength != 0;
            if (blackWhiteActive)
            {
                _blackWhiteLerp += 0.1f;
                if (_blackWhiteLerp >= 1f)
                {
                    _blackWhiteLerp = 1f;
                }
            }
            else
            {
                _blackWhiteLerp -= 0.1f;
                if (_blackWhiteLerp <= 0)
                {
                    _blackWhiteLerp = 0f;
                }
            }
            blackWhiteStrength -= 0.05f;
            if (blackWhiteStrength <= 0f)
            {
                blackWhiteStrength = 0f;
            }

            float strength = MathHelper.Lerp(0, blackWhiteStrength, _blackWhiteLerp);

            screenShaderData = FilterManager["LunarVeil:BlackWhite"].GetShader();
            screenShaderData.Shader.Parameters["strength"].SetValue(strength);
            screenShaderData.Shader.Parameters["brightnessThreshold"].SetValue(blackWhiteThreshold);
            TogglePaletteShader("LunarVeil:BlackWhite", _blackWhiteLerp != 0);

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


        public override void PostUpdate()
        {
            base.PostUpdate();
            SpecialBiomeEffects();
            UpdateVignette();
        }

        private void UpdateVignette()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (Main.myPlayer != Player.whoAmI)
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

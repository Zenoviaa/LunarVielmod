using Stellamod.Assets;
using Stellamod.Assets.Biomes;
using Stellamod.Backgrounds;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.WorldsEnd
{
    public class WorldsEndSky : CustomSky
    {
        private bool _isActive;
        private float _drawOpacity;
        public override void Update(GameTime gameTime)
        {
         
            if (_isActive && _drawOpacity < 1f)
            {
                _drawOpacity += 0.01f;
            }
            else if (!_isActive && _drawOpacity > 0f)
            {
                _drawOpacity -= 0.1f;
            }
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.White * 0.5f * _drawOpacity;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            //draw the sky itself
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {

                SkyGradientShader skyGradientShader = SkyGradientShader.Instance;
                skyGradientShader.H = 0.15f;
                skyGradientShader.Bend = -0.25f;
                skyGradientShader.StartColor = Color.Black;
                skyGradientShader.MidColor = Color.Blue;
                skyGradientShader.EndColor = Color.Aquamarine;
                spriteBatch.Restart(effect: skyGradientShader.Effect);
                Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * _drawOpacity);
                spriteBatch.RestartDefaults();
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - _drawOpacity);
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            _drawOpacity = 0.002f;
            _isActive = true;
        }


        public override void Deactivate(params object[] args)
        {
            _isActive = false;
        }

        public override void Reset()
        {
            _isActive = false;
        }

        public override bool IsActive()
        {
            return (_isActive || _drawOpacity > 0.001f) && !Main.gameMenu;
        }
    }
    public class WorldsEndBackgroundStyle : ModSurfaceBackgroundStyle
    {
        // Use this to keep far Backgrounds like the mountains.
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/GreyGrassBackgroundFar");
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/GreyGrassBackgroundMid");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/GreyGrassBackgroundClose");
        }
    }
    public class WorldsEndBiome : ModBiome
    {
        public override int Music
        {
            get
            {
                //Put your if statement here

                //Normal music
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/WorldsEnd");
            }
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();

        public override bool IsBiomeActive(Player player) => (player.ZoneOverworldHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InWorldsEnd;
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneWorldsEnd = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneWorldsEnd = false;
    }
}

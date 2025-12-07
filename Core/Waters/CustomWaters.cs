using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.ScreenSystems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Waters
{
    public enum WaterStyle : byte
    {
        Default,
        Icey,
        Bloody,
        Shimmer
    }
    public struct WaterGradient
    {
        public WaterGradient(Color startColor, Color endColor)
        {
            this.startColor = startColor;
            this.endColor = endColor;
        }
        public Color startColor;
        public Color endColor;
    }

    public class CustomWaters : WaterAddon
    {
        private static WaterStyle _style;
        public static bool Biomes => ModContent.GetInstance<LunarVeilClientConfig>().LiquidsToggle;

        public static ScreenTarget BackTarget = new ScreenTarget(RenderFront, () => Biomes, 1, (a) => Main.waterTarget.Size());
        public static ScreenTarget FrontTarget = new ScreenTarget(RenderBack, () => Biomes, 1, (a) => Main.instance.backWaterTarget.Size());

        public override bool Visible => true;
        public override Texture2D BlockTexture(Texture2D normal, int x, int y)
        {
            return normal;
        }

        private static void ChooseWaterStyle()
        {
            Player player = Main.LocalPlayer;
            _style = WaterStyle.Default;
            if (player.ZoneCrimson)
            {
                _style = WaterStyle.Bloody;
            }

            if (player.ZoneSnow)
            {
                _style = WaterStyle.Icey;
            }

            if (player.ZoneShimmer || player.GetModPlayer<MyPlayer>().ZoneWonder)
            {
                _style = WaterStyle.Shimmer;
            }
        }

        private static Texture2D GetWaterStyleNoiseTexture(WaterStyle waterStyle)
        {
            Texture2D tex2;
            switch (waterStyle)
            {
                case WaterStyle.Default:
                default:
                    tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Water3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    break;

                case WaterStyle.Icey:
                    tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Water", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    break;

                case WaterStyle.Bloody:
                    tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloodWater", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    break;

                case WaterStyle.Shimmer:
                    tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Refraction", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    break;
            }
            return tex2;
        }

        private static void RenderFront(SpriteBatch spriteBatch)
        {
            ChooseWaterStyle();
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default);
            Texture2D tex2 = GetWaterStyleNoiseTexture(_style);
            RenderFront_Inner(spriteBatch, tex2);
        }

        private static void RenderFront_Inner(SpriteBatch spriteBatch, Texture2D tex2)
        {
            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the BACKGROUND, which is the position of the FOREGROUND minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWaterPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;
                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White * 0.4f);
                }
            }
        }

        private static void RenderBack(SpriteBatch spriteBatch)
        {
            ChooseWaterStyle();
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default);

            Texture2D tex2 = GetWaterStyleNoiseTexture(_style);
            RenderBack_Inner(spriteBatch, tex2);
        }

        private static void RenderBack_Inner(SpriteBatch spriteBatch, Texture2D tex2)
        {
            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the FOREGROUND, which is the position of the WALL RT minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWallPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;
                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White * 0.4f);
                }
            }
        }

        private WaterGradient GetGradient()
        {
            WaterGradient gradient = new WaterGradient(Color.White, Color.Green);
            return gradient;
        }

        private Effect GetWaterEffect()
        {
            Effect effect = Filters.Scene["LunarVeil:Water"].GetShader().Shader;
            return effect;
        }

        private void ApplyOffsetAndTime(Effect effect)
        {
            effect.Parameters["offset"].SetValue(Vector2.Zero);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount / 20f);
        }

        private void ApplyGradient(Effect effect, WaterGradient gradient)
        {
            effect.Parameters["startGradientColor"].SetValue(gradient.startColor.ToVector3());
            effect.Parameters["endGradientColor"].SetValue(gradient.endColor.ToVector3());
        }

        public override void SpritebatchChange()
        {
            Effect effect = GetWaterEffect();
            ApplyOffsetAndTime(effect);

            WaterGradient gradient = GetGradient();
            ApplyGradient(effect, gradient);
            effect.Parameters["sampleTexture2"].SetValue(FrontTarget.RenderTarget);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }

        public override void SpritebatchChangeBack()
        {
            Effect effect = GetWaterEffect();
            ApplyOffsetAndTime(effect);

            WaterGradient gradient = GetGradient();
            ApplyGradient(effect, gradient);
            effect.Parameters["sampleTexture2"].SetValue(BackTarget.RenderTarget);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }
    }
}

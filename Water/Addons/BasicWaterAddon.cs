using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Water
{
    internal class BasicWaterAddon : WaterAddon
    {
        public static ScreenTarget BackTarget = new(RenderFront, () => Main.LocalPlayer.ZoneBeach, 1, (a) => Main.waterTarget.Size());
        public static ScreenTarget FrontTarget = new(RenderBack, () => Main.LocalPlayer.ZoneBeach, 1, (a) => Main.instance.backWaterTarget.Size());

        public override bool Visible => Main.LocalPlayer.ZoneBeach;

        public override Texture2D BlockTexture(Texture2D normal, int x, int y)
        {
            return normal;
        }

        private static void RenderFront(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, default);

            Texture2D tex2 = ModContent.Request<Texture2D>("Stellamod/Textures/Water2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the BACKGROUND, which is the position of the FOREGROUND minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWaterPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;

                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White * 0.65f);
                }
            }
            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, default);

            tex2 = ModContent.Request<Texture2D>("Stellamod/Textures/WaterGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the BACKGROUND, which is the position of the FOREGROUND minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWaterPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;

                    for (int w = 0; w < 20; w++)
                        spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White);
                }
            }
        }

        private static void RenderBack(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, default);

            Texture2D tex2 = ModContent.Request<Texture2D>("Stellamod/Textures/Water2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the FOREGROUND, which is the position of the WALL RT minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWallPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;
                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White * 0.65f);
                }
            }

            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, default);

            tex2 = ModContent.Request<Texture2D>("Stellamod/Textures/WaterGradient", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the BACKGROUND, which is the position of the FOREGROUND minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWaterPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;

                    for (int w = 0; w < 20; w++)
                        spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.White);
                }
            }
        }

        public override void SpritebatchChange()
        {
            Effect effect = Filters.Scene["Stellamod:Water"].GetShader().Shader;
            effect.Parameters["offset"].SetValue(Vector2.Zero);
            effect.Parameters["sampleTexture2"].SetValue(FrontTarget.RenderTarget);
            effect.Parameters["sampleTexture3"].SetValue(FrontTarget.RenderTarget);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount / 20f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }

        public override void SpritebatchChangeBack()
        {
            Effect effect = Filters.Scene["Stellamod:Water"].GetShader().Shader;
            effect.Parameters["offset"].SetValue(Vector2.Zero);
            effect.Parameters["sampleTexture2"].SetValue(BackTarget.RenderTarget);
            effect.Parameters["sampleTexture3"].SetValue(BackTarget.RenderTarget);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount / 20f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }
    }
}

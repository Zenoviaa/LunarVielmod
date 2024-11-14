using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ScreenSystems;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Common.Waters
{
    internal class LavaAddon : WaterAddon
    {
        public static bool ShouldShow
        {
            get
            {
                LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();

                //Don't show if waters
                /*
                if (!clientConfig.WatersToggle)
                    return false;*/

                //Don't show if not in underworld
                /*if (!Main.LocalPlayer.ZoneUnderworldHeight)
                    return false;
                */
                if (!clientConfig.LiquidsToggle)
                    return false;

                return true;
            }
        }


        public static ScreenTarget BackTarget = new(RenderFront, () => ShouldShow, 1, (a) => Main.waterTarget.Size());
        public static ScreenTarget FrontTarget = new(RenderBack, () => ShouldShow, 1, (a) => Main.instance.backWaterTarget.Size());

        public override bool Visible => ShouldShow;

        public override Texture2D BlockTexture(Texture2D normal, int x, int y)
        {
            return normal;
        }

        private static void RenderFront(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default);

            Texture2D tex2 = TextureRegistry.LavaDepths.Value;

            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the BACKGROUND, which is the position of the FOREGROUND minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWaterPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;

                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.Orange * 0.55f);
                }
            }
        }

        private static void RenderBack(SpriteBatch spriteBatch)
        {
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default);

            Texture2D tex2 = TextureRegistry.LavaDepths.Value;

            for (int i = -tex2.Width; i <= Main.screenWidth + tex2.Width; i += tex2.Width)
            {
                for (int j = -tex2.Height; j <= Main.screenHeight + tex2.Height; j += tex2.Height)
                {
                    var pos = new Vector2(i, j);

                    // This is the offset for the FOREGROUND, which is the position of the WALL RT minus screen pos (why? because god is a cruel creature)
                    if (!Main.drawToScreen)
                        pos -= Main.sceneWallPos - Main.screenPosition;

                    Vector2 tsp = Main.screenPosition;

                    spriteBatch.Draw(tex2, pos - new Vector2(tsp.X % tex2.Width, tsp.Y % tex2.Height), null, Color.Orange * 0.55f);
                }
            }
        }

        public override void SpritebatchChange()
        {
            Effect effect = Filters.Scene["LunarVeil:Lava"].GetShader().Shader;
            effect.Parameters["offset"].SetValue(Vector2.Zero);
            effect.Parameters["sampleTexture2"].SetValue(FrontTarget.RenderTarget);
            effect.Parameters["sampleTexture3"].SetValue(FrontTarget.RenderTarget);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount / 20f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }

        public override void SpritebatchChangeBack()
        {
            Effect effect = Filters.Scene["LunarVeil:Lava"].GetShader().Shader;
            effect.Parameters["offset"].SetValue(Vector2.Zero);
            effect.Parameters["sampleTexture2"].SetValue(BackTarget.RenderTarget);
            effect.Parameters["sampleTexture3"].SetValue(BackTarget.RenderTarget);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount / 20f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }
    }
}

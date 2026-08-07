using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static Terraria.Main;

namespace Stellamod.Skies
{
    public class GreenSunSky : CustomSky
    {
        public bool isActive;
        public float Intensity;

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override bool IsActive()
        {
            return Intensity > 0f;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.gameMenu)
                return;

            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

                Texture2D Tex2 = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/SkyGradient2").Value;

             //   spriteBatch.Draw(Tex, new Rectangle(0, 0 - (int)screenPosition.Y, screenWidth, 3000), null, Color.LightPink * Intensity * 1f, 0, Vector2.Zero, SpriteEffects.None, 0);
                for (int i = 0; i < 2; i++)
                    spriteBatch.Draw(Tex2, new Rectangle(0, -30, screenWidth, screenHeight), null, Color.Green * Intensity * 1f, 0, Vector2.Zero, SpriteEffects.None, 0);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
            }

            if (maxDepth >= 11 && minDepth < 11)
            {
                DrawSun(spriteBatch);
            }
  
        }
        private void DrawSun(SpriteBatch spriteBatch)
        {
            int num13 = screenWidth;
            int num14 = screenHeight;
            Vector2 zero = Vector2.Zero;
            if (num13 < 800)
            {
                int num15 = 800 - num13;
                zero.X -= (float)num15 * 0.5f;
                num13 = 800;
            }

            if (num14 < 600)
            {
                int num16 = 600 - num14;
                zero.Y -= (float)num16 * 0.5f;
                num14 = 600;
            }

            SceneArea sceneArea2 = default(SceneArea);
            sceneArea2.bgTopY = 0;
            sceneArea2.totalWidth = num13;
            sceneArea2.totalHeight = num14;
            sceneArea2.SceneLocalScreenPositionOffset = zero;
            SceneArea sceneArea3 = sceneArea2;
            var sceneArea = sceneArea3;

            Color sunColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            Texture2D value = TextureAssets.Sun.Value;
            int num = moonType;
            if (!TextureAssets.Moon.IndexInRange(num))
                num = Utils.Clamp(num, 0, 8);

            Texture2D value2 = TextureAssets.Moon[num].Value;

            if (gameMenu)
            {
                ModMenu menu = MenuLoader.CurrentMenu;
                value = menu.SunTexture?.Value ?? value;
                value2 = menu.MoonTexture?.Value ?? value2;
            }

            int num2 = sceneArea.bgTopY;
            int num3 = (int)(time / 54000.0 * (double)(sceneArea.totalWidth + (float)(value.Width * 2))) - value.Width;
            int num4 = 0;
            float num5 = 1f;
            float rotation = (float)(time / 54000.0) * 2f - 7.3f;
            int num6 = (int)(time / 32400.0 * (double)(sceneArea.totalWidth + (float)(value2.Width * 2))) - value2.Width;
            int num7 = 0;
            float num8 = 1f;
            float num9 = (float)(time / 32400.0) * 2f - 7.3f;
            if (dayTime)
            {
                double num10;
                if (time < 27000.0)
                {
                    num10 = Math.Pow(1.0 - time / 54000.0 * 2.0, 2.0);
                    num4 = (int)((double)num2 + num10 * 250.0 + 180.0);
                }
                else
                {
                    num10 = Math.Pow((time / 54000.0 - 0.5) * 2.0, 2.0);
                    num4 = (int)((double)num2 + num10 * 250.0 + 180.0);
                }

                num5 = (float)(1.2 - num10 * 0.4);
            }
            else
            {
                double num11;
                if (time < 16200.0)
                {
                    num11 = Math.Pow(1.0 - time / 32400.0 * 2.0, 2.0);
                    num7 = (int)((double)num2 + num11 * 250.0 + 180.0);
                }
                else
                {
                    num11 = Math.Pow((time / 32400.0 - 0.5) * 2.0, 2.0);
                    num7 = (int)((double)num2 + num11 * 250.0 + 180.0);
                }

                num8 = (float)(1.2 - num11 * 0.4);
            }

            num5 *= ForcedMinimumZoom;
            num8 *= ForcedMinimumZoom;
            if (starGame)
            {
                if (WorldGen.generatingWorld)
                {
                    alreadyGrabbingSunOrMoon = true;
                    if (rand.Next(60) == 0)
                    {
                        for (int i = 0; i < numStars; i++)
                        {
                            if (star[i].hidden)
                                Star.SpawnStars(i);
                        }
                    }

                    if (dayTime)
                    {
                        dayTime = false;
                        time = 0.0;
                    }
                }
                else
                {
                    starGame = false;
                }
            }
            else
            {
                starsHit = 0;
            }

            if (dayTime)
            {
                if ((remixWorld && !gameMenu) || WorldGen.remixWorldGen)
                    return;

                num5 *= 1.1f;
                float num12 = 1f - 0;
                num12 -= cloudAlpha * 1.5f * atmo;
                if (num12 < 0f)
                    num12 = 0f;

                Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color((byte)(255f * num12), (byte)((float)(int)sunColor.G * num12), (byte)((float)(int)sunColor.B * num12), (byte)(255f * num12));
                Microsoft.Xna.Framework.Color color2 = new Microsoft.Xna.Framework.Color((byte)((float)(int)sunColor.R * num12), (byte)((float)(int)sunColor.G * num12), (byte)((float)(int)sunColor.B * num12), (byte)((float)(int)sunColor.B * num12));
                bool flag = false;
                if (eclipse)
                {
                    value = TextureAssets.Sun3.Value;
                    flag = true;
                }
                else if (!gameMenu && player[myPlayer].head == 12)
                {
                    value = TextureAssets.Sun2.Value;
                    flag = true;
                }

                if (flag)
                    color2 = new Microsoft.Xna.Framework.Color((byte)((float)(int)sunColor.R * num12), (byte)((float)(int)sunColor.G * num12), (byte)((float)(int)sunColor.B * num12), (byte)((float)(sunColor.B - 60) * num12));

                Vector2 origin = value.Size() / 2f;
                Vector2 position = new Vector2(num3, num4 + sunModY) + sceneArea.SceneLocalScreenPositionOffset;
                position.Y -= 128;
            //    spriteBatch.Draw(value, position, null, color, rotation, origin, num5, SpriteEffects.None, 0f);
              //  spriteBatch.Draw(value, position, null, color2, rotation, origin, num5, SpriteEffects.None, 0f);

                sunColor = Color.Lerp(sunColor, Color.Green, 0.7f);
                sunColor.A = 0;
                Asset<Texture2D> glowCircle = ModContent.Request<Texture2D>($"Stellamod/Assets/Effects/SimpleGlowCircle");


                spriteBatch.Draw(glowCircle.Value, position, null, sunColor, 0, glowCircle.Size() * 0.5f, 0.65f, SpriteEffects.None, 0);
            
            }
        }
        public override Color OnTileColor(Color inColor)
        {
           
            return Color.Lerp(inColor, Color.LightGreen, 0.2f);
        }
    }
}
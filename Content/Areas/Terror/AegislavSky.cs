using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using static Terraria.Main;

namespace Stellamod.Content.Areas.Terror;

public class AegislavSky : CustomSky
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
        _drawOpacity = MathHelper.Clamp(_drawOpacity, 0f, 1f);
    }

    public override Color OnTileColor(Color inColor)
    {
        Color targetColor = inColor * 0.5f;
        Color inbetweenColor = Color.Lerp(inColor, targetColor, _drawOpacity);
        return inbetweenColor;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
     //   Main
        if (maxDepth >= 11 && minDepth < 11)
        {

            Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            SkyGradientShader skyGradientShader = SkyGradientShader.Instance;
            skyGradientShader.H = 0.8f;
            skyGradientShader.Bend = -0.2f;
            skyGradientShader.StartColor = Color.Black;
            skyGradientShader.MidColor = Color.DarkRed;
            skyGradientShader.EndColor = Color.Red;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, skyGradientShader.Effect, Main.BackgroundViewMatrix.TransformationMatrix);


            spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * _drawOpacity);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);

            //Prepare Clouds Draw
            AegislavCloudsShader aegislavCloudsShader = AegislavCloudsShader.Instance;
            aegislavCloudsShader.XStretch =4f;

            aegislavCloudsShader.Time = Main.GlobalTimeWrappedHourly * 2f;
            aegislavCloudsShader.Parallax = new Vector2(Main.GlobalTimeWrappedHourly * 0.002f + Main.screenPosition.X * 0.00003f, 0.5f);
            Asset<Texture2D> cloudsTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds");
            Asset<Texture2D> cloudsTexture2 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds2");
            Asset<Texture2D> cloudsTexture3 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds3");

            aegislavCloudsShader.TexelSize = Vector2.One / new Vector2(cloudsTexture.Width(), cloudsTexture.Height());
            spriteBatch.GraphicsDevice.Textures[1] = cloudsTexture2.Value;
            spriteBatch.GraphicsDevice.Textures[2] = cloudsTexture3.Value;
            spriteBatch.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            spriteBatch.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer,
                aegislavCloudsShader.Effect, Main.BackgroundViewMatrix.TransformationMatrix);



            Color cloudCOlor = Color.IndianRed * 0.86f;
            cloudCOlor.A = 0;
            spriteBatch.Draw(cloudsTexture.Value, Vector2.Zero, targetRectangle, cloudCOlor * _drawOpacity, 0, Vector2.Zero, new Vector2(5f, 1f), SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);

            DrawSun(spriteBatch);
        }

        //draw the sky itself
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {

          
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
            spriteBatch.Draw(value, position, null, color, rotation, origin, num5, SpriteEffects.None, 0f);
            spriteBatch.Draw(value, position, null, color2, rotation, origin, num5, SpriteEffects.None, 0f);
           
            SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(
                    AssetManager.GlowMask.SimpleGlowCircle, position);
            bloomDrawer.color = sunColor * 0.25f;
            bloomDrawer.color.A = 0;

            bloomDrawer.worldPosition += Main.screenPosition;
            Main.spriteBatch.Draw(bloomDrawer);
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
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

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
        return Color.White * 0.5f * _drawOpacity;
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
        }

        //draw the sky itself
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {

          
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
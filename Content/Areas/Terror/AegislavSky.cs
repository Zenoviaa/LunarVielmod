using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.Graphics.Effects;

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
        //draw the sky itself
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {

            SkyGradientShader skyGradientShader = SkyGradientShader.Instance;
            skyGradientShader.H = 0.95f;
            skyGradientShader.Bend = -0.25f;
            skyGradientShader.StartColor = Color.Black;
            skyGradientShader.MidColor = Color.DarkRed;
            skyGradientShader.EndColor = Color.Red;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, skyGradientShader.Effect, Main.BackgroundViewMatrix.TransformationMatrix);

            Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * _drawOpacity);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
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
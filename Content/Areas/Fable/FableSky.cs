using Stellamod.Assets;
using Terraria;
using Terraria.Graphics.Effects;

namespace Stellamod.Content.Areas.Fable;

public class FableSky : CustomSky
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
        return inColor;
        //  return Color.White * 0.5f * _drawOpacity;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        //draw the sky itself
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {
            //Draw Sky visual here
            var pass = AssetReferences.Effects.Fable.FableSky.CreatePixelPass();

            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.noiseSampler = new()
            {
                Texture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset.Value,
                Sampler = SamplerState.LinearWrap
            };
            pass.Parameters.ditherSampler = new()
            {
                Texture = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value,
                Sampler = SamplerState.LinearWrap
            };
            pass.Parameters.spriteSize = new Vector2(Main.screenWidth, Main.screenHeight);
            pass.Parameters.ditherTexelSize = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value.GetTexelSize();
            pass.Parameters.parallax = Main.screenPosition * 0.001f;
            pass.Apply();
            using(spriteBatch.Ctx(spriteBatch.Parameters with {  effect = pass.Shader }))
            {
                Rectangle targetRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Main.ColorOfTheSkies * _drawOpacity);
            }
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

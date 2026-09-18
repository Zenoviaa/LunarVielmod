using Stellamod.Core;
using Terraria;
using Terraria.Graphics.Effects;

namespace Stellamod.Skies;


public class RoyalCapitalSky : CustomSky
{
    private float _strength;
    public float Strength { get => _strength; }
    public float Fogginess { get; set; }

    public override void Activate(Vector2 position, params object[] args)
    {

    }

    public override void Deactivate(params object[] args)
    {

    }

    public override bool IsActive() =>
        _strength > 0.001f && !Main.gameMenu;

    public override void Reset()
    {

    }

    public override void Update(GameTime gameTime)
    {

    }



    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (maxDepth >= 7 && minDepth < 7)
        {
            DrawSky(spriteBatch);
        }
    }


    private void DrawSky(SpriteBatch spriteBatch)
    {
        var texture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset;
        var pass = AssetReferences.Effects.RoyalCapitalSky.CreateScreenPass();

        var noiseSampler = new HlslSampler();
        noiseSampler.Sampler = SamplerState.LinearWrap;
        noiseSampler.Texture = texture.Value;
        pass.Parameters.noiseSampler = noiseSampler;
        pass.Parameters.uTime = Main.GlobalTimeWrappedHourly;
        pass.Apply();

        spriteBatch.End();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.LinearWrap,
            DepthStencilState.None,
            Main.Rasterizer,
            pass.Shader,
            Main.BackgroundViewMatrix.TransformationMatrix);

        Vector2 drawOrigin = texture.Value.Size() * 0.5f;
        BackgroundDrawParameters draw = DrawUtilities.CalculateScaledBackgroundDraw(texture.Value.Size());
        spriteBatch.Draw(texture.Value,
           draw.DrawOffset,
            draw.SourceRectangle, Color.White * 0.3f, 0, drawOrigin, 4, SpriteEffects.None, 0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer,
            null,
            Main.BackgroundViewMatrix.TransformationMatrix);
    }
}

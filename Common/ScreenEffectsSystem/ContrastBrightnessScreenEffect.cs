using Stellamod.Core;

namespace Stellamod.Common.ScreenEffectsSystem;

public class ContrastBrightnessScreenEffect : AScreenEffect
{
    public float brightness;
    public float contrast;
    public override ScreenEffectPriority Priority => ScreenEffectPriority.Very_Late;
    public override void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst)
    {
        var pass = AssetReferences.Effects.ContrastBrightness.CreateScreenPass();
        pass.Parameters.brightness = brightness;
        pass.Parameters.contrast = contrast;
        pass.Apply();
        spriteBatch.Begin(
            SpriteSortMode.Deferred, 
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None, 
            RasterizerState.CullNone, 
            pass.Shader);
        spriteBatch.Draw(src, Vector2.Zero, Color.White);
        spriteBatch.End();
    }
}

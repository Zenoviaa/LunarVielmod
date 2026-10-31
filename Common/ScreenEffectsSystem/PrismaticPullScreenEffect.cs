using Terraria;

namespace Stellamod.Common.ScreenEffectsSystem;

public class PrismaticPullScreenEffect : AScreenEffect
{
    public Vector2 worldCenter;
    public float strength;
    public float alpha;
    public override ScreenEffectPriority Priority => ScreenEffectPriority.Very_Late;
    public override void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst)
    {
        var pass = AssetReferences.Effects.PrismaticPull.CreateScreenPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.texelSize = src.GetTexelSize();
        pass.Parameters.strength = strength;
        pass.Parameters.centerPoint = (worldCenter - Main.screenPosition) / new Vector2(src.Width, src.Height);
        pass.Parameters.alpha = alpha;
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

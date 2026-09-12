using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering.RTs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem;

public interface IDrawSpectral
{
    void DrawSpectralWhites(SpriteBatch spriteBatch);
    void DrawSpectral(SpriteBatch spriteBatch);
}

[Autoload(Side = ModSide.Client)]
public class SpectralSummonDrawSystem : ModSystem
{
    private static readonly List<IDrawSpectral> _spectralDraws = new();
    public override void Load()
    {
        On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRenderTarget;
    }

    public override void Unload()
    {
        On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRenderTarget;
        _spectralDraws?.Clear();
    }

    private void DrawPixelRenderTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        _spectralDraws.Clear();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.ModProjectile is IDrawSpectral minion)
            {
                _spectralDraws.Add(minion);
            }
        }

        if (_spectralDraws.Count <= 0)
            return;

        RenderTargetHandle screenTarget = RenderTargets.ScreenTarget;
        using (new RenderTargetContext(screenTarget))
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.GameViewMatrix.TransformationMatrix);
            foreach (var drawer in _spectralDraws)
            {
                drawer.DrawSpectralWhites(Main.spriteBatch);
            }
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (var drawer in _spectralDraws)
            {
                drawer.DrawSpectral(Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }


        var shader = SpectralShader.Instance;
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
            shader.Effect);
        Main.spriteBatch.Draw(screenTarget, Vector2.Zero, null, Color.White * 0.87f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        Main.spriteBatch.End();
    }
}

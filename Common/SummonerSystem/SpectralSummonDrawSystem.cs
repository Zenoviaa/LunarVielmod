using Stellamod.Common.Shaders;
using Stellamod.Core;
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

        using var screenTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(screenTarget, Color.Transparent))
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

        var pass = AssetReferences.Effects.CrystalShaders.Spectral.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.distortionStrength = 0.002f;
        var noiseTex = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value;
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture  = noiseTex
        };
        pass.Parameters.noiseTexelSize = noiseTex.GetTexelSize();
        pass.Parameters.texelSize = screenTarget.Target.GetTexelSize();
        pass.Parameters.screenOffset = Main.screenPosition;
        pass.Apply();

        Main.spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.None, 
            Main.Rasterizer,
            pass.Shader);
        Main.spriteBatch.Draw(screenTarget, Vector2.Zero, null, Color.White * 0.87f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        Main.spriteBatch.End();
    }
}

using Stellamod.Common.ScreenEffectsSystem;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class MoonEffect : AScreenEffect
{
    static readonly Queue<Action<SpriteBatch>> _drawQueue = new();
    public override ScreenEffectPriority Priority => ScreenEffectPriority.Very_Late;
    public override void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(temp, Color.Transparent))
        {
            var beginner = SpritebatchParams.InWorldAndZoomed();
            spriteBatch.Begin(beginner);
            while (_drawQueue.Count > 0)
            { 
                _drawQueue.Dequeue()(spriteBatch);
            }
            spriteBatch.End();
        }

        var pass = AssetReferences.Effects.Generic.MoonAuraMask.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.maskSampler = new()
        {
            Sampler = SamplerState.PointClamp,
            Texture = temp
        };
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.NoiseTextures.BlurryPerlinNoise.Asset.Value
        };

        pass.Parameters.starsSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.NoiseTextures.StarNoise.Asset.Value
        };
        pass.Parameters.starsTexelSize = AssetReferences.Assets.NoiseTextures.StarNoise.Asset.Value.GetTexelSize();
        pass.Parameters.spriteSize = Main.ScreenSize.ToVector2();
        pass.Parameters.distortionStrength = 2f;
        pass.Parameters.screenFixer = Main.screenPosition * (Vector2.One / Main.ScreenSize.ToVector2());
        pass.Apply();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            pass.Shader);
        spriteBatch.Draw(src, Vector2.Zero, Color.LightSkyBlue);
        spriteBatch.End();
    }

    public static void PrepareForRenderering(Action<SpriteBatch> drawAction)
    {
        _drawQueue.Enqueue(drawAction);
    }
}


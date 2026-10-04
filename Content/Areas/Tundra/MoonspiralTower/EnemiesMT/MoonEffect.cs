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
    static readonly List<Action<SpriteBatch>> _exclusionList = new();
    public override ScreenEffectPriority Priority => ScreenEffectPriority.Very_Late;
    public override void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        using var temp2 = RT.Context(RenderTargets.ScreenTarget);
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

        using(RT.Clear(temp2, Color.Transparent))
        {
            var beginner = SpritebatchParams.InWorldAndZoomed();
            spriteBatch.Begin(beginner);
            foreach (var action in _exclusionList)
            {
                action(spriteBatch);
            }
            spriteBatch.End();
            _exclusionList.Clear();
        }


        var pass = AssetReferences.Effects.Generic.MoonAuraMask.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.maskSampler = new()
        {
            Sampler = SamplerState.PointClamp,
            Texture = temp
        };

        var stars = AssetReferences.Assets.NoiseTextures.StarNoise2.Asset.Value;
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.NoiseTextures.BlurryPerlinNoise.Asset.Value
        };

        pass.Parameters.starsSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = stars
        };
        pass.Parameters.exclusionSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = temp2
        };
        pass.Parameters.starsTexelSize = stars.GetTexelSize();
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

    public static void PrepareForExclusionRendering(Action<SpriteBatch> drawAction)
    {
        _exclusionList.Add(drawAction);
    }

    public static void PrepareForRenderering(Action<SpriteBatch> drawAction)
    {
        _drawQueue.Enqueue(drawAction);
    }
}


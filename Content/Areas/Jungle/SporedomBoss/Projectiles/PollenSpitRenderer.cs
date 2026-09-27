using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

[Autoload(Side = ModSide.Client)]
public class PollenSpitRenderer : ModSystem
{
    public static readonly Queue<Action<SpriteBatch>> DrawActionQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderPollenSpit;
    }

    private void RenderPollenSpit(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (DrawActionQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPollenSpit);
        }
    }

    private void DrawPollenSpit(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        spriteBatch.EndOut(out var oldParameters);
        using var maskTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(maskTarget, Color.Transparent))
        {
            using (spriteBatch.Ctx(oldParameters with { matrix = Matrix.identity }))
            {
                while (DrawActionQueue.Count > 0)
                {
                    DrawActionQueue.Dequeue()(spriteBatch);
                }
            }
        }


        var pollenMixPass = AssetReferences.Effects.Foresty.PollenMix.CreateBlackPass();
        pollenMixPass.Parameters.time = Main.GlobalTimeWrappedHourly;

        var noiseTexture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset.Value;
        pollenMixPass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointClamp,
            Texture = noiseTexture
        };

        var darkColor = new Color(242, 92, 0);
        var lightColor = new Color(255, 189, 162);
        darkColor = Color.Lerp(darkColor, Color.Black, 0.5f);
        lightColor = Color.Lerp(lightColor, Color.Gold, 0.9f);
        pollenMixPass.Parameters.darkColor = darkColor.ToVector4();
        pollenMixPass.Parameters.lightColor = lightColor.ToVector4();
        pollenMixPass.Parameters.spriteSize = maskTarget.Target.Size();
        pollenMixPass.Parameters.noiseTexelSize = noiseTexture.GetTexelSize();
        pollenMixPass.Apply();
        using (spriteBatch.Ctx(oldParameters with { effect = pollenMixPass.Shader }))
        {
            spriteBatch.Draw(maskTarget, Vector2.Zero, Color.White);
        }
        spriteBatch.Begin(oldParameters);

    }
}

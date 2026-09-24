using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

[Autoload(Side = ModSide.Client)]
public class IceRenderer : ModSystem,
    IRenderer
{
    private Queue<PixelTarget.SpritebatchDrawAction> _drawActionQueue;
    public int Priority => 0;

    public override void OnModLoad()
    {
        base.OnModLoad();
        _drawActionQueue = new Queue<PixelTarget.SpritebatchDrawAction>(2);
    }

    public void Render()
    {
        if (_drawActionQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawMaskToPixelTarget, DrawLayer.OverNPCsWithOutline);
        }
    }

    private void DrawMaskToPixelTarget(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        using var iceRT = RT.Context(RenderTargets.ScreenTarget);
        using var icicleMaskRT = RT.Context(RenderTargets.ScreenTarget);
        using var icicleRT = RT.Context(RenderTargets.ScreenTarget);
        spriteBatch.EndOut(out var parameters);

        using (RT.Clear(icicleMaskRT, Color.Transparent))
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            while (_drawActionQueue.Count > 0)
            {
                var drawAction = _drawActionQueue.Dequeue();
                drawAction(spriteBatch, Main.screenPosition);
            }
            spriteBatch.End();
        }

        using (RT.Clear(iceRT, Color.Transparent))
        {
            IceShader iceShader = IceShader.Instance;
            iceShader.NoiseTexture = TrailRegistry.Clouds3;
            iceShader.Tiling = Vector2.One * 132;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, iceShader.Effect);
            spriteBatch.Draw(icicleMaskRT, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        using (RT.Clear(icicleRT, Color.Transparent))
        {
            MaskCombineShader combineShader = MaskCombineShader.Instance;
            combineShader.MixTexture = iceRT;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, combineShader.Effect);
            spriteBatch.Draw(icicleMaskRT, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        spriteBatch.Begin(parameters);
        spriteBatch.Draw(icicleRT, Vector2.Zero, Color.White);
    }


    public static void QueueDrawAction(PixelTarget.SpritebatchDrawAction drawAction)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        IceRenderer renderer = ModContent.GetInstance<IceRenderer>();
        renderer._drawActionQueue.Enqueue(drawAction);
    }
}

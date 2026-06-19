using Stellamod.Core.Pixelation;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

[Autoload(Side = ModSide.Client)]
public class ShaderRenderPipeline : ModSystem
{
    private static readonly Queue<IDrawBatch> _drawBatches = new();
    public override void Load()
    {
        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += Render;
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= Render;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();

    }

    private void Render()
    {
        while(_drawBatches.Count > 0)
        {
            IDrawBatch batch = _drawBatches.Dequeue();
            PixelationManager.QueuePrimitivesDrawAction(batch.Flush, batch.DrawLayer);
        }
        
    }

    public static void QueueBatch(IDrawBatch batch)
    {
        _drawBatches.Enqueue(batch);
    }
}
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

[Autoload(Side = ModSide.Client)]
public abstract class BasePixelPrimitiveRenderer : ModSystem
{
    public abstract void PreparePoints(Vector2[] points);
}

/// <summary>
/// Renders all primitives of the same shader into a single draw call, into the pixelation manager
/// </summary>
/// <typeparam name="T"></typeparam>

public abstract class PixelPrimitiveRenderer<T> : BasePixelPrimitiveRenderer where T : BasePixelPrimitiveRenderer
{
    //Not sure if a queue is the best structure to use here for storing the points
    //Maybe we can make a single buffer work?
    private Queue<Vector2[]> _primitiveDrawQueue;
    public override void Load()
    {
        base.Load();
        _primitiveDrawQueue = new Queue<Vector2[]>();
        PixelationManager.OnPreRender += QueueDraws;
    }



    public override void Unload()
    {
        base.Unload();
        _primitiveDrawQueue.Clear();
        _primitiveDrawQueue = null;
        PixelationManager.OnPreRender -= QueueDraws;
    }
    private void QueueDraws()
    {
        if (_primitiveDrawQueue.Count <= 0)
            return;

       // Main.NewText("E");
        PixelationManager.QueuePrimitivesDrawAction(DrawPrimitives, DrawLayer.OverPlayers);
    }



    public override void PreparePoints(Vector2[] points) => _primitiveDrawQueue.Enqueue(points);
    public static void Queue(Vector2[] points)
    {
        T instance = ModContent.GetInstance<T>();
        instance.PreparePoints(points);
    }

    public void DrawPrimitives(GraphicsDevice graphicsDevice)
    {

        TrailDrawer.ClearPrimitives();
        //Main.NewText(_primitiveDrawQueue.Count);
        while (_primitiveDrawQueue.Count > 0)
        {
            TrailDrawer.PreparePrimitives(_primitiveDrawQueue.Dequeue(), GetTrailColor, GetTrailWidth);
        }

        TrailDrawer.DrawCached(PrepareShader());
    }

    public abstract BaseShader PrepareShader();
    public abstract Color GetTrailColor(float completionRatio);
    public abstract float GetTrailWidth(float completionRatio);
}

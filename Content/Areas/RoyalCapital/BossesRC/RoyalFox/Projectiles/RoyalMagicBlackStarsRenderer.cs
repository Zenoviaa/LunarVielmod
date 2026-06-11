using Stellamod.Common.Shaders;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

[Autoload(Side = ModSide.Client)]
public class RoyalMagicBlackStarsRenderer : ModSystem
{
    public struct RoyalMagicMiniDraw
    {
        public Vector2[] points;
        public Func<float, float> trailWidthFunction;
        public Func<float, Color> trailColorFunction;
    }

    private Queue<RoyalMagicMiniDraw> _primitiveDrawQueue;
    public override void Load()
    {
        base.Load();
        _primitiveDrawQueue = new();
        On_Main.CheckMonoliths += QueueDraws;
    }

    private void QueueDraws(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        QueueDraws();
    }

    public override void Unload()
    {
        base.Unload();
        _primitiveDrawQueue.Clear();
        _primitiveDrawQueue = null;
        On_Main.CheckMonoliths -= QueueDraws;
    }
    private void QueueDraws()
    {
        if (_primitiveDrawQueue.Count <= 0)
            return;

        // Main.NewText("E");
        RoyalMagicRenderer.Queue(DrawPrimitives);
    }

    public void PreparePoints(RoyalMagicMiniDraw points) => _primitiveDrawQueue.Enqueue(points);
    public static void Queue(RoyalMagicMiniDraw points)
    {
        RoyalMagicBlackStarsRenderer instance = ModContent.GetInstance<RoyalMagicBlackStarsRenderer>();
        instance.PreparePoints(points);
    }

    public void DrawPrimitives(GraphicsDevice graphicsDevice)
    {
        TrailDrawer.ClearPrimitives();
        //Main.NewText(_primitiveDrawQueue.Count);
        while (_primitiveDrawQueue.Count > 0)
        {
            var e = _primitiveDrawQueue.Dequeue();
            TrailDrawer.PreparePrimitives(e.points, e.trailColorFunction, e.trailWidthFunction);
        }
        BasicLaserAlphaShader alphaShader = ShaderContent.GetInstance<BasicLaserAlphaShader>();
        alphaShader.LaserTexture = TrailRegistry.LightningTrail3;
        TrailDrawer.DrawCached(alphaShader);
    }
}

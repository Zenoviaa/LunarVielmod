using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public record struct TrailCircleDraw(VertexPositionColorTexture[] vertices, short[] indices);
public record struct TrailCircleStep(float progressInTrail, float progressInCircle);

public struct PixelPrimitiveCircleParams
{
    public Func<TrailCircleStep, Color> colorFunction;
    public Func<TrailCircleStep, float> widthFunction;
    public float time;
    public float minRadius;
    public float maxRadius;
}

public class PixelCircleUpdater : ModSystem
{
    private static readonly List<PixelPrimitiveCircle> _pixelPrimitiveCircles = new();
    public override void Load()
    {
        base.Load();
        On_Main.DrawProjectiles += QueueCircleDraws;
    }
    public override void Unload()
    {
        base.Unload();
        _pixelPrimitiveCircles.Clear();
        On_Main.DrawProjectiles -= QueueCircleDraws;
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        foreach (var item in _pixelPrimitiveCircles)
        {
            item.Update();
        }
        _pixelPrimitiveCircles.RemoveAll(x => !x.active);
    }
    private void QueueCircleDraws(On_Main.orig_DrawProjectiles orig, Main self)
    {
        orig(self);
        foreach (var item in _pixelPrimitiveCircles)
        {
            item.QueueDraw();
        }
    }


    public static PixelPrimitiveCircle Create(
        in RenderVertices renderFunction,
        in Vector2 position,
        in Func<TrailCircleStep, float> widthFunction,
        in Func<TrailCircleStep, Color> colorFunction,
        in float minRadius,
        in float maxRadius,
        in float time)
    {
        var circle = new PixelPrimitiveCircle();
        circle.position = position;
        circle.circleParams.minRadius = minRadius;
        circle.circleParams.maxRadius = maxRadius;
        circle.circleParams.time = time;
        circle.circleParams.widthFunction = widthFunction;
        circle.circleParams.colorFunction = colorFunction;
        circle.renderPixelPrimitivesFunction = renderFunction;
        circle.active = true;
        _pixelPrimitiveCircles.Add(circle);
        return circle;
    }

    public void Add(PixelPrimitiveCircle circle)
    {
        circle.active = true;
        _pixelPrimitiveCircles.Add(circle);
    }
}

public delegate void CalculateVertices(in PixelPrimitiveCircleParams circleParams);
public delegate void RenderVertices(TrailCircleDraw draw);
public class PixelPrimitiveCircle
{
    private static Vector2[] _circlePoints = new Vector2[128];


    public float timer;
    public PixelPrimitiveCircleParams circleParams;
    public Entity parent;
    public Vector2 position;
    public bool active;
    public CalculateVertices circlePointsFunction;
    public RenderVertices renderPixelPrimitivesFunction;

    public PixelPrimitiveCircle()
    {

    }

    public void DefaultCirclePointsFunction(ref Vector2[] points, in PixelPrimitiveCircleParams circleParams)
    {
        float completionRatio = timer / circleParams.time;
        float radius = MathHelper.Lerp(circleParams.minRadius, circleParams.maxRadius, EasingFunction.InOutSine(completionRatio));
        float maxRadians = MathHelper.ToRadians(360);
        if (parent != null)
        {
            position = parent.Center;
        }
       
       
        for (int f = 0; f < points.Length; f++)
        {
            float ratio = f / (float)(points.Length - 1);
            ref Vector2 point = ref points[f];

            float radians = ratio * maxRadians;
            float x = MathF.Sin(radians) * radius;
            float y = MathF.Cos(radians) * radius;

            point = position + new Vector2(x, y);
           
        }
    }

    public void Update()
    {
        timer++;
       
        if (timer >= circleParams.time)
        {
            active = false;
        }
    }

    public void QueueDraw()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelPrimitives);
    }

    public void RenderPixelPrimitives(GraphicsDevice graphicsDevice)
    {

        if (renderPixelPrimitivesFunction == null)
            return;

        //Calculate the points at which we render at
        DefaultCirclePointsFunction(ref _circlePoints, in circleParams);

        //calculate verts
        float progressInCircle = timer / circleParams.time;
        var vertices = DrawUtilities.PreparedWrappedTrailing(_circlePoints,
             (float progress) => { return circleParams.colorFunction(new TrailCircleStep(progress, progressInCircle)); },
             (float progress) => { return circleParams.widthFunction(new TrailCircleStep(progress, progressInCircle)); });


        var indices = DrawUtilities.PrepareIndicesForDrawing(vertices.Length / 4);
        renderPixelPrimitivesFunction(new TrailCircleDraw(vertices, indices));
    }
}

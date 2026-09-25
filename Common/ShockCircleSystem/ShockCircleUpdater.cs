using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ShockCircleSystem;

public struct ShockCircleData
{
    public Asset<Texture2D> textureAsset;
    public Func<float, float> easing;
    public Func<float, Color> colorOverTime;
    public Vector2 position;
    public float startScale;
    public float endScale;
    public float time;

    public float GetRadius(float uneasedProgress)
    {
        return MathHelper.Lerp(startScale, endScale, easing(uneasedProgress));
    }

    public Color GetColor(float uneasedProgress)
    {
        return colorOverTime(easing(uneasedProgress));
    }
}

public class ShockCircle
{
    public ShockCircleData data;
    public float timer;
    public void Update()
    {
        timer++;
    }
}

public class ShockCircleUpdater : ModSystem
{
    private static readonly List<ShockCircle> _circles = new();
    public static ShockCircle Create(ShockCircleData circleData)
    {
        ShockCircle circle = new();
        circle.data = circleData;
        _circles.Add(circle);
        return circle;
    }

    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderPixelatedShockCircles;
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        foreach(var circle in _circles)
        {
            circle.Update();
        }
        _circles.RemoveAll(x => x.timer >= x.data.time);
        
        /*
        if(Main.mouseLeft && Main.mouseLeftRelease)
        {
            Create(new()
            {
                position = Main.MouseWorld,
                time = 120,
                startScale = 0.1f,
                endScale = 1,
                colorOverTime = (float f) => Color.Lerp(Color.White, Color.SkyBlue, f) * MathHelper.Lerp(1f, 0f, f),
                easing = EasingFunction.OutExpo,
                textureAsset = AssetReferences.Assets.NoiseTextures.BeamTrail.Asset
            });
        }*/
    }
    private void RenderPixelatedShockCircles(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        if(_circles.Count >= 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedCircles);
        }
    }

    private void DrawPixelatedCircles(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var circlePass = AssetReferences.Effects.Generic.ShockCircle.CreatePixelPass();
        circlePass.Parameters.time = Main.GlobalTimeWrappedHourly;
        circlePass.Apply();
        using(spriteBatch.Ctx(spriteBatch.Parameters with { effect = circlePass.Shader }))
        {
            foreach (var circle in _circles)
            {
                var uneasedProgress = circle.timer / circle.data.time;
                var drawer = SpritebatchDrawer.FromTextureAsset(circle.data.textureAsset, circle.data.position);
                drawer.color = circle.data.GetColor(uneasedProgress);

                float b = (circle.data.easing(uneasedProgress) * 255);
                drawer.color.A = (byte)b;
                drawer.scale = new Vector2(circle.data.GetRadius(uneasedProgress));
  
                spriteBatch.Draw(drawer);
            }
        }
    }
}

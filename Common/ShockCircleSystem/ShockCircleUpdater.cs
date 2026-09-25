using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ShockCircleSystem;


[Autoload(Side = ModSide.Client)]
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
        foreach (var circle in _circles)
        {
            circle.Update();
        }
        _circles.RemoveAll(x => x.timer >= x.data.time);
    }
    private void RenderPixelatedShockCircles(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        if (_circles.Count >= 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedCircles);
        }
    }

    private void DrawPixelatedCircles(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var circlePass = AssetReferences.Effects.Generic.ShockCircle.CreatePixelPass();
        circlePass.Parameters.time = Main.GlobalTimeWrappedHourly;
        circlePass.Apply();
        using (spriteBatch.Ctx(spriteBatch.Parameters with { effect = circlePass.Shader }))
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

using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.DungeonFogSystem;


public record struct DungeonFog(Rectangle rectangle, float alpha);
[Autoload(Side = ModSide.Client)]
public class DungeonFogSystem : ModSystem
{
    /// <summary>
    /// The rectangles to draw fog within
    /// </summary>
    public static readonly List<DungeonFog> FogRectangles = new();

    //Called when the fog rectangles has just been cleared and needs new rectangles
    public static event Action<List<DungeonFog>> OnPrepareFog;
    public override void OnModLoad()
    {
        base.OnModLoad();
        On_Main.CheckMonoliths += PrepareRectangles;
        On_Main.DrawInfernoRings += RenderFog;
    }

    private void PrepareRectangles(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;

        FogRectangles.Clear();
        OnPrepareFog?.Invoke(FogRectangles);
    }

    private void RenderFog(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (FogRectangles.Count <= 0)
            return;
        Vector2 texSize = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Size();
        //Render fog here
        //We need our shader
        var pass = AssetReferences.Effects.Generic.ThickTempleFog.CreatePixelPass();
        pass.Parameters.time = -Main.GlobalTimeWrappedHourly * 0.1f;
        pass.Parameters.texelSize = Vector2.One / texSize;
        pass.Parameters.cloudColor = Color.DarkBlue.ToVector4();
        pass.Apply();
        using (new SpritebatchContext(Main.spriteBatch, SpritebatchParams.InWorldAndZoomed() with
        {
            effect = pass.Shader,
            samplerState = SamplerState.LinearWrap
        }))
        {
            
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.Clouds.Asset, Vector2.Zero);
            drawer.drawOrigin = Vector2.Zero;

            foreach (var fog in FogRectangles)
            {
                var rect = fog.rectangle;
                drawer.sourceRect = null;

                Rectangle drawRect = fog.rectangle;
                drawRect.Location -= new Point((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
                drawer.dstRect = drawRect;
                drawer.color = Color.Lerp(Color.DarkBlue, Color.White, 0.2f) * fog.alpha;
                Main.spriteBatch.Draw(drawer);
            }
        }
    }
}

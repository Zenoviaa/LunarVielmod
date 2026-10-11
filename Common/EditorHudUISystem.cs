using Stellamod.Content.BuildingTools;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Common;

[Autoload(Side = ModSide.Client)]
public class EditorHudUISystem : ModSystem
{
    private HashSet<Point> _visited;
    public override void Load()
    {
        base.Load();
        On_Main.DrawInfernoRings += DrawOverWater;
    }

    private void DrawOverWater(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        Player player = Main.LocalPlayer;
        if (player.HeldItem.type == ModContent.ItemType<TileEyeDropper>())
        {
            SpriteBatch sb = Main.spriteBatch;
            SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
            drawer2.color = Color.Green * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 6);
            drawer2.worldPosition = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates();
            sb.Draw(drawer2);
        }

        if (player.HeldItem.type != ModContent.ItemType<MagicPaintBucket>())
            return;

        Point tilePoint = Main.MouseWorld.ToTileCoordinates();
        if (Main.GameUpdateCount % 12 == 0)
        {
            MagicTileUtility.GetFloodCreateTiles(player, out var selected);
            (int loops, HashSet<Point> visited) = MagicTileUtility.GetAffectedPoints(tilePoint, selected);
            _visited = visited;
        }

        if (_visited == null)
            return;

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
        drawer.color = Color.Green * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        foreach (Point p in _visited)
        {
            drawer.worldPosition = p.ToWorldCoordinates();
            Main.spriteBatch.Draw(drawer);
        }
    }
}

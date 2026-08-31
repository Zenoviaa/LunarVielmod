using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssBiomeTileGlow : GlobalTile
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        if (!Main.LocalPlayer.ZoneAbyss)
            return;
        Tile tile = Main.tile[i, j];
        if (WorldGen.TileIsExposedToAir(i, j))
        {
            r = 0.5f;
            g = 0.51f;
            b = 0.8f;
        }
    }
}

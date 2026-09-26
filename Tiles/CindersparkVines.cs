using System;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles;

public class CindersparkVines : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileCut[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileNoFail[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;


        TileID.Sets.VineThreads[Type] = true;
        TileID.Sets.IsVine[Type] = true;

        HitSound = SoundID.Grass;
        DustType = DustID.Torch;

        AddMapEntry(new Color(293, 86, 93));
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = 4;

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        Tile tile = Framing.GetTileSafely(i, j + 1);
        if (tile.HasTile && tile.TileType == Type)
        {
            WorldGen.KillTile(i, j + 1);
        }
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = .255f;
        g = .077f;
        b = .102f;
    }
    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        Tile tileAbove = Framing.GetTileSafely(i, j - 1);
        int type = -1;
        if (tileAbove.HasTile && !tileAbove.BottomSlope)
        {
            type = tileAbove.TileType;
        }

        if (type == ModContent.TileType<CindersparkDirt>() || type == Type)
        {
            return true;
        }

        WorldGen.KillTile(i, j);
        return true;
    }

    public override void RandomUpdate(int i, int j)
    {

    }



    public float GetOffset(int i, int j, int frameX, float sOffset = 0f)
    {
        float sin = (float)Math.Sin((Main.time + (i * 24) + (j * 19)) * (0.04f * (!Lighting.NotRetro ? 0f : 1)) + sOffset) * 1.4f;
        if (Framing.GetTileSafely(i, j - 1).TileType != Type) //Adjusts the sine wave offset to make it look nicer when closer to ground
            sin *= 0.25f;
        else if (Framing.GetTileSafely(i, j - 2).TileType != Type)
            sin *= 0.5f;
        else if (Framing.GetTileSafely(i, j - 3).TileType != Type)
            sin *= 0.75f;

        return sin;
    }
}
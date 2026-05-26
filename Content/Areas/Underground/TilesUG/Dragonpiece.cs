using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.TilesUG;

public class Dragonpiece : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.Orange;
        Item.DefaultToPlaceableTile(ModContent.TileType<DragonpieceOre>());
    }
}

public class DragonpieceOre : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileShine[Type] = 45000;
        Main.tileLighted[Type] = true;
        TileID.Sets.Ore[Type] = true;
        Main.tileLighted[Type] = true;
        HitSound = new SoundStyle("Stellamod/Assets/Sounds/HardRockHit") with { PitchVariance = 0.8f };
        DustType = DustID.Torch;
        MineResist = 1f;

        RegisterItemDrop(ModContent.ItemType<Dragonpiece>());
        AddMapEntry(new Color(125, 25, 25));
    }
    public override void Unload()
    {
        base.Unload();
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
        r = MathF.Max(r, ExtraMath.Osc(0.25f, 0.75f, speed: 2, PreGeneratedNoise.SampleSimplexNoise(i, j) * 8));

    }

    public override bool CanExplode(int i, int j) => true;
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return base.PreDraw(i, j, spriteBatch);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
    }
}

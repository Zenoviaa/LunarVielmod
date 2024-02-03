
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

using Stellamod.Dusts;
using Stellamod.Items.Weapons.PowdersItem;

using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;




namespace Stellamod.Tiles
{
    public class VeilScripture : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            MineResist = 4f;
            MinPick = 25;

            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Sun Alter");
            AddMapEntry(new Color(220, 126, 58));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .354f * 3;
            g = .000f * 3;
            b = .255f * 3;
        }

        public override bool CanExplode(int i, int j) => false;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    }
}
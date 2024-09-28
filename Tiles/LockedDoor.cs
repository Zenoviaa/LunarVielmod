using Stellamod.Dusts;
using Stellamod.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Stellamod.Tiles
{
    public abstract class LockedDoor : ModTile
    {
        public abstract int KeyType { get; }
        public abstract string FailString { get; }
        public abstract Color FailColor { get; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.NotReallySolid[Type] = true;
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = ModContent.DustType<Sparkle>();

            // Names
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.Width = 2;

            MineResist = 8f;
            MinPick = 200;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
            TileObjectData.newTile.StyleMultiplier = 2; //same as above
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(126, 77, 59), name);
            AddMapEntry(new Color(100, 122, 111), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.ClosedDoor };
            TileID.Sets.HasOutlines[Type] = false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.HasItem(KeyType))
            {
                WorldGen.KillTile(i, j);
                NetMessage.SendTileSquare(Main.myPlayer, i, j);
                return true;
            }
            else
            {
                Main.NewText(FailString, FailColor);
            }

            return true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = KeyType;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

        }
    }
}

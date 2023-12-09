
using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Items.Consumables;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Catacombs
{
    //TODO: Smart Cursor Outlines and tModLoader support
    public class CatacombsDoorClosed : ModTile
	{
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

			// Placement
			// In addition to copying from the TileObjectData.Something templates, modders can copy from specific tile types. CopyFrom won't copy subtile data, so style specific properties won't be copied, such as how Obsidian doors are immune to lava.

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
			// name.SetDefault("BarS");
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

			int key = ModContent.ItemType<CatacombsKey>();
			if (player.HasItem(key))
			{




				WorldGen.KillTile(i, j);
				// SoundEngine.PlaySound(SoundID.Roar);
				return true;
			}
			else
			{
				Main.NewText("Kill wall of flesh! Key needed!", Color.Gold);


			}
			


			return true;
		}



		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<CatacombsKey>();
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			
		}
	}
}

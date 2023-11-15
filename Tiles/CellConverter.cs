
using Stellamod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles
{
    public class CellConverter : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Cell Converter");
			AddMapEntry(Colors.RarityAmber, name);
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, Mod.Find<ModItem>("CellConverterI").Type);
        }
		public override bool CanExplode(int i, int j) => false;
		public override void MouseOver(int i, int j)
		{
			//shows the Cryptic Crystal icon while mousing over this tile
			Main.player[Main.myPlayer].cursorItemIconEnabled = true;
			Main.player[Main.myPlayer].cursorItemIconID = ModContent.ItemType<LostScrap>();
		}


		public override bool RightClick(int i, int j)
		{
			//check if player has a Cryptic Crystal
			Player player = Main.player[Main.myPlayer];
			if (player.HasItem(ModContent.ItemType<LostScrap>()))
			{
				//now to search for it
				Item[] inventory = player.inventory;
				for (int k = 0; k < inventory.Length; k++)
				{
					if (inventory[k].type == ModContent.ItemType<LostScrap>())
                    {
                        var entitySource = player.GetSource_OpenItem(Type);
                        //consume it, and summon the Crystal King!
                        inventory[k].stack--;
						int Choose = Main.rand.Next(6);
						if (Choose == 0)                                                
						{
							player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("MeleeDrive").Type);
						}
						if (Choose == 1)                                                 
						{
							player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("MagicDrive").Type);
						}
						if (Choose == 2)                                                  
						{
							player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("RangerDrive").Type);
						}
						if (Choose == 3)                                                
						{
							player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("SummonerDrive").Type);
                        }
                        if (Choose == 4)
                        {
                            player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("ArmorDrive").Type);
                        }
                        if (Choose == 5)
                        {
                            player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("ToolDrive").Type);
                        }
                        return true;
					}
				}
			}
			return false;
		}
	}
}

using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.GothiviaNRek.Gothivia;
using Stellamod.NPCs.Bosses.GothiviaNRek.Reks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;



namespace Stellamod.Tiles.Structures.AlcadizNGovheil

{
    public class Gothiv : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileTable[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
		
			// This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
			MineResist = 4f;
			MinPick = 200;

			DustType = DustID.SilverCoin;
			AdjTiles = new int[] { TileID.Bookcases };
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 7;
			TileObjectData.newTile.Width = 7;



			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();


			// name.SetDefault("Hunter's Curtains");
			AddMapEntry(new Color(147, 149, 93), name);
		}
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

		public override bool CanExplode(int i, int j) => false;
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public bool Checked = false;
		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			if (!NPC.AnyNPCs(ModContent.NPCType<Gothiviab>()) 
				&& !NPC.AnyNPCs(ModContent.NPCType<Rek>()) 
				&& !NPC.AnyNPCs(ModContent.NPCType<Gothiviabb>()))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int npcID = NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16, j * 16, ModContent.NPCType<Gothiviabb>());
					Main.npc[npcID].netUpdate2 = true;
				}
				else
				{
					if (Main.netMode == NetmodeID.SinglePlayer)
						return false;

					StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<Gothiviabb>(), i * 16, (j * 16));
				}
			}
			else if (NPC.AnyNPCs(ModContent.NPCType<Gothiviab>()) || NPC.AnyNPCs(ModContent.NPCType<Gothiviabb>()))
			{
				Main.NewText("...", Color.Gold);
			}

			return true;
		}

	
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;

			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<GothiviasSeal>();
			if (tile.TileFrameX % 36 != 0)
			{
				left--;
			}

			if (tile.TileFrameY != 0)
			{
				top--;
			}

			int chest = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chest < 0)
			{
				player.cursorItemIconText = LangText.Misc("Gothiv");
			}
			else
			{
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); /* tModPorter Note: new method takes in FrameX and FrameY */; // This gets the ContainerName text for the currently selected language
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = ModContent.ItemType<GothiviasSeal>();
					if (Main.tile[left, top].TileFrameX / 36 == 1)
					{
						player.cursorItemIconID = ModContent.ItemType<GothiviasSeal>();
					}

					player.cursorItemIconText = "";
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}


		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.2f;
			g = 0.165f;
			b = 0.12f;
		}

	

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Vector2 pos = new Vector2(i, j) * 16;
			Lighting.AddLight(pos, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);

			if (Main.rand.NextBool(100))
			{
				if (!Main.tile[i, j - 1].HasTile)
				{
					Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
						ModContent.DustType<Sparkle>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 2f));

				}
			}
		}

	}
}
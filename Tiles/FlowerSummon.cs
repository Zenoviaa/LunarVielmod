using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles
{

    public class FlowerSummon : ModTile
	{
		public override LocalizedText DefaultContainerName(int frameX, int frameY)
		{
			int option = frameX / 36;
			return this.GetLocalization("MapEntry" + option);
		}

		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// Properties
		

			DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.Containers };

			// Names
		
			// name.SetDefault("Morrowed Plants");


			Main.tileTable[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Sparkle>();
			DustType = ModContent.DustType<Dusts.SalfaceDust>();
			AdjTiles = new int[] { TileID.Bookcases };
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 5;
			MineResist = 4f;
			MinPick = 80;
			TileObjectData.newTile.DrawYOffset = 16; // So the tile sinks into the ground
			TileObjectData.newTile.DrawXOffset = 4; // So the tile sinks into the ground


			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}


		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}


		public bool Checked = false;
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;

			Main.LocalPlayer.cursorItemIconEnabled = true;
		
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
				player.cursorItemIconText = LangText.Misc("FlowerSummon.2");
			}
			else
			{
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); /* tModPorter Note: new method takes in FrameX and FrameY */; // This gets the ContainerName text for the currently selected language
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = ModContent.ItemType<Flowersummon>();
					if (Main.tile[left, top].TileFrameX / 36 == 1)
					{
						player.cursorItemIconID = ModContent.ItemType<StoneKey>();
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
			r = 0.215f;
			g = 0.165f;
			b = 0.1f;
		}


		public override bool RightClick(int i, int j)
		{

			if (NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>()) || NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>())) //Do nothing if the boss is alive
				return false;

			Player player = Main.LocalPlayer;

	








			if (!NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>()))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Main.NewText(LangText.Misc("FlowerSummon.1"), Color.Gold);
					int npcID = NPC.NewNPC(new EntitySource_TileBreak(i + 10, j), i * 16, j * 16, ModContent.NPCType<StarrVeriplant>());
					Main.npc[npcID].netUpdate2 = true;
				}
				else
				{
					if (Main.netMode == NetmodeID.SinglePlayer)
						return false;

					StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<StarrVeriplant>(), i * 16, (j * 16) - 5);
				}

				return true;
			}
			if (NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>()))
			{

				Main.NewText("...", Color.Gold);



			}
			


			return true;

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
						ModContent.DustType<Sparkle>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.2f, 0.15f, 0.08f, 0f), Main.rand.NextFloat(0.25f, 2f));

				}
			}
		}
	}
	}





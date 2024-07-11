using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable.Cathedral;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.Sylia;
using Stellamod.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Structures.UnderworldRuins
{

    public class UnstableRift : ModTile
	{
		public override LocalizedText DefaultContainerName(int frameX, int frameY)
		{
			int option = frameX / 36;
			return this.GetLocalization("MapEntry" + option);
		}

		public override void SetStaticDefaults()
		{
			// Properties
		

			DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.Containers };

			CreateMapEntryName();

			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Sparkle>();
			DustType = ModContent.DustType<Dusts.SalfaceDust>();
			AdjTiles = new int[] { TileID.Bookcases };
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Width = 3;
			MineResist = 8f;
			MinPick = 200;
			TileObjectData.newTile.DrawYOffset = 6; // So the tile sinks into the ground
			//TileObjectData.newTile.DrawXOffset = -4; // So the tile sinks into the ground
			Main.tileBlockLight[Type] = true;


			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);


			Main.tileOreFinderPriority[Type] = 800;
			TileID.Sets.HasOutlines[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;
			//AddMapEntry()
		}



		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override bool CanExplode(int i, int j) => false;

		public override bool RightClick(int i, int j)
		{
			if (NPC.AnyNPCs(ModContent.NPCType<Sylia>())) //Do nothing if the boss is alive
				return false;

            if (!Main.hardMode)
            {
				Main.NewText(LangText.Misc("UnstableRift.1"), Color.Red);
			} else if (!NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Main.NewText(LangText.Misc("UnstableRift.2"), Color.Purple);
					int npcID = NPC.NewNPC(new EntitySource_TileBreak(i + 10, j), i * 16, j * 16, ModContent.NPCType<Sylia>());
					Main.npc[npcID].netUpdate2 = true;
				}
				else
				{
					if (Main.netMode == NetmodeID.SinglePlayer)
						return false;

					StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<Sylia>(), i * 16, (j * 16) - 5);
				}

				return true;
			}

			if (NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
			{
				Main.NewText("...", Color.Purple);
			}

			return true;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) { }
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;

			Main.LocalPlayer.cursorItemIconEnabled = true;
			//Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<WanderingEssence>();
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
				player.cursorItemIconText = LangText.Misc("UnstableRift.3");
			}
			else
			{
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); /* tModPorter Note: new method takes in FrameX and FrameY */; // This gets the ContainerName text for the currently selected language
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = ModContent.ItemType<ShrineI>();
					if (Main.tile[left, top].TileFrameX / 36 == 1)
					{
						player.cursorItemIconID = ModContent.ItemType<MoonflameLantern>();
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

			if (Main.rand.NextBool(50))
			{
				if (!Main.tile[i, j - 1].HasTile)
				{
					int bodyParticleCount = 2;
					float bodyRadius = 2;
					for (int b = 0; b < bodyParticleCount; b++)
					{
						Vector2 position = pos + Main.rand.NextVector2Circular(bodyRadius / 2, bodyRadius / 2);
						Vector2 vel = new Vector2(0, -1);
						float size = Main.rand.NextFloat(0.25f, 0.3f);
						Particle p = ParticleManager.NewParticle(position, vel, ParticleManager.NewInstance<VoidParticle>(),
							default(Color), size);

						p.layer = Particle.Layer.BeforeProjectiles;
						Particle tearParticle = ParticleManager.NewParticle(position, vel, ParticleManager.NewInstance<VoidTearParticle>(),
							default(Color), size + 0.025f);

						tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
					}
				}
			}
		}
	}
}




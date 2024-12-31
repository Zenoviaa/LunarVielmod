using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable.Cathedral;
using Stellamod.NPCs.Bosses.Caeva;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Town;
using Stellamod.Projectiles;
using Stellamod.Projectiles.IgniterExplosions;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Structures.AlcadizNGovheil
{

    public class LabCrystal : ModTile
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


			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Shrine of The Moon");



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
			Player player = Main.LocalPlayer;
			Vector2[] altarPositions = null;

            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.TileFrameX != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            bool makePortal = false;
            if (player.HeldItem.type == ModContent.ItemType<FocalCrystalFire>())
			{
				altarPositions = TeleportSystem.FireDungeonAltarWorld;
				makePortal = true;
            } 
			else if(player.HeldItem.type == ModContent.ItemType<FocalCrystalWater>())
			{
                altarPositions = TeleportSystem.WaterDungeonAltarWorld;
                makePortal = true;
            } 
			else if(player.HeldItem.type == ModContent.ItemType<FocalCrystalTrap>())
			{
				altarPositions = TeleportSystem.TrapDungeonAltarWorld;
                makePortal = true;
            }

			if (makePortal)
			{
                Vector2 altar = altarPositions[Main.rand.Next(0, altarPositions.Length)];
                if (StellaMultiplayer.IsHost)
				{   
                    TeleportSystem.CreatePortal(altar, left, top);
					TeleportSystem.RefreshPortals();
                } 
				else
				{
                    Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.CreatePortal,
                        altar.X,
						altar.Y,
                        left,
                        top).Send(-1);
                }

 
                Vector2 portalPosition = new Vector2((left+ 1) * 16, (top - 6) * 16);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_PreSpawn2"), player.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_PreSpawn"), player.position);
                Projectile.NewProjectile(player.GetSource_FromThis(), portalPosition, Vector2.Zero,
					ModContent.ProjectileType<KaBoomMagic2>(), 0, 1, player.whoAmI);
				player.GetModPlayer<MyPlayer>().ShakeAtPosition(player.position, 1024, 32);

                for (int h = 0; h < 48; h++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(4f, 4f);
                    Dust.NewDustPerfect(portalPosition, DustID.Electric, speed);
                }

                player.velocity.X = -player.direction * 6f;
                player.velocity.Y = -9f;
                player.HeldItem.TurnToAir();
            }

            return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;

            player.cursorItemIconEnabled = true;
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
				player.cursorItemIconText = LangText.Misc("LabCrystal");
				if(player.HeldItem.type == ModContent.ItemType<FocalCrystalFire>() ||
                    player.HeldItem.type == ModContent.ItemType<FocalCrystalTrap>() ||
                    player.HeldItem.type == ModContent.ItemType<FocalCrystalWater>())
				{
					player.cursorItemIconID = player.HeldItem.type;
				} 
			}
			else
			{
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); /* tModPorter Note: new method takes in FrameX and FrameY */; // This gets the ContainerName text for the currently selected language
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = Main.LocalPlayer.HeldItem.type;
                    if (Main.tile[left, top].TileFrameX / 36 == 1)
					{
						player.cursorItemIconID = Main.LocalPlayer.HeldItem.type;
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

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];

			if (tile == null || !tile.HasTile) { return false; }

			Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Particles/GradientPillar").Value;

			Vector2 offScreen = new Vector2(Main.offScreenRange);
			Vector2 globalPosition = p.ToWorldCoordinates(0f, 0f);
			Vector2 position = globalPosition + offScreen - Main.screenPosition + new Vector2(-6, -100f + 16f);
			Color color = new Color(0.02f, 0.03f, 0.01f, 0f) * (2 * (((float)Math.Sin(Main.GameUpdateCount * 0.02f) + 4) / 4));

			Main.EntitySpriteDraw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

			return true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Vector2 pos = new Vector2(i, j) * 16;
			Lighting.AddLight(pos, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);

			if (Main.rand.NextBool(50))
			{
				if (!Main.tile[i, j - 1].HasTile)
				{
					Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
						DustID.CursedTorch, new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 2f));

				}
			}
		}
	}
}




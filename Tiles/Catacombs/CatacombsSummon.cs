using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.singularityFragment;
using Stellamod.NPCs.Catacombs;
using Stellamod.NPCs.Catacombs.Fire;
using Stellamod.NPCs.Catacombs.Fire.BlazingSerpent;
using Stellamod.NPCs.Catacombs.Trap.Cogwork;
using Stellamod.NPCs.Catacombs.Trap.Sparn;
using Stellamod.NPCs.Catacombs.Water.WaterCogwork;
using Stellamod.NPCs.Catacombs.Water.WaterJellyfish;
using Stellamod.NPCs.Minibosses;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace Stellamod.Tiles.Catacombs
{
    internal class CatacombsSummon : ModTile
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
			LocalizedText name = CreateMapEntryName();
 
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Sparkle>();
			DustType = ModContent.DustType<Dusts.SalfaceDust>();
			AdjTiles = new int[] { TileID.Bookcases };
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 2;
			MineResist = 8f;
			MinPick = 200;
			TileObjectData.newTile.DrawYOffset = 6; // So the tile sinks into the ground
			Main.tileBlockLight[Type] = true;

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);

			Main.tileOreFinderPriority[Type] = 800;
			TileID.Sets.DisableSmartCursor[Type] = true;
		}
		
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .154f * 3;
            g = .010f * 3;
            b = .355f * 3;
        }

        public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			int key = ModContent.ItemType<CursedShard>();

			if (player.HasItem(key))
			{

				player.RemoveItem(key);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int npcID = NPC.NewNPC(new EntitySource_TileBreak(i + 10, j), i * 16, j * 16, ModContent.NPCType<CatacombsBossSpawn>());
                    Main.npc[npcID].netUpdate2 = true;
                }
                else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return false;

                    StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<CatacombsBossSpawn>(), i * 16, (j * 16) - 5);
                }
                return true;
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
			player.cursorItemIconID = ModContent.ItemType<CursedShard>();
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{

		}
		public override bool CanExplode(int i, int j) => false;
	}
}
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Stellamod.Dusts;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Tiles
{
	
	public class FlowerSummon : ModTile
	{

		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// Properties
		

			DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.Containers };

			// Names
			ContainerName.SetDefault("Morrowed Plants");

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Morrowed Plants");


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
			TileObjectData.newTile.DrawYOffset = 6; // So the tile sinks into the ground
			TileObjectData.newTile.DrawXOffset = 4; // So the tile sinks into the ground


			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);
		}

		

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}


		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}


		public bool Checked = false;
		public override bool RightClick(int i, int j)
		{
			
			if (!NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>()))
            {
		
		
			NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16, j * 16, ModContent.NPCType<StarrVeriplant>());
			SoundEngine.PlaySound(SoundID.Roar);
		
	}
				
			

			return true;
		}
		public override void MouseOver(int i, int j)
		{
			Checked = true;
 
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];

			Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Tiles/FlowerSummon_Highlight").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			
			if (Checked == true)
            {
				spriteBatch.Draw(texture, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
			}
			 timer++;
			if (timer == 40)
            {
				Checked = false;
				timer = 0;
			}
	}
	}
}




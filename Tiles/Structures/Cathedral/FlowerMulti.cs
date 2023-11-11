
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Structures.Cathedral
{
    public class FlowerMulti : ModTile
	{
		public const int FlowerVerticalFrameCount = 20;
		public override void SetStaticDefaults()
		{
			// If a tile is a light source
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
			MineResist = 4f;
			MinPick = 30;

			DustType = ModContent.DustType<Dusts.SalfaceDust>();
			AdjTiles = new int[] { TileID.Bookcases };
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 9;

			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
			TileObjectData.newTile.StyleMultiplier = 2; //same as above
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();

			// name.SetDefault("Crystal");
			AddMapEntry(new Color(16, 40, 33), name);

			AnimationFrameHeight = 32;
		}

		// Our textures animation frames are arranged horizontally, which isn't typical, so here we specify animationFrameWidth which we use later in AnimateIndividualTile
		//private readonly int animationFrameWidth = 122;

		// This method allows you to determine how much light this block emits
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.93f;
			g = 0.11f;
			b = 0.12f;
		}

		// This method allows you to determine whether or not the tile will draw itself flipped in the world
		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			// Flips the sprite if x coord is odd. Makes the tile more interesting
			if (i % 2 == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;
		}

		
		

		// This method allows you to change the sound a tile makes when hit
		public override bool KillSound(int i, int j, bool fail)
		{
			// Play the glass shattering sound instead of the normal digging sound if the tile is destroyed on this hit
			if (!fail)
			{
				SoundEngine.PlaySound(SoundID.Grass, new Vector2(i, j).ToWorldCoordinates());
				return false;
			}
			return base.KillSound(i, j, fail);
		}

		//TODO: It's better to have an actual class for this example, instead of comments

		

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			
			// Spend 9 ticks on each of 6 frames, looping
			frameCounter++;
			if (frameCounter >= 4) {
				frameCounter = 0;
				if (++frame >= 20) {
					frame = 0;
				}
			}
		

			
		}
		public override bool CanExplode(int i, int j) => false;
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
		
		}
	}

	
}
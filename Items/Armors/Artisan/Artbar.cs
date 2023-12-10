
using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Artisan
{
	// - ModProjectile - the minion itself

	// It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
	// To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// This is NOT an in-depth guide to advanced minion AI

	// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class Artbar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch Of speedy fish");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 30;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults()
		{
			Projectile.originalDamage = (int)0f;
			Projectile.width = 125;
			Projectile.height = 125;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely
											// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.scale = 1f;
		


		}
		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return true;
		}
		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return true;
		}
		
		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			Projectile.Center = owner.Center + new Vector2(0, 96);
			Visuals();
		}
		

        public override void OnKill(int timeLeft)
        {
			for (int i = 0; i < 50; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob1>(), 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[num].noGravity = false;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				}
			}
			for (int i = 0; i < 14; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, DustID.SilverCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
			}
			for (int i = 0; i < 40; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
			}
			for (int i = 0; i < 40; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
			}
			for (int i = 0; i < 20; i++)
			{
				Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
			}
			Projectile.active = false;
		}
	
        private void Visuals()
		{
			// So it will lean slightly towards the direction it's moving
			Player player = Main.player[Projectile.owner];
			// This is a simple "loop through all frames from top to bottom" animation
			int frameSpeed = 6 + player.GetModPlayer<MyPlayer>().PPFrameTime;

			Projectile.frameCounter++;

			if (Projectile.frameCounter >= frameSpeed)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;

				if (Projectile.frame >= Main.projFrames[Projectile.type])
				{
					Projectile.frame = 0;
					Projectile.Kill();
				}
			}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}


}
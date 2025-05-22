
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.StringnNeedles.Verl
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
    public class SwordsArmy3 : ModProjectile
	{
		int afterImgCancelDrawCount = 0;
		//public bool[] hitByThisStardustExplosion = new bool[200] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, };
        float ta = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spragald");
			// Sets the amount of frames this minion has on its spritesheet

			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = false; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public int Timer = 0;

		public override void SetDefaults()
		{
			Projectile.damage = (int)10f;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely
			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.timeLeft = 5000;
			Projectile.scale = 0.5f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
			Projectile.CloneDefaults(ProjectileID.Spazmamini);
			AIType = ProjectileID.Spazmamini;
		}

		// Here you can decide if your minion breaks things like grass or pots	
		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{		
			Projectile.tileCollide = false;
			if (ta > 150)
			{			
				afterImgCancelDrawCount++;
			}

			ta += 0.01f;
		}
	
		public override bool PreDraw(ref Color lightColor)
		{
			Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
			//float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
			afterImgColor.A = 40;
			afterImgColor.B = 96;
			afterImgColor.G = 149;
			afterImgColor.R = 59;

			Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
			{
				//if(i % 2 == 0)
				float rotationToDraw;
				Vector2 interpolatedPos;
				for (float j = 0; j < 1; j += 0.25f)
				{
					if (i == 0)
					{
						rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
						interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
					}
					else
					{
						interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
						rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
					}
					Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
				}
			}
			
			return false;
		}
	}
}

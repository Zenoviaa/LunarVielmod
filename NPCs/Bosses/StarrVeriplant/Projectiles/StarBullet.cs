using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
    public class StarBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star Bullet");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 45;
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 100;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = false;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;
		
		public override void AI()
		{
			Timer2++;
			Projectile.spriteDirection = Projectile.direction;
			Timer++;
			if (Timer == 15)
            {



				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Starblast"));
				float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;


				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabc, Projectile.position.Y + speedYabc - 16, speedXabc * 0, speedYabc - 3 * 2, ModContent.ProjectileType<StarSpike>(), 10, 0f, Projectile.owner, 0f, 0f);
				Timer = 0;


			}

			Projectile.velocity.X *=  0.98f;
			
		Projectile.velocity.Y = 0f;

		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);

		}
		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		

		
	}
}

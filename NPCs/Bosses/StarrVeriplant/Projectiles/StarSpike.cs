using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
	public class StarSpike : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Beaming Star");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 12;
			Projectile.height = 24;
			Projectile.light = 1.5f;
			Projectile.friendly = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = true;
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
			Projectile.velocity.X *= 0;

			Projectile.velocity.Y *= 0.99f;
			Projectile.rotation -= 0.1f;
			Timer++;

			if (Timer == 599)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f);
				Main.dust[dust].scale = 2f;
				Projectile.Kill();	
			}

			int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f);
			Main.dust[dust2].scale = 0.8f;






		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{

			behindNPCs.Add(index);

		}
		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		
		
		
	}
}

		

	

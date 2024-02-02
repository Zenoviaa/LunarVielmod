using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Buffs.PocketDustEffects;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class LiberatorBubble : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IgniterStart");
			Main.projFrames[Projectile.type] = 60;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 53;
			Projectile.height = 53;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 1280;
			Projectile.scale = 1.5f;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 60)
				{
					Projectile.frame = 0;
				}
			}



			Projectile.scale *= 0.96f;






			if (Projectile.scale <= 0)
			{
				Projectile.Kill();
			}















		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null


		public override void PostDraw(Color lightColor)
		{
			Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 1.75f * Main.essScale);
			
		}











	}
}



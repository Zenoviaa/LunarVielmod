using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Powders
{
	public class IllusionistPowderProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Powdered Sepsis");

		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 30;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{

			Projectile.velocity *= 0.97f;
			for (int j = 0; j < 3; j++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(0.5f, 1f);
				ParticleManager.NewParticle(Projectile.Center, speed * 5, ParticleManager.NewInstance<BurnParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));


			}
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f);
			Main.dust[dust].scale = 1.5f;


			return true;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			player.AddBuff(ModContent.BuffType<UseIgniter>(), 720);
			target.AddBuff(ModContent.BuffType<Dusted>(), 720);
			target.AddBuff(ModContent.BuffType<EldritchDust>(), 720);
			base.OnHitNPC(target, hit, damageDone);
		}
	}
}
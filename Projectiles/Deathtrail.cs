using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Deathtrail : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flame Trail");
		}
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = ProjectileID.WoodenArrowFriendly;
			Projectile.timeLeft = 60;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 1;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
		}
		public override bool PreAI()
		{
			int newDust = Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height, DustID.GoldCoin, Main.rand.Next(-3, 4), Main.rand.Next(-3, 4), 100, default, 1f);
			Dust dust = Main.dust[newDust];
			dust.position.X -= 2f;
			dust.position.Y += 2f;
			dust.scale = 2f;
			dust.noGravity = true;
			return false;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, 120);
		}
	}
}
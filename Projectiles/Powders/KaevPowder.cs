using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Powders
{
	public class KaevPowder : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Powdered Blood");
			
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 45;
			Projectile.ignoreWater = true;
		}
        public override void AI()
        {

			Projectile.velocity *= 0.98f;

        }
        public override bool PreAI()
		{
			Projectile.tileCollide = false;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworkFountain_Red, 0f, 0f);
			Main.dust[dust].scale = 1f;


			return true;
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{

			target.AddBuff(ModContent.BuffType<Dusted>(), 1120);
			target.AddBuff(ModContent.BuffType<KaevDust>(), 1120);
			base.OnHitNPC(target, damage, knockback, crit);
		}
	}
}
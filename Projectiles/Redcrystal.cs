using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{

	public class Redcrystal : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Crystal");
		}
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.RainbowCrystalExplosion);
			AIType = ProjectileID.RainbowCrystalExplosion;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.rotation += 0f * (float)Projectile.direction;
			Projectile.scale = 2.5f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 0, 10, 0) * (1f - (float)Projectile.alpha / 255f);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<DeathmultiplierBloodLamp>(), 360);
		}
	}
}
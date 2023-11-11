using Stellamod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Ultras
{
    public class CoralBubble : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coconuts");
		}
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ToxicBubble);
			AIType = ProjectileID.ToxicBubble;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.ignoreWater = true;
			Projectile.scale = 0.9f;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<Wounded>(), 360);
		}
	}
}




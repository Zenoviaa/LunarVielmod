using Stellamod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Gun
{
    public class TociBolt4 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("TociBolt");
		}

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 5;
            Projectile.damage = 30;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 6;
            Projectile.alpha = 255;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = true; //Tells the game whether or not it can collide with a tile
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// This will always be true, so it's a nothing statement
			/*
			if (Main.rand.Next(1) == 0)
				target.AddBuff(Mod.Find<ModBuff>("AcidFlame").Type, 200);
			*/

			//Use ModContent.BuffType<> instead of Mod.Find, it's faster and cleaner
			target.AddBuff(BuffType<AcidFlame>(), 200);
		}

		public override bool PreAI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			int dust4 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			int dust5 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
			Main.dust[dust3].noGravity = true;
			Main.dust[dust4].noGravity = true;
			Main.dust[dust5].noGravity = true;
			Main.dust[dust5].scale = 2f;
			Main.dust[dust4].scale = 1.5f;
			Main.dust[dust3].scale = 1.5f;
			return false;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
		}
	}
}

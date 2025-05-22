using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class Zap : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Zap");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 8;
			Projectile.alpha = 255;
			Projectile.timeLeft = 206;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 2;
		}

		private void Trail(Vector2 from, Vector2 to)
		{
			float distance = Vector2.Distance(from, to);
			float step = 1 / distance;
			for (float w = 0; w < distance; w += 4)
			{
				Dust.NewDustPerfect(Vector2.Lerp(from, to, w * step), 226, Vector2.Zero).noGravity = true;
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Frostburn2, 380);
			target.AddBuff(BuffID.Chilled, 380);
			target.AddBuff(BuffID.Frozen, 380);
		}

        public override bool PreAI()
        {
            Trail(Projectile.position, Projectile.position + Projectile.velocity);
            return true;
        }

		public override void AI()
		{
			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 11)
            {
                var entitySource = Projectile.GetSource_Death();
                Projectile.timeLeft = 0;
				Projectile.frameCounter = 0;
                float rotation = (float)(Main.rand.Next(0, 361) * (Math.PI / 180));
                Vector2 velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                int proj = Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, velocity.X, velocity.Y, ModContent.ProjectileType<Zap>(), Projectile.damage, Projectile.owner, 0, 0f);
                Main.projectile[proj].friendly = true;
                Main.projectile[proj].hostile = false;
                Main.projectile[proj].velocity *= 7f;
            }
		}
	}
}
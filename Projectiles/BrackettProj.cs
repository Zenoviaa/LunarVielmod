
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class BrackettProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plantius");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.BoulderStaffOfEarth);
			AIType = ProjectileID.BoulderStaffOfEarth;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.height = 32;
			Projectile.width = 32;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(2))
				target.AddBuff(BuffID.Poisoned, 180);

			ShakeModSystem.Shake = 4;
			float speedX = Projectile.velocity.X;
			float speedY = Projectile.velocity.Y;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<BrackettThrough>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Saw1"));
			Projectile.Kill();

		}

		public override bool PreAI()
		{
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants);
			}

			return true;
		}

        public override void AI()
        {
			Projectile.rotation += 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 15; i++)
			{
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
			}

			
		}
	}
}
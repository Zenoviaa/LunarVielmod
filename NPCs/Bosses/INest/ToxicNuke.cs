
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest
{
    public class ToxicNuke : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Toxic Nuke");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 10;
			Projectile.timeLeft = 70;
			Projectile.tileCollide = false;
            Projectile.aiStyle = 1;
			Projectile.damage = 60;
			AIType = ProjectileID.Bullet;
		}
		int timer;
		int colortimer;
		public override bool PreAI()
		{
			timer++;
			if (timer <= 50)
			{
				colortimer++;
			}
			if (timer > 50)
			{
				colortimer--;
			}
			if (timer >= 100)
			{
				timer = 0;
			}
			return true;
		}
		public override void AI()
		{
			if (Projectile.timeLeft == 1)
            {
                int Spawn = Main.rand.Next(1, 4 + 1);
				if(Spawn == 1)
                {
                    var entitySource = Projectile.GetSource_FromThis();
                    int SpawnPosX = Main.rand.Next(-800, 800 + 1);
					Projectile.NewProjectile(entitySource, new Vector2(Projectile.Center.X + SpawnPosX, Projectile.Center.Y), new Vector2(0, 40), Mod.Find<ModProjectile>("AcidicZap").Type, Projectile.damage, 0);
				}

			}
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150 + colortimer * 2, 150 + colortimer * 2, 150 + colortimer * 2, 100);
        }

	}
}
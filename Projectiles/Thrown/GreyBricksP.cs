
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class GreyBricksP : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Grey Brick");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
            Projectile.CloneDefaults(ProjectileID.FrostDaggerfish);
            AIType = ProjectileID.FrostDaggerfish;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gray.ToVector3() * 0.75f * Main.essScale);
        }

        public override bool PreAI()
		{
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
			}
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

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
            var EntitySource = Projectile.GetSource_Death();
            int Gore1 = ModContent.Find<ModGore>("Stellamod/GreyBricks1").Type;
            int Gore2 = ModContent.Find<ModGore>("Stellamod/GreyBricks2").Type;
            Gore.NewGore(EntitySource, Projectile.position, Projectile.velocity, Gore1);
            Gore.NewGore(EntitySource, Projectile.position, Projectile.velocity, Gore2);
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
            }
        }
    }
}
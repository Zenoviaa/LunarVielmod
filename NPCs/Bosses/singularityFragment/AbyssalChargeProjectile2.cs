using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    public class AbyssalChargeProjectile2 : ModProjectile
    {
        public bool OptionallySomeCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            // DisplayName.SetDefault("Abyssal Charge");
        }

		public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 350;
			Projectile.height = 32;
			Projectile.width = 32;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		int counter = 6;
		float alphaCounter = 4;
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkBlue, Color.Transparent, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, (Projectile.Center - Main.screenPosition), null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(45f * alphaCounter), 0), Projectile.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, (Projectile.Center - Main.screenPosition), null, new Color((int)(05f * alphaCounter), (int)(05f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (counter + 0.3f * 2), SpriteEffects.None, 0f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.09f;
			Projectile.velocity *= 0.99f;
            if (counter >= 1440)
            {
                counter = -1440;
            }

            if(Projectile.timeLeft == 100)
            {
                Utilities.NewProjectileBetter(Projectile.Center.X, Projectile.Center.Y - 900, 0, 10, 
                    ModContent.ProjectileType<SoulBeam>(), 100, 0f, -1, 0, Projectile.whoAmI);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			for (int i = 0; i < 10; i++)
			{
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, -2f, 0, default(Color), 1.1f);
				Main.dust[num].noGravity = true;
				Dust expr_62_cp_0 = Main.dust[num];
				expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-30, 31) / 20 - 1.5f);
				Dust expr_92_cp_0 = Main.dust[num];
				expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-30, 31) / 20 - 1.5f);
				if (Main.dust[num].position != Projectile.Center)
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				}
			}
		}
	}
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class DirtBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Boralius");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
            Projectile.CloneDefaults(ProjectileID.FrostDaggerfish);
            AIType = ProjectileID.FrostDaggerfish;
            base.Projectile.width = 12;
            base.Projectile.height = 12;
            Projectile.penetrate = 3;
        }

        public override void PostDraw(Color lightColor)
        {

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, 0f, 150, Color.White, 0.4f);
                Main.dust[dustnumber].noGravity = false;
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkSlateGray, Color.Transparent, completionRatio * 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.TerraTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.SaddleBrown, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class ArchariliteArrowSmall : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;
            Projectile.timeLeft = 290;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.extraUpdates += 1;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 1.01f;
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 256);
            if (nearest != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {

                Vector2 velocity = Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, 0, Color.OrangeRed, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
            }
        }

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
            return Color.Lerp(Color.SandyBrown, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
  
        }
    }
}

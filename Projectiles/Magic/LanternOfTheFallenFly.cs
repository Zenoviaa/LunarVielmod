using Stellamod.Content.Dusts;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    //TODO: new fallen fly visual
    public class LanternOfTheFallenFly : ModProjectile, IDrawToRenderTarget
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
            Projectile.timeLeft = 320;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.position, DustID.GreenTorch, Vector2.Zero, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.Turquoise, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            if (Timer < 45)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
            }

            if (Timer == 46)
            {
                Projectile.velocity += Vector2.UnitY;
            }
            if (Timer > 47)
            {
                if (Projectile.velocity.Length() < 1)
                    Projectile.velocity *= 1.02f;
                NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
                if (nearest != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 3);
                }

                Projectile.extraUpdates = (int)MathHelper.Lerp(0, 4, (Timer - 47) / 160f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 1; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Green, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(6, 12),
                    baseSize: Main.rand.NextFloat(0.01f, 0.05f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public void DrawToRenderTargets()
        {

        }
    }
}

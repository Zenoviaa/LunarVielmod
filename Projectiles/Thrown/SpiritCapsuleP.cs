
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class SpiritCapsuleP : ModProjectile
    {
        private float Timer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acidius");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BouncingShield);
            Projectile.width = 8;
            Projectile.height = 8;
            AIType = ProjectileID.BouncingShield;
            Projectile.penetrate = 5;
        }

        public override void PostAI()
        {
            base.PostAI(); ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            Timer++;
            if (Timer % 12 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
                else
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowHeartDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.75f * Main.essScale);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(32, 0f, completionRatio);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Pink, Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private void DrawTrail()
        {
            Main.spriteBatch.RestartDefaults();
            Vector2 drawOffset = -Main.screenPosition + Projectile.Size / 2f;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.ColorFunc = ColorFunction;
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 255);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw2(Projectile, ref lightColor, rotationOffset: Timer * 0.05f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<SpiritualBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
            for (float f = 0; f < 16; f++)
            {
                float p = f / 16f;
                Vector2 spawnPoint = Projectile.Center + VectorHelper.PointOnHeart(p * 8, 6);
                Vector2 vel = (spawnPoint - Projectile.Center).SafeNormalize(Vector2.Zero) * 5;
                Dust.NewDustPerfect(spawnPoint, ModContent.DustType<GlowHeartDust>(), vel, 0, Color.Pink, 2f).noGravity = true;
            }
            SoundStyle explosionSound = new SoundStyle($"Stellamod/Assets/Sounds/Briskfly");
            explosionSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(explosionSound, Projectile.position);
        }
    }
}
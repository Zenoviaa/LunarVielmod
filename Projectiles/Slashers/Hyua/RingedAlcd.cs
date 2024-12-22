using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Hyua
{
    public class RingedAlcd : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float HitTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Projectile.type] = 30;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 64;
            Projectile.penetrate = 15;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.LightPink, Main.rand.NextFloat(1f, 1.5f));
            }

            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.Center, 1024);
            if (nearest != null)
            {
                //Very weak homing
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 0.5f);
            }
            if (Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.1f;
            if (HitTimer > 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
            }
            DrawHelper.AnimateTopToBottom(Projectile, 2);
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightPink, Color.CadetBlue, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StarTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);*/
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = Vector2.One;
            Color glowColor = Color.White;
            glowColor = glowColor.MultiplyRGB(lightColor);
            glowColor.A = 0;
            float num = 8f;
            float buildProgress = MathHelper.Clamp(Timer / 30f, 0f, 1f);
            float speed = MathHelper.Lerp(0.001f, 0.04f, buildProgress);
            for (float f = 0; f < num; f++)
            {
                float progress = f / num;

                float swingRange = MathHelper.TwoPi;
                float swingXRadius = 64 * buildProgress;
                float swingYRadius = 8 * buildProgress;
                float swingProgress = Timer * speed + progress * MathHelper.TwoPi;

                float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
                float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
                Vector2 offset = new Vector2(xOffset, yOffset);
                Vector2 finalDrawPos = drawPos + offset;
                Vector2 finalDrawScale = drawScale;
                //     finalDrawScale.Y *= VectorHelper.Osc(0.2f, 0.75f, speed: 3f, offset: f);
                finalDrawScale.Y = 0.5f;
                spriteBatch.Draw(texture, finalDrawPos, frame, glowColor, drawRotation, drawOrigin, finalDrawScale, SpriteEffects.None, 0f);
            }


            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.LightPink,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: Main.rand.NextFloat(12, 25), baseSize: Main.rand.NextFloat(0.06f, 0.12f));
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Parendine2"), target.position);
            ShakeModSystem.Shake = 4;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.LightPink, 0.5f).noGravity = true;
            }

            for (float f = 0; f < 4; f++)
            {
                float progress = f / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(25f, 35f);
                velocity.Y *= 0.2f;
                var particle = FXUtil.GlowStretch(target.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightPink;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                particle.VectorScale *= 0.5f;

            }
            Projectile.velocity *= 0.15f;
            HitTimer = 15;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(0.2f, 0.8f), 0, Color.LightPink, 0.5f).noGravity = true;
            }
        }
    }
}
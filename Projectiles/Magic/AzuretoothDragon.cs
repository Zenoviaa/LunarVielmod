using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AzuretoothDragon : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 150;

            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            float maxDetectDistance = 1500;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if(closestNpc != null)
            {
                Vector2 targetVelocity = Projectile.Center.DirectionTo(closestNpc.Center) * 16;
                Vector2 velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.08f);
                Projectile.velocity = velocity;
                Projectile.alpha++;
                if (Projectile.alpha >= 255)
                    Projectile.alpha = 255;
            }

            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.5f);
            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.04f, Color.Transparent, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.alpha -= 50;
            Projectile.velocity *= 2;
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Frostburn2, 60);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            Vector2 frameSize = Projectile.Frame().Size();

            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            TrailDrawer.DrawPrims(Projectile.oldPos, frameSize * 0.5f - Main.screenPosition, 155);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.White, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }


        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public new Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;

        public override void PostDraw(Color lightColor)
        {
            float num108 = 4;
            float glowOsc = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;

            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color glowColor = Color.White * glowOsc * .8f;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(
                GlowTexture,
                Projectile.position - Main.screenPosition + drawOrigin,
                drawFrame,
                glowColor,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                spriteEffects,
                0
            );

            Color glowColorRot = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0)
                .MultiplyRGBA(Color.LightBlue);
            for (int num103 = 0; num103 < 1; num103++)
            {
                Color drawColor = glowColorRot;
                drawColor = Projectile.GetAlpha(drawColor);
                drawColor *= 1f - glowOsc;
                Vector2 drawPosition = Projectile.position + drawOrigin + (num103 / (float)num108 * 6.28318548f + Projectile.rotation + num106)
                    .ToRotationVector2() * (4f * glowOsc + 2f) - Main.screenPosition;
                Main.spriteBatch.Draw(GlowTexture, drawPosition, drawFrame, 
                    drawColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
        }

        private void Visuals()
        {
            int frameSpeed = 2;
            DrawHelper.AnimateTopToBottom(Projectile, frameSpeed);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightSkyBlue, 1f).noGravity = true;
        }
    }
}

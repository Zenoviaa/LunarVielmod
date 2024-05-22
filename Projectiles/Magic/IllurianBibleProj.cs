using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class IllurianBibleProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private Vector2 OriginalVelocity;
        private Vector2 OriginalPosition;

        private ref float Timer => ref Projectile.ai[0];
        private ref float ReturnTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 720;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (Projectile.velocity.X < targetVelocity.X)
            {
                Projectile.velocity.X++;
                if (Projectile.velocity.X >= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }
            else if (Projectile.velocity.X > targetVelocity.X)
            {
                Projectile.velocity.X--;
                if (Projectile.velocity.X <= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }

            if (Projectile.velocity.Y < targetVelocity.Y)
            {
                Projectile.velocity.Y++;
                if (Projectile.velocity.Y >= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
            else if (Projectile.velocity.Y > targetVelocity.Y)
            {
                Projectile.velocity.Y--;
                if (Projectile.velocity.Y <= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                Player owner = Main.player[Projectile.owner];
                Vector2 offset = -Vector2.UnitY;
                offset *= 128;
                Vector2 targetCenter = owner.Center + offset;
                OriginalVelocity = Projectile.velocity;
                OriginalPosition = targetCenter;
                SoundStyle soundStyle = SoundRegistry.Niivi_StarSummon;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if(Timer < 30 || ReturnTimer > 0)
            {
                ReturnTimer--;
                if(ReturnTimer == 0 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * OriginalVelocity.Length();
                    Projectile.netUpdate = true;
                }
   
                if(ReturnTimer > 0)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 30);
                }
                else
                {
                    AI_MoveToward(OriginalPosition, 8);

                }
            } 

            if(Timer > 30 && Timer < 45)
            {
                Projectile.velocity *= 0.99f;
            }

            if(Timer == 45 && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, 0, 0,
                     ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * OriginalVelocity.Length();
                Projectile.netUpdate = true;
            }

            if(Timer > 45 && ReturnTimer <= 0)
            {
                float maxDetectDistance = 256;
                NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
                if(closestNpc != null)
                {
                    AI_MoveToward(closestNpc.Center, 8);
                }
            }

            if (Timer % 7 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Color[] colors = new Color[] { Color.LightCyan, Color.Cyan, Color.Blue, Color.White };
                Color color = colors[Main.rand.Next(0, colors.Length)];
                float scale = Main.rand.NextFloat(0.5f, 0.8f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, color, scale);
            }

            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Main.DiscoColor * 0.3f, Color.Transparent, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            ReturnTimer = 60;
            Projectile.velocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver2);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Draw the trail
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);

            Vector2 frameSize = new Vector2(32, 32);
            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            TrailDrawer.DrawPrims(Projectile.oldPos, frameSize * 0.5f - Main.screenPosition, 155);

            float scale = 1.2f;
            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                float rotOffset = MathHelper.TwoPi * (i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = SoundRegistry.Niivi_StarringDeath;
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            for (int i = 0; i < 48; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4, 4);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Main.DiscoColor, scale);
            }
        }
    }
}

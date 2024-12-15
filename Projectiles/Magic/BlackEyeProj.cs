using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BlackEyeProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 17, Scale: 1f);
                    d.noGravity = true;

                    Vector2 speeda = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var da = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speeda * 11, Scale: 1f);
                    da.noGravity = false;

                    Vector2 speedab = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var dab = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speeda * 30, Scale: 1f);
                    dab.noGravity = false;
                }

                FXUtil.GlowCircleBoom(Projectile.Center,
                     innerColor: Color.White,
                     glowColor: Color.Yellow,
                     outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
                Projectile.velocity = Vector2.Zero;
                Projectile.velocity += -Vector2.UnitY * 4;
            }
            else
            {
                Projectile.velocity *= 0.8f;
            }

            if (Timer == 30)
            {
                float maxDetectDistance = 2400;
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
                if (npc != null)
                {
                    Vector2 velocity = Projectile.Center.DirectionTo(npc.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<BlackEyeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            if (Timer >= 120)
            {
                Projectile.Kill();
            }
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.5f);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 2; i++)
            {
                Color glowColor = Color.White.MultiplyRGB(lightColor);
                glowColor *= 0.8f;
                glowColor *= VectorHelper.Osc(0.5f, 1f);
                spriteBatch.Draw(texture, drawPos, frame, glowColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            //Draw the glow thing
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = 0.2f;

            //Colors
            Color startInner = Color.Goldenrod;
            Color startGlow = Color.Lerp(Color.Red, Color.DarkRed, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Orange, Color.DarkRed, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }
    }
}

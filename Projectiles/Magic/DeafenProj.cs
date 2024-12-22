using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class DeafenProj : ModProjectile
    {
        private Vector2[] _soundWavePos;
        private Vector2[] _soundWavePos2;
        private Vector2[] _soundWavePos3;

        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private float TrailAlpha;

        public bool Slowdown;
        public PrimDrawer Trail { get; set; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            _soundWavePos = new Vector2[32];
            _soundWavePos2 = new Vector2[32];
            _soundWavePos3 = new Vector2[32];
            TrailAlpha = 1f;
            Projectile.penetrate = 5;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
                target.AddBuff(BuffID.Confused, 180);
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(SoundID.Zombie83, Projectile.position);
                    SoundEngine.PlaySound(SoundID.Item104, Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Zombie82, Projectile.position);
                    SoundEngine.PlaySound(SoundID.Item103, Projectile.position);
                }
            }

            if (Timer % 4 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            }



            float swingXRadius = 32 * Easing.OutCirc(Timer / 60f);
            float swingYRadius = 64 * Easing.OutCirc(Timer / 60f);
            float swingRange = MathHelper.ToRadians(360);
            for (int i = 0; i < _soundWavePos.Length; i++)
            {
                float progress = (float)i / (float)_soundWavePos.Length;
                float xRadius = swingXRadius *
                    VectorHelper.Osc(0.85f, 1f, speed: 6);
                float yRadius = swingYRadius *
                    VectorHelper.Osc(0.85f, 1f, speed: 6);



                float xOffset = (xRadius)
                    * MathF.Sin(progress * swingRange + swingRange);
                float yOffset = (yRadius)
                    * MathF.Cos(progress * swingRange + swingRange);
                Vector2 offset = new Vector2(xOffset, yOffset).RotatedBy(Projectile.rotation);

                Vector2 pos = Projectile.Center + offset;
                _soundWavePos[i] = pos;

                offset = new Vector2(xOffset, yOffset).RotatedBy(Projectile.rotation);
                offset *= 0.66f;
                pos = Projectile.Center + offset;

                _soundWavePos2[i] = pos - (Projectile.velocity.SafeNormalize(Vector2.Zero) * VectorHelper.Osc(32, 44, speed: 6));

                offset = new Vector2(xOffset, yOffset).RotatedBy(Projectile.rotation);
                offset *= 0.33f;
                pos = Projectile.Center + offset;
                _soundWavePos3[i] = pos - (Projectile.velocity.SafeNormalize(Vector2.Zero) * VectorHelper.Osc(48, 64, speed: 6, offset: 6));
            }


            if (Slowdown && Timer > 30)
            {
                Projectile.tileCollide = false;
                Projectile.velocity *= 0.94f;
                TrailAlpha = MathHelper.Lerp(0f, 1f, Projectile.velocity.Length());
                if (Projectile.velocity.Length() <= 0.5f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 1.01f;
                Projectile.rotation = Projectile.velocity.ToRotation();

            }


            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
        }

        public float WidthFunction(float completionRatio)
        {
            float multiplier = MathF.Sin((Main.GlobalTimeWrappedHourly + completionRatio) * 24);
            return 186 * MathHelper.Lerp(1f, 0.5f, multiplier);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.MediumPurple * TrailAlpha;
        }

        protected virtual void DrawWindTrail(ref Color lightColor)
        {
            Main.spriteBatch.RestartDefaults();
            Trail ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            Trail.DrawPrims(_soundWavePos, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
            Trail.DrawPrims(_soundWavePos2, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
            Trail.DrawPrims(_soundWavePos3, -Main.screenPosition + Projectile.Size / 2, totalTrailPoints: 155);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawWindTrail(ref lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.DemonTorch, vel);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Slowdown)
            {
                Slowdown = true;
                Projectile.tileCollide = false;
                Projectile.velocity = oldVelocity;
            }

            return false;
        }
    }
}



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles
{
    public class BellSpike : ModProjectile
    {
        private float _pillarFlameScale;
        private float _flameTimer;
        private float _bloomLine;
        private Vector2 _scale;
        private Vector2[] _spikePos;
        private ref float Timer => ref Projectile.ai[0];
        private enum AIState
        {
            Telegraph,
            Stab
        }

        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (_spikePos == null)
                return false;
            return ProjectileHelper.OldPosColliding(_spikePos, projHitbox, targetHitbox, 6);
        }
        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target) && Timer >= 45 && Timer < 150;
        }
        public override void AI()
        {
            base.AI();

            float numPoints = 100;
            _spikePos ??= new Vector2[(int)numPoints];
            Vector2 start = Projectile.Center;

            float expandMult = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
            Vector2 end = start + Projectile.velocity * expandMult *
                ExtraMath.Osc(MathHelper.Lerp(0.9f, 1f, Timer / 100f), 1f, speed: 16, offset: Projectile.whoAmI) * _pillarFlameScale
                * MathHelper.Lerp(3f, 1f, EasingFunction.InOutSine(Timer / 60f));
            Vector2 end2 = start + Projectile.velocity * 0.3f;
            for (float f = 0; f < numPoints; f++)
            {
                float interpolant = f / numPoints;

                Vector2 e = Timer <= 5 ? end2 : end;
                Vector2 point = Vector2.Lerp(start, e, interpolant);
                point.X += MathF.Sin((-Timer * 0.1f) + interpolant * 18) * 3;
                _spikePos[(int)f] = point;
            }

            _flameTimer += MathHelper.Lerp(0.5f, 0.1f, EasingFunction.InOutSine(Timer / 30f));
            _pillarFlameScale = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 180f));
            Timer++;
            if (Timer == 1)
            {
                var p = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY, Color.Gray, Scale: 0.5f);

                for (float f = 0; f < 16; f++)
                {
                    Vector2 velocity = -Vector2.UnitY;
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    velocity *= Main.rand.NextFloat(1, 35);
                    if (Main.rand.NextBool(16))
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Gray, Scale: Main.rand.NextFloat(0.5f, 2));
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.Gray, Scale: Main.rand.NextFloat(0.5f, 2));
                    if (Main.rand.NextBool(8))
                    {
                        FXUtil.GlowStretch(Projectile.Center, velocity * 3);
                    }
                }

            }
            if (Timer % 15 == 0)
            {
                Vector2 velocity = -Vector2.UnitY;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(5, 15);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.GhostWhite, Scale: Main.rand.NextFloat(0.25f, 1));
            }
            _scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.OutExpo(Timer / 30f));
            _bloomLine = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / 30f));
        }
        private Color ColorFunction(float completionRatio)
        {
            if (Timer <= 5)
                return Color.White;
            Color fadeColor = Color.Yellow;
            fadeColor *= EasingFunction.InOutSine(Timer / 2f);

            Color flameColor = Color.Lerp(Color.Gray, Color.Lerp(Color.Blue, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 8, offset: Projectile.whoAmI)), completionRatio) * EasingFunction.QuadraticBump(completionRatio);
            Color finalColor = Color.Lerp(fadeColor, flameColor, Timer / 40f);
            finalColor *= _pillarFlameScale;
            finalColor *= EasingFunction.QuadraticBump(Timer / 180f);
            return finalColor * 3;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = MathHelper.SmoothStep(100, 0, completionRatio) * _scale.X;
            width *= MathHelper.Lerp(1f, 3f, EasingFunction.InExpo(Timer / 180f));
            return width;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (_spikePos == null)
                return false;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One;

            Texture2D voxTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 voxDrawOrigin = voxTexture.Size() / 2f;
            Color voxGlowColor = Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 4));
            voxGlowColor.A = 0;
            Vector2 voxDrawScale = new Vector2(5, 1) * 0.2f;
            spriteBatch.Draw(voxTexture, drawPosition + new Vector2(0, 21), null, voxGlowColor * EasingFunction.QuadraticBump(Timer / 60f), 0, voxDrawOrigin, voxDrawScale * EasingFunction.InOutSine(Timer / 30f), SpriteEffects.None, 0);

            Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 bloomLineOrigin = new Vector2(bloomLineTexture.Width / 2, bloomLineTexture.Height);
            Color glowDrawColor = Color.Lerp(Color.Gray, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 32));
            glowDrawColor *= _bloomLine;
            glowDrawColor.A = 0;
            spriteBatch.Draw(bloomLineTexture, drawPosition, null, glowDrawColor, 0, bloomLineOrigin, drawScale * EasingFunction.InOutSine(Timer / 30f), SpriteEffects.None, 0);



            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.WhispyTrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Lerp(Color.Black, Color.Gray, MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 170f)));
            shader.OuterColor = Color.Lerp(Color.Blue, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 4, offset: Projectile.whoAmI));
            shader.Distortion = MathHelper.Lerp(0.6f, 0.2f, EasingFunction.InOutSine(Timer / 30f)) * MathHelper.Lerp(1, 0, EasingFunction.InOutExpo(Timer / 90f));
            shader.Time = _flameTimer;
            TrailDrawer.Draw(spriteBatch, _spikePos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}

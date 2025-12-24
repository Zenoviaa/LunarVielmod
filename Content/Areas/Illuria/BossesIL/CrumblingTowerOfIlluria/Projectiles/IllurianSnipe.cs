using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    /*
     * Crumbling Tower of Illuria - Father of the flame and pandoras box
        Static boss, stays in the middle until his lantern head falls off and turns into a rolling ball that hits the walls
        During first phase it snipes laser bolts around as there are wisps that summon around, that you have to attack to damage the boss
        During this phase it has a chance to have little homing white moths fly at you. 
        After first phase, the head drops down and moves from side to side aggressively hitting the wall and creating a shockwave against it that you have to dodge 
        It can also jump in the second phase and shoot out a bunch of white whips during this too.
        Disco head, where it shines white and blue lights everywhere for no reason

    */


    public class IllurianSnipe : ScarletProjectile
    {
        private float _telegraphLineAlpha;
        private float _telegraphLineRot;
        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[2]];
        }
        private enum AIState
        {
            Charge = 0,
            Fire = 1
        }

        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 24;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }


        public override bool ShouldUpdatePosition()
        {
            return State == AIState.Fire;
        }

        public override void AI()
        {
            base.AI();

            switch (State)
            {
                case AIState.Charge:
                    AI_Charge();
                    break;
                case AIState.Fire:
                    AI_Fire();
                    break;
            }

        }

        private void SwitchState(AIState state)
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

        private void AI_Charge()
        {
            Timer++;
            float chargeTime = 250;
            float completionRatio = Timer / chargeTime;
            Projectile.Center = Parent.Center;
            Projectile.scale = MathHelper.Lerp(1.5f, 1f, EasingFunction.Anticipation2(completionRatio));
            if(Timer % 20 == 0)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200);
                Vector2 spawnVelocity = (Projectile.Center - spawnPos).SafeNormalize(Vector2.Zero);
                spawnVelocity *= 24;
                var stretch = FXUtil.GlowStretch(spawnPos, spawnVelocity);
                stretch.Scale *= Main.rand.NextFloat(0.5f, 1f);
                stretch.VectorScale.X *= Main.rand.NextFloat(0.5f, 1f);
            }
            if(Timer >= chargeTime)
            {
                SwitchState(AIState.Fire);
            }

            Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
         
            if(closest != null)
            {
                Vector2 fireVelocity = (closest.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                _telegraphLineRot = fireVelocity.ToRotation() - MathHelper.PiOver2;
                _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(completionRatio));
            }
        }

        private void AI_Fire()
        {
            _telegraphLineAlpha *= 0.9f;
            Timer++;
            if(Timer == 1)
            {
                Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
                Vector2 fireVelocity = (closest.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                fireVelocity *= Projectile.velocity.Length();
                Projectile.velocity = fireVelocity;
                Projectile.netUpdate = true;

                float numDust = 6;
                for(float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = Projectile.velocity;
                    dustVelocity = dustVelocity.RotatedByRandom(0.25f);
                    dustVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.White, Scale: Main.rand.NextFloat(0.3f, 0.8f));
                }

                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, newColor: Color.Cyan);
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.AutomationCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }
            Projectile.velocity *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 3;
            for(float n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                velocity += -Projectile.oldVelocity * Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Cyan, Scale: Main.rand.NextFloat(0.4f, 0.75f));
            }
            var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.Blue);
            part.Scale *= 0.66f;

            SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Illuria.IceImpact1 : AssetRegistry.Sounds.Illuria.IceImpact2;
            hitSound.PitchVariance = 0.3f;
            hitSound.Volume = 0.5f;
            SoundEngine.PlaySound(hitSound, Projectile.position);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(69, 196, 182), Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(8, 0, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            Texture2D bloomlineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(bloomlineTexture.Width / 2f, 0f);
            Color drawColor = Color.White;
            drawColor.A = 0;
            drawColor *= _telegraphLineAlpha;
            drawColor *= 0.5f;
            Vector2 scale = Vector2.One;
            scale.Y *= 2;
            scale.X *= 0.15f;
            spriteBatch.Draw(bloomlineTexture, drawCenter, null, drawColor, _telegraphLineRot, drawOrigin, scale, SpriteEffects.None, 0);

            if(State == AIState.Charge)
            {
                Texture2D glowballTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
                scale = Vector2.One * _telegraphLineAlpha;
                drawOrigin = glowballTexture.Size() / 2f;
                for(float r = 0; r < 4; r++)
                    spriteBatch.Draw(glowballTexture, drawCenter, null, drawColor, 0, drawOrigin, scale, SpriteEffects.None, 0);

            }

    
            if(State == AIState.Fire)
            {
                var shader = MagicNormalShader.Instance;
                shader.PrimaryTexture = TrailRegistry.GlowTrail;
                shader.NoiseTexture = TrailRegistry.SpikyTrail1;
                shader.BlendState = BlendState.Additive;
                shader.SamplerState = SamplerState.PointWrap;
                shader.Speed = 0.5f;
                shader.Repeats = 1f;
                //This just applis the shader changes
                TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
            }

        }
    }
}

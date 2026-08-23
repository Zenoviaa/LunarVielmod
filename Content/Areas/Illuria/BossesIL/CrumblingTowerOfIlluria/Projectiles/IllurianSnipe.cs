using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.Materials;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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


    public class IllurianSnipe : ModProjectile,
        IDrawToRenderTarget
    {
        private float _size;
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
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
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
                    SwitchState(AIState.Fire);
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
                _size = Main.rand.NextFloat(0.50f, 1.00f);
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

       
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.AutomationCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }

            if(Timer % 5 == 0 && Timer < 19)
            {
                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, newColor: Color.Cyan);
            }
            if(Timer % 4 == 0)
            {
                var isp = IllurianSnowflakeParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero));
                isp.Scale *= Main.rand.NextFloat(0.4f, 0.6f);
                isp.color *= 0.85f;
            }

            if (Main.rand.NextBool(3))
            {
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * 45);
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.2f;
                dp.Scale *= 0.7f;
                dp.outerColor = Color.Blue;
                dp.superFast = true;
            }

            if (Main.rand.NextBool(3))
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
           
                dp.gravity = 0f;
                dp.dampening = 0.05f;
               
                // dp.fast = true;
                dp.Scale *= 0.8f;
            }
            if (Main.rand.NextBool(3))
            {
                FaintSmokeParticle dp = Particle<FaintSmokeParticle>.SpawnInAlphaLayer(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.behindLayer = true;
                dp.fadeToColor = Color.White * 0f;
                dp.color *= 0.2f;
                // dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.5f;
            }
            if (Main.rand.NextBool(3))
            {
                FaintSmokeParticle dp = Particle<FaintSmokeParticle>.SpawnInAlphaLayer(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: 1f);
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
       
                dp.behindLayer = true;

                dp.fadeToColor = Color.White * 0f;
                dp.color = Color.DarkGray * 0.4f;
                // dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.45f;
            }



            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.25f;
                        break;
                    case 1:
                        FlakeParticle sp2 = Particle<FlakeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        //sp2.fast = true;
                        sp2.dampening = 0.1f;

                        break;
                }
            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= 0.25f;
            }


            Projectile.extraUpdates = 0;
            if(Projectile.velocity.Length() < 27)
                Projectile.velocity *= MathHelper.Lerp(1f, 1.2f, EasingFunction.InExpo(Timer / 60f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.LightSkyBlue;
                spawnParams.outerColor = Color.DarkBlue;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 30, baseSize: 0.2f);
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.oldVelocity,
                    ModContent.ProjectileType<IceCometBoom>(),0, Projectile.knockBack, Projectile.owner);
            }
            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;

            var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Cyan, Color.Blue);
            part.Scale *= 0.66f;

            SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Illuria.IceImpact1 : AssetRegistry.Sounds.Illuria.IceImpact2;
            hitSound.PitchVariance = 0.3f;
            hitSound.Volume = 0.5f;
            SoundEngine.PlaySound(hitSound, Projectile.position);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        private void DrawHead(SpriteBatch spriteBatch, Vector2 sp)
        {
            SpritebatchDrawer haedDrawer = SpritebatchDrawer.FromTextureAsset(
                AssetManager.GlowMask.StarFlare2, Projectile.Center);
            haedDrawer.color = Color.White * ExtraMath.Osc(0.65f, 0.9f, speed: 12, offset: Projectile.whoAmI);
            haedDrawer.color.A = 0;
            haedDrawer.scale *= 0.3f * _size;
            spriteBatch.Draw(haedDrawer);


            haedDrawer = SpritebatchDrawer.FromTextureAsset(
    AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            haedDrawer.color = Color.Blue * ExtraMath.Osc(0.65f, 0.9f, speed: 12, offset: Projectile.whoAmI) * 0.7f;
            haedDrawer.color.A = 0;
            haedDrawer.scale *= 0.5f * _size;
            spriteBatch.Draw(haedDrawer);
        }

        public void DrawToRenderTargets()
        {
            if (State != AIState.Fire)
                return;

            Color GetTrailColor(float ratio)
            {
                Color t = DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.LightSkyBlue, Color.Blue, Color.Purple);//.Lerp(Color.LightSkyBlue, Color.Purple, ratio);
                t.A = 0;
                return t;
            }

            float GetTrailWidth(float ratio)
            {
                float lerp1 = MathHelper.Lerp(36, 0, EasingFunction.InOutSine(ratio));
                float lerp2 = MathHelper.Lerp(0f, 1.4f, EasingFunction.OutExpo(ratio));
                return lerp1 * lerp2 * _size;
            }

            BlizzardTrailMaterial.PrepareRender(TrailDrawer.PrepareVertices(Projectile.oldPos,
                GetTrailColor, GetTrailWidth, useSmoothing: false, offset: Projectile.Size * 0.5f));
            PixelationManager.QueueSpritebatchDrawAction(DrawHead, DrawLayer.OverPlayers);
        }
    }
}

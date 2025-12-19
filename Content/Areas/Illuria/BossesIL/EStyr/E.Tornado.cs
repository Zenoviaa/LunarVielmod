using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackTornadoWind
    {
        private readonly TexturedQuad _texturedQuad;
        public BlackTornadoWind()
        {
            _texturedQuad = new TexturedQuad();
        }

        public float alpha;
        public void Draw(Vector2 drawCenter, float length, float width)
        {
            if (alpha <= 0)
                return;

            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.Black;
            flamingTrailShader.InnerColor = Color.White * 0.1f;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = new Vector2(1, 3);
            flamingTrailShader.BlendState = BlendState.AlphaBlend;
            flamingTrailShader.Time = Main.GlobalTimeWrappedHourly * 64;


            _texturedQuad.CalculateCenterVertices(drawCenter,
                length, width);
            _texturedQuad.SetColor(Color.White * alpha);
            _texturedQuad.DrawWithShader(flamingTrailShader);
        }
    }

    public class BlackTornadoDebris : ModProjectile,
        IDrawBlackStar,
        IDrawOutlines
    {
        private float _telegraphLineRot;
        private float _telegraphLineAlpha;
        private ref float Timer => ref Projectile.ai[0];
        private enum AIState
        {
            Fly,
            ShootDown
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float ShouldFall => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 800;
            Projectile.hostile = true;
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Fly:
                    AI_Fly();
                    break;
                case AIState.ShootDown:
                    AI_ShootDown();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void PlayPingSound()
        {
            //eh


        }

        private void AI_ShootDown()
        {
            Timer++;
            if (Timer == 1)
            {
                Particle.NewParticle<StarParticle>(Projectile.Center, Vector2.Zero, Color.White);
                PlayPingSound();
            }

            if (Timer < 15)
            {
                Projectile.velocity *= 0.9f;
            }

            float prepTime = 60f;
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / prepTime));
            _telegraphLineRot = Vector2.UnitY.ToRotation();
            if (Timer == 60)
            {
                var donut = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Vector2.UnitY, Color.White);
                donut.Scale *= 0.3f;
            }

            if (Timer >= 60)
            {
                float speed = 35;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.UnitY * speed, 0.1f);
            }
        }

        private void AI_Fly()
        {
            Timer++;
            if (Timer == 1 && this.OwnedByLocalClient())
            {
                /*
                SoundStyle flashSound = AssetRegistry.Sounds.Bishinine.FallingBell;
                flashSound.Pitch = 0.66f;
                flashSound.Volume = 0.25f;
                flashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flashSound, Projectile.position);*/
                ShouldFall = Main.rand.NextBool(2) ? 1 : 0;
            }



            if (Timer % 10 == 0)
            {
                var p = Particle.NewParticle<StarParticle>(Projectile.Center, Vector2.Zero, Color.White, Scale: 0.4f);
                p.fast = true;
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), DustID.Dirt, Vector2.Zero,
                    newColor: Color.White,
                    Scale: 1);
                d.noGravity = true;
            }

            float outScale = (float)Projectile.timeLeft / 15f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            Projectile.rotation += 0.01f * MathF.Sign(Projectile.velocity.X);
            Projectile.rotation += Projectile.velocity.Length() * 0.025f;

            if (ShouldFall == 1)
            {
                Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
                if (closestPlayer != null)
                {
                    //Once above the player, we're going to shoot down really fast and explode lol.
                    Vector2 directionToPlayer = (closestPlayer.Center - Projectile.Center);
                    directionToPlayer = directionToPlayer.SafeNormalize(Vector2.Zero);
                    Vector2 normalVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    float dp = Vector2.Dot(directionToPlayer, Vector2.UnitY);
                    if (dp > 0.75f)
                    {
                        SwitchState(AIState.ShootDown);
                    }
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawAfterImages(Main.spriteBatch);
            DrawHelper.DrawBloomLine(Main.spriteBatch, Projectile.Center, Color.White, _telegraphLineRot, _telegraphLineAlpha * 0.2f);
            return false;
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = (float)i / (float)Projectile.oldPos.Length;

                Vector2 drawCenter = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float rotation = Projectile.oldRot[i];
                float scale = Projectile.scale;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.15f;
                Vector2 drawScale = Vector2.One;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, Projectile.Center + v - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center - v - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center + h - screenPos, Color.Red);
            DrawSprite(spriteBatch, Projectile.Center - h - screenPos, Color.Red);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawSprite(spriteBatch, Projectile.Center - Main.screenPosition, Color.White);
        }
    }


    public class BlackTornado : ModProjectile
    {
        private LittleStarParticleManager _tornadoStreakParticlesBackingField;
        private LittleStarParticleManager TornadoStreakParticles
        {
            get
            {
                _tornadoStreakParticlesBackingField ??= new LittleStarParticleManager(50, 16, GetTrailWidth);
                return _tornadoStreakParticlesBackingField;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];

        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            //For this projectile what we're going to need to do is create a tornado visual with projectiles coming outward and then coming in
            //Gustbeak has a tornado but it's meh
            //Gintzia's winds look a bit better and should look fine when combined with swirling particles
            //So make a new particle manager for this
            Timer++;
            if(Timer % 12 == 0)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
                jiitasSit.PitchVariance = 0.2f;
                jiitasSit.Pitch = 0.6f;
                SoundEngine.PlaySound(jiitasSit, Projectile.position);
            }
            if (Timer % 25 == 0)
            {
          
                if (this.OwnedByLocalClient())
                {
                    float direction = Main.rand.NextBool(2) ? -1 : 1;
                    float xOffset = direction * 2000;
                    Vector2 spawnOffset = new Vector2(xOffset, Main.rand.NextFloat(-450f, -159f));
                    Vector2 spawnPos = Projectile.Center + spawnOffset;
                    Vector2 velocity = Vector2.UnitX * -direction * Main.rand.NextFloat(15f, 25);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, velocity,
                        ModContent.ProjectileType<BlackTornadoDebris>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            float inTornado = Timer / 30f;
            float outTornado = (float)Projectile.timeLeft / 30f;

            inTornado = EasingFunction.InOutSine(inTornado);
            outTornado = EasingFunction.InOutSine(outTornado);
            float alpha = inTornado * outTornado;
            TornadoStreakParticles.xOvalRadius = 5;
            TornadoStreakParticles.yOvalRadius = 350;
            TornadoStreakParticles.minX = ExtraMath.Osc(200f, 300f, speed: 3);
            TornadoStreakParticles.spinTime = 50;
            TornadoStreakParticles.rotationAxis = new Vector3(0, 1, 0.2f);
            TornadoStreakParticles.alpha = 0.45f * alpha;
            TornadoStreakParticles.Update(Projectile.Center);
           
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0.2f, 4, EasingFunction.QuadraticBump(completionRatio));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            TornadoStreakParticles.Draw();
            return false;
        }
    }
    public class TornadoSuckPlayer : ModPlayer
    {
        public Vector2? TornadoCenter;
        public float TornadoPullStrength;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (TornadoCenter.HasValue)
            {

                Vector2 tornadoCenter = TornadoCenter.Value;
                Vector2 tornadoPullDirection = tornadoCenter - Player.Center;
                tornadoPullDirection = tornadoPullDirection.SafeNormalize(Vector2.Zero);

                Player.velocity += tornadoPullDirection * TornadoPullStrength;
                TornadoCenter = null;
            }
        }
    }
    public partial class E
    {
        private int TornadoDamage => 45;

        /// <summary>
        /// Sucks in all players to him
        /// </summary>
        /// <param name="strength"></param>
        private void SuckAllPlayers(float strength)
        {
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(NPC.Center, player.Center);
                TornadoSuckPlayer tornadoSuckPlayer = player.GetModPlayer<TornadoSuckPlayer>();
                tornadoSuckPlayer.TornadoCenter = NPC.Center;
                tornadoSuckPlayer.TornadoPullStrength = distance / 2560f * strength;
            }
        }

        private void AI_TornadoStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            float startupTime = 60;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 32);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            if (Timer >= startupTime)
            {
                SwitchState(AIState.Tornado_PreSpin);
            }
        }

        private void AI_TornadoPreSpin()
        {
            //In this state, he'll slowly start speeding up and then creating the tornado, 
            //The earlier startup state is just to get him into the position
            //This is mostly done with a sound and animation, so not much happens here
            Timer++;
            if(Timer == 1)
            {
                TargetVector = NPC.Center;
            }


            float prespinTime = 60f;
            float completionRatio = Timer / prespinTime;
            float ease = EasingFunction.InOutSine(completionRatio);



            //Speed up
            float xOffset = MathF.Sin(Timer * -0.05f) * 64;
            float yOffset = MathF.Cos(Timer * 0.05f) * 32f;
            Vector2 targetOffset = new Vector2(xOffset, yOffset);
            Vector2 positionToMoveTo = TargetVector + targetOffset;
            Vector2 tornadoVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = tornadoVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.5f, ease);
            ShakeModSystem.Shake = MathHelper.Lerp(0f, 2f, ease);
            Wind.alpha = MathHelper.Lerp(0f, 1f, ease);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= prespinTime)
            {
                SwitchState(AIState.Tornado_Spin);
            }
        }

        private void AI_TornadoSpin()
        {
            //Here the tornado projectile will actually spawn and we'll begin sucking in all of the players
            //At the same time we'll slowly move towards our target
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, TargetVector, Vector2.Zero,
                        ModContent.ProjectileType<BlackTornado>(), TornadoDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                }
            }

            float tornadoTime = 600;
            _extraAfterImageAlpha = 0.5f;
            float xOffset = MathF.Sin(Timer * -0.5f) * 164;
            float yOffset = MathF.Cos(Timer * 0.5f) * 32f;
            Vector2 targetOffset = new Vector2(xOffset, yOffset);
            Vector2 positionToMoveTo = TargetVector + targetOffset;
            Vector2 tornadoVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = tornadoVelocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;


            ShakeModSystem.Shake = 4;
            Wind.alpha = 1f;
            //Unsure how strong this should actually be so make sure to balance this number properly
            float tornadoStrength = 1f;
            SuckAllPlayers(tornadoStrength);
            TargetOutlineColor = Color.Red;
            if (Timer >= tornadoTime)
            {
                SwitchState(AIState.Tornado_End);
            }
        }

        private void AI_TornadoEnd()
        {
            Timer++;
            float endTime = 15f;
            float completionRatio = Timer / endTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Wind.alpha = MathHelper.Lerp(1f, 0f, ease);
            NPC.velocity *= 0.9f;
            _extraAfterImageAlpha = MathHelper.Lerp(0.5f, 0f, ease);
            if (Timer >= endTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}

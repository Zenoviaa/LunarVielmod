using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{

    public class SingularBall : ScarletProjectile,
        IDrawOutlines,
        IDrawBlackStar
    {
        private float _vortexFrame;
        private ref float Timer => ref Projectile.ai[0];
        public enum AIState
        {
            Grow,
            Bounce,
            Explode
        }
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private ref float Charge => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            DrawHelper.UpdateFrame(ref _vortexFrame, 0.8f, 1, 90);
            switch (State)
            {
                case AIState.Grow:
                    AI_Grow();
                    break;
                case AIState.Bounce:
                    AI_Bounce();
                    break;
                case AIState.Explode:
                    AI_Explode();
                    break;
            }
        }

        private void AI_Explode()
        {
            Timer++;
            if(Timer == 1 && this.OwnedByLocalClient())
            {
                float numProjectiles = 8;
                for(int n = 0; n < numProjectiles; n++)
                {
                    float ratio = (float)n / numProjectiles;
                    Vector2 velocity = Vector2.UnitY.RotatedBy(ratio * MathHelper.TwoPi);
                    velocity *= 15;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                        ModContent.ProjectileType<DarkStarMini>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                ShakeModSystem.Shake = 4;
                var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Black);
                boom.Scale *= 2f;

                float numDust = 16;
                for(float n = 0; n < numDust; n++)
                {
                    float ratio = n / numDust;
                    Vector2 velocity = Vector2.UnitY.RotatedBy(ratio * MathHelper.TwoPi);
                    velocity *= 15;
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), velocity, newColor: Color.White, Scale: 2);
                }
            }
            Projectile.scale *= 0.9f;
            if (Projectile.scale <= 0.03f)
                Projectile.Kill();
        }

        private void AI_Grow()
        {
            Timer++;
            if(Timer == 1)
            {
                Projectile.scale = 0.001f;
            }

            float growTime = 60;
            float completionRatio = Timer / growTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, ease);
        }

        private void AI_Bounce()
        {
            Timer++;
            if(Timer == 1)
            {
                Charge++;
                ShakeModSystem.Shake = 8;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);




                Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
                if(closest != null)
                {
                    Vector2 velocity = (closest.Center - Projectile.Center);
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= 20;
                    Projectile.velocity = velocity;
                }
                Vector2 rotatedVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 startPosition = Projectile.Center - rotatedVelocity * 1200;
                ScreenSmearEffectManager.NewParticle(startPosition, rotatedVelocity, 2400, 45);

                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity * MathHelper.Lerp(15, 1f, i / 3f));
                    donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

                }
                var strike = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;

                var strike2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity);
                strike2.xMult = 32;
                strike2.rotOffset += MathHelper.PiOver2;

                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, Projectile.position);
            }
            if(Timer >= 45)
            {
                Projectile.velocity *= 0.9f;
            }
        }

        public void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            float rotation = Projectile.rotation;

            Vector2 drawScale = Vector2.One * Projectile.scale;
            float scaleMultiplier = MathHelper.Lerp(1f, 2f, Charge / 16f);
            drawScale *= scaleMultiplier;
            spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private void DrawAfterImages(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;

                Vector2 drawCenter = OldCenterPos[i] - Main.screenPosition;
                float rotation = OldCenterRot[i];

                Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                drawColor *= 0.3f;
                Vector2 drawScale = Vector2.One * Projectile.scale;
                spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawSprite(Main.spriteBatch, Main.screenPosition, Color.Black);
            DrawSingularity(Main.spriteBatch, Main.screenPosition);
            return false;
        }
        private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_vortexFrame, columns: 5, frameWidth: 50, frameHeight: 50);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Vortex").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
     //       incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = Projectile.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float scaleMultiplier = MathHelper.Lerp(1f, 2f, Charge / 16f);
            float drawScale = 1.8f * Projectile.scale * scaleMultiplier;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);
            DrawSingularity(spriteBatch, Main.screenPosition);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, Main.screenPosition + v, Color.White);
            DrawSprite(spriteBatch, Main.screenPosition - v, Color.White);
            DrawSprite(spriteBatch, Main.screenPosition + h, Color.White);
            DrawSprite(spriteBatch, Main.screenPosition - h, Color.White);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            BlackStars.AddBuff(target, 50);
        }
    }

    public partial class E
    {
        /*
         * Holds out his hand charging a small singularity ball, 
         * then he hits hit around like bishinine mixed with Gerson, ricocheting off himself to bounce it at you, 
         * and every time he hits it, it gets bigger until it explodes into a bunch of black stars
         */
        private SingularBall _singularBall;
        private int SingularBallDamage => 30;
        private void AI_SingularBaseballStart()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
                NPC.direction = TargetDirection;
            }

            float startupTime = 60f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 192);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 easeVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = easeVelocity;
            Animator.PlayAnimation(Anim_BattleIdle);
            if(Timer >= startupTime)
            {
                SwitchState(AIState.SingularBaseball_SummonBall);
            }
        }

        private void AI_SinuglarBaseballSummonBall()
        {
            Timer++;
            if(Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 startPosition = NPC.Center + Vector2.UnitX * NPC.direction * 64;
                    _singularBall = Projectile.NewProjectileDirect(SourceFromThis, startPosition, Vector2.Zero,
                        ModContent.ProjectileType<SingularBall>(), SingularBallDamage, 1, Main.myPlayer).ModProjectile as SingularBall;
                    _singularBall.SwitchState(SingularBall.AIState.Grow);
                }
            }

            float summonTime = 120f;
            Animator.PlayAnimation(Anim_Holding);
            float completionRatio = Timer / summonTime;
            NPC.velocity *= 0.9f;
            if(Timer >= summonTime)
            {
                SwitchState(AIState.SingularBaseball_HitBall);
            }
        }

        private void AI_SingularBaseballHitBall()
        {
            Timer++;
            if(Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    TargetVector = NPC.Center;
                    _singularBall.SwitchState(SingularBall.AIState.Bounce);
                    NPC.netUpdate = true;
                }
            }
            float hitTime = 25f;
            float completionRatio = Timer / hitTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 start = TargetVector;
            Vector2 end = start + Vector2.UnitX * -NPC.direction * 128;
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = targetVelocity;
            if(_attackNumber % 2 == 0)
            {
                Animator.PlayAnimation(Anim_ForwardSlash);
            }
            else
            {
                Animator.PlayAnimation(Anim_BackSlash);
            }
           // _extraAfterImageAlpha = MathHelper.Lerp(0);
            if (Timer >= hitTime)
            {
                SwitchState(AIState.SingularBaseball_FindBall);
            }
        }

        private void AI_SingularBaseballFindBall()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    _forwardVector = NPC.velocity;
                    TargetVector = _singularBall.Projectile.Center;
                    NPC.netUpdate = true;
                }
            }

            float findTime = 15f;
            float completionRatio = Timer / findTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 targetVelocity = (TargetVector - NPC.Center);
            Vector2 easeVelocity = Vector2.Lerp(_forwardVector, targetVelocity, ease);
            NPC.velocity = easeVelocity;
            NPC.direction = TargetDirection;
            _extraAfterImageAlpha = 0.7f;
          
            if(Timer >= findTime)
            {
                _attackNumber++;
                if(_attackNumber >= 12)
                {
                    SwitchState(AIState.SingularBaseball_End);
                }
                else
                {
                    SwitchState(AIState.SingularBaseball_HitBall);
                }
            }
        }

        private void AI_SingularBaseballEnd()
        {
            Timer++;
            if(Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _singularBall.SwitchState(SingularBall.AIState.Explode);    
                }
            }
            NPC.velocity *= 0.9f;
            if(Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class StarRift : ModProjectile,
         IDrawBlackStar
    {
        private float _easeInTimer;
        private float _inFlash;
        private float _outScale = 1f;
        private enum AIState
        {
            Spawn,
            Gravity,
            LaserBlast
        }
        private Vector2[] Points = new Vector2[64];
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            BlackStars.AddBuff(target, 65);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void AI_Spawn()
        {
            Timer++;

            Vector2 rotatedVelocity = Projectile.velocity;
            rotatedVelocity = rotatedVelocity.RotatedBy(MathHelper.PiOver2);

            if (Timer == 1)
            {
                ShakeModSystem.Shake = 32;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);


      
                Vector2 startPosition = Projectile.Center - rotatedVelocity * 1200;
                ScreenSmearEffectManager.NewParticle(startPosition, rotatedVelocity, 2400, 45);

                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity * MathHelper.Lerp(15, 1f, i / 3f));
                    donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

                }
                var strike = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;

                var strike2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, rotatedVelocity);
                strike2.xMult = 32;
                strike2.rotOffset += MathHelper.PiOver2;
                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, Projectile.position);
            }
            if (Points == null)
                return;



            float length = 1500;
            Vector2 start = Projectile.Center - rotatedVelocity * length;
            Vector2 end = Projectile.Center + rotatedVelocity * length;
            for (int i = 0; i < Points.Length; i++)
            {
                float completionRatio = (float)i / (float)Points.Length;
                float ease = EasingFunction.InOutSine(completionRatio);

                Vector2 interpolatedPoint = Vector2.Lerp(start, end, ease);
                Points[i] = interpolatedPoint;
            }

            if(Timer % 5 == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    float rand = Main.rand.NextFloat(0.00f, 1.00f);
                    Vector2 point = Vector2.Lerp(start, end, rand);
                    Vector2 velocity = Vector2.UnitY * 8;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, velocity, 
                        ModContent.ProjectileType<DarkStarMini>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, velocity,
                        ModContent.ProjectileType<BlackSplash>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 0.15f);
                }
            }
            if(Projectile.timeLeft < 30)
            {
                _outScale *= 0.9f;
            }
        }

        public override void AI()
        {
            base.AI();
            _easeInTimer++;
            AI_Spawn();
        }

        private float GetTrailWidth(float completionRatio)
        {
            float w = MathHelper.Lerp(0f, 16, EasingFunction.QuadraticBump(completionRatio));
            w *= MathHelper.Lerp(2f, 1f, _inFlash);

            float inScale = EasingFunction.InOutSine(_easeInTimer / 30f);
            w *= inScale;
            w *= _outScale;
            return w;
        }

        private Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, _inFlash);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            if (Points == null)
                return;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BeamTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 2;
            shader.Repeats = 1f;

            _inFlash = 0f;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);

            _inFlash = 1f;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, Points, GetTrailColor, GetTrailWidth, shader);
        }
    }
    public class DarkStarMini : ScarletProjectile,
        IDrawBlackStar,
        IDrawOutlines
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool IsSlow => Projectile.ai[1] == 1;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 9;
            Projectile.height = 9;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle starSound = new SoundStyle("Stellamod/Assets/Sounds/Starrer");
                starSound.PitchVariance = 0.3f;
                starSound.Volume = 0.5f;
                SoundEngine.PlaySound(starSound, Projectile.position);
            }
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            if (Timer >= 30)
            {
                if (Projectile.velocity.Length() < 8)
                {
                    if (IsSlow)
                    {
                        Projectile.velocity *= 1.01375f;
                    }
                    else
                    {
                        Projectile.velocity *= 1.065f;
                    }
                    
                }
            }
            else
            {
                if (Projectile.velocity.Length() > 1)
                {
                    Projectile.velocity *= 0.9f;
                }
            }

                Projectile.rotation += 0.005f;
            Projectile.rotation += Projectile.velocity.Length() * 0.005f;

            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
            if (player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, degreesToRotate: 0.15f);
            }

            if (Timer % 32 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Scale: 0.4f);
            }
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            float rotation = Projectile.rotation;

            Vector2 drawScale = Vector2.One * Projectile.scale;
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
            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);

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
            BlackStars.AddBuff(target, 25);
        }
    }
    public partial class E
    {
        /*
         * 
         * Jumps over top of you and charges his sword, 
         * slashing downwards at you with a big slash until it meets half way and a bunch of stars explode
         */
        private int StarRiftDamage => 30;
        private float GetSlashOffset()
        {
            return -300;
        }
        private void AI_SwordStarPlosionStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.Center;
            }

            float startupTime = 60f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 start = TargetVector;
            Vector2 end = MyTarget.Center + new Vector2(0, GetSlashOffset());
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = targetVelocity;
            Animator.PlayAnimation(Anim_BattleIdle);
            if (Timer >= startupTime)
            {
                SwitchState(AIState.SwordStarPlosion_Charge);
            }
        }

        private void AI_SwordStarPlosionCharge()
        {
            Timer++;

            //Move to the position that we're holding the sword, getting ready to slash
            Vector2 positionToMoveTo = MyTarget.Center + new Vector2(0, GetSlashOffset());
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = targetVelocity;

            //Get extra after image
            float chargeTime = 120;
            float completionRatio = Timer / chargeTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.7f, ease);

            Animator.PlayAnimation(Anim_Holding);

            if (Timer >= chargeTime)
            {
                SwitchState(AIState.SwordStarPlosion_Swing);
            }
        }

        private void AI_SwordStarPlosionSwing()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.UnitY,
                        ModContent.ProjectileType<StarRift>(), StarRiftDamage, 1, Main.myPlayer);
                }
            }
            Animator.PlayAnimation(Anim_BigSlash);
            float swingTime = 30f;
            float completionRatio = Timer / swingTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _extraAfterImageAlpha = MathHelper.Lerp(0.7f, 0f, ease);

            if(Timer == 1)
            {
                Vector2 slideVelocity = Vector2.UnitX * NPC.direction;
                slideVelocity *= -1;
                float initialSpeed = 52;
                slideVelocity *= initialSpeed;
                NPC.velocity = slideVelocity;
            }

            NPC.velocity *= 0.92f;
            if (Timer >= swingTime)
            {
                SwitchState(AIState.SwordStarPlosion_End);
            }
        }

        private void AI_SwordStarPlosion_End()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class DarkStar : ScarletProjectile,
        IDrawOutlines,
        IDrawBlackStar
    {

        private ref float Timer => ref Projectile.ai[0];
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
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            Projectile.scale = 1f * outScale;
            if (Projectile.velocity.Length() < 8)
            {
                Projectile.velocity *= 1.065f;
            }
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
            if(player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, degreesToRotate: 0.15f);
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

            return false;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {
            DrawAfterImages(spriteBatch);
            DrawSprite(spriteBatch, Main.screenPosition, Color.White);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            DrawSprite(spriteBatch, Main.screenPosition + v, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition - v, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition + h, Color.Red);
            DrawSprite(spriteBatch, Main.screenPosition - h, Color.Red);
        }
    }

    public partial class E
    {
        //For this attack, I'm not exactly sure what zemmie was saying we're supposed to do
        //But based on the description we need 5 states
        private int DarkStarDamage => 20;
        private void AI_ScreenSlashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            //He should come next to you and slow down
            //Very simple windup
            float startupTime = 90f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 positionToMoveTo = MyTarget.Center - new Vector2(0, 200);
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            Vector2 interpolatedVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = interpolatedVelocity;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startupTime)
            {
                SwitchState(AIState.ScreenSlash_PreSlash);
            }
        }

        private void AI_ScreenSlashPreSlash()
        {
            //For this state, he's gonna look at the screen and then do a big screen slashing effect
            //We already made the smear shader so we can just use that on a really long timeer I think
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
            }
            float preSlashTime = 30f;
            float completionRatio = Timer / preSlashTime;
            float ease = EasingFunction.InExpo(completionRatio);
            Vector2 startCenter = TargetVector;
            Vector2 endCenter = startCenter + new Vector2(0, -252);
            Vector2 interpolatedCenter = Vector2.Lerp(startCenter, endCenter, ease);
            Vector2 targetVelocity = (interpolatedCenter - NPC.Center);
            NPC.velocity = targetVelocity;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= preSlashTime)
            {
                SwitchState(AIState.ScreenSlash_Slash);
            }
        }

        private void AI_ScreenSlashSlash()
        {
            Timer++;
            if (Timer == 1)
            {
                //Here we do a crossing slash effect similar to the one in the grab attack
                ShakeModSystem.Shake = 32;
                FXUtil.ShakeCamera(NPC.position, 1024, 4);

                Vector2 direction = new Vector2(1, 1);
                Vector2 startPosition = NPC.Center - direction * 1200;
                ScreenSmearEffectManager.NewParticle(startPosition, direction, 2400, 240);

                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -direction * MathHelper.Lerp(15, 1f, i / 3f));
                    donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

                }
                var strike = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;
                var strike2 = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
                strike2.xMult = 32;
                strike2.rotOffset += MathHelper.PiOver2;

                var strike3 = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
                strike3.xMult = 48;
                strike3.rotOffset += MathHelper.Pi;

                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, NPC.position);

                TargetVector = NPC.Center;
            }
            float slashTime = 30;
            float completionRatio = Timer / slashTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 startCenter = TargetVector;
            Vector2 endCenter = startCenter + new Vector2(0, 400);
            Vector2 interpolatedCenter = Vector2.Lerp(startCenter, endCenter, ease);
            Vector2 targetVelocity = interpolatedCenter - NPC.Center;
            NPC.velocity = targetVelocity;

            TargetOutlineColor = Color.Red;
            if (Timer >= slashTime)
            {
                SwitchState(AIState.ScreenSlash_SwordPoint);
            }
        }

        private void AI_ScreenSlashSwordPoint()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.velocity;
            }

            if (Timer % 20 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int darkStarType = ModContent.ProjectileType<DarkStar>();
                    float numProjectiles = _attackNumber % 2 == 0 ? 5 : 4;
                    float radsOffset = _attackNumber % 2 == 0 ? 0 : 45;
                    
                    
                    float radiansSpread = MathHelper.ToRadians(135 - radsOffset);
                    float startRadians = -radiansSpread / 2f;
                    float endRadians = radiansSpread / 2f;


                    float weaveoffset = _attackNumber % 2 == 0 ? 22 : 0;
                    float weaveRadians = MathHelper.ToRadians(weaveoffset);
                    for(float n = 0; n < numProjectiles; n++)
                    {
                        float ratio = n / numProjectiles;
                        Vector2 fireVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        fireVelocity *= 2;
                        fireVelocity = fireVelocity.RotatedBy(weaveRadians);

                        float rads = MathHelper.Lerp(startRadians, endRadians, ratio);
                        fireVelocity = fireVelocity.RotatedBy(rads);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, darkStarType, DarkStarDamage, 1, Main.myPlayer);
                    }
                    if(_attackNumber % 2 == 0)
                    {
                        Vector2 mainVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        mainVelocity *= 5;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, mainVelocity, darkStarType, DarkStarDamage, 1, Main.myPlayer);
                    }
                  
                    _attackNumber++;
                }
                 
            }

            float pointingTime = 180;
            float inCompletionRatio = Timer / 30;
            float inEase = EasingFunction.InExpo(inCompletionRatio);
            Vector2 targetVelocity = CalculateHoverVelocity();
            Vector2 easeVelocity = Vector2.Lerp(TargetVector, targetVelocity, inEase);
            NPC.velocity = easeVelocity;
            TargetOutlineColor = Color.Red;
            if (Timer >= pointingTime)
            {
                SwitchState(AIState.ScreenSlash_End);
            }
        }

        private void AI_ScreenSlashEnd()
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

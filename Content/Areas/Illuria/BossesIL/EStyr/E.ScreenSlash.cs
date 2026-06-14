using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{


    public class WhiteTear : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
        }

        public override void AI()
        {
            base.AI();
            Projectile.Center = Main.LocalPlayer.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawTearTexture);
            return false;
        }

        private void DrawTearTexture(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D tearTexture = TrailRegistry.LightningTrail2.Value;
            Vector2 drawOrigin = new Vector2(0, tearTexture.Height / 2f);
            Vector2 drawPosition = new Vector2(-900, -450);
            Vector2 drawScale = new Vector2(16, 0.15f);
            float outScale = (float)Projectile.timeLeft / 30f;
            outScale = EasingFunction.InOutSine(outScale);
            drawScale.Y *= outScale;
            spriteBatch.Draw(tearTexture, drawPosition, null, Color.White * 0.5f, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            drawScale.Y *= 0.5f;
            spriteBatch.Draw(tearTexture, drawPosition, null, Color.Black, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
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
                    Projectile.velocity *= 1.065f;
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
            BlackStarRenderer.QueueBlackStarDraw(this);
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
            BlackStars.AddBuff(target, 65);
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
            NPC.velocity *= 0.9f;
            Animator.PlayAnimation(Anim_Holding);
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
                ShakeScreenPosition.Shake = 64;
                FXUtil.ShakeCamera(NPC.position, 1024, 4);
                ScreenSmearEffectManager.DiagonalCut();

                Vector2 start = new Vector2(0, -550);
                Vector2 end = start + new Vector2(Main.screenWidth, Main.screenHeight);
                Vector2 velocity = end - start;
                velocity = velocity.SafeNormalize(Vector2.Zero);
                if (MultiplayerHelper.IsHost)
                {
               //     Projectile.NewProjectile(SourceFromThis, NPC.Center, new Vector2(-1, 1), ModContent.ProjectileType<WhiteTear>(), 1, 1, Main.myPlayer);
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, ModContent.ProjectileType<WhiteTear>(), 1, 1 ,Main.myPlayer);
                }
 
                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, NPC.position);
                TargetVector = NPC.Center;
            }

            float slashTime = 30;
            float completionRatio = Timer / slashTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 startCenter = TargetVector;
            Vector2 endCenter = startCenter - new Vector2(0, 32);
            Vector2 interpolatedCenter = Vector2.Lerp(startCenter, endCenter, ease);
            Vector2 targetVelocity = interpolatedCenter - NPC.Center;
            NPC.velocity = targetVelocity;
            NPC.direction = TargetDirection;
            Animator.PlayAnimation(Anim_BigSlash);
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

            if (Timer % 30 == 0)
            {
                if (_attackNumber % 2 == 0)
                {
                    Animator.PlayAnimation(Anim_ForwardSlash);
                }
                else
                {
                    Animator.PlayAnimation(Anim_BackSlash);
                }

   
                ShakeScreenPosition.Shake = 4;
                FXUtil.ShakeCamera(NPC.position, 1024, 4);

                Vector2 direction = Vector2.UnitY.RotateRandom(1.5f);
                Vector2 startPosition = NPC.Center - direction * 1200;

                Vector2 pos = NPC.Center;
                pos += Vector2.UnitX * NPC.direction * 100;
                ScreenSmearEffectManager.NewParticle(pos, direction, 2400, 15);

                var strike = LegacyParticle.NewParticle<GlowDonutParticle>(pos, direction);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;
                var strike2 = LegacyParticle.NewParticle<GlowDonutParticle>(pos, direction);
                strike2.xMult = 32;
                strike2.rotOffset += MathHelper.PiOver2;
                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                hurriSlash.Pitch = 0.8f;
                SoundEngine.PlaySound(hurriSlash, NPC.position);

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
                    for (float n = 0; n < numProjectiles; n++)
                    {
                        float ratio = n / numProjectiles;
                        Vector2 fireVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        fireVelocity *= 2;
                        fireVelocity = fireVelocity.RotatedBy(weaveRadians);

                        float rads = MathHelper.Lerp(startRadians, endRadians, ratio);
                        fireVelocity = fireVelocity.RotatedBy(rads);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, darkStarType, DarkStarDamage, 1, Main.myPlayer);
                    }
                    if (_attackNumber % 2 == 0)
                    {
                        Vector2 mainVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        mainVelocity *= 5;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, mainVelocity, darkStarType, DarkStarDamage, 1, Main.myPlayer);
                    }

                    _attackNumber++;
                }

            }

            float pointingTime = 360;
            float inCompletionRatio = Timer / 30;
            float inEase = EasingFunction.InExpo(inCompletionRatio);
            Vector2 targetVelocity = CalculateHoverVelocity();
            Vector2 easeVelocity = Vector2.Lerp(TargetVector, targetVelocity, inEase);
            NPC.velocity = easeVelocity;
            NPC.direction = TargetDirection;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class MachineGunBullet : ScarletProjectile,
        IDrawOutlines
    {

        private ref float Timer => ref Projectile.ai[0];
        private ref float Distance => ref Projectile.ai[1];
        private ref float TraveledDistance => ref Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            base.AI();

            TraveledDistance += Vector2.Distance(Projectile.position, Projectile.oldPosition);
            if(TraveledDistance >= Distance)
            {
                Projectile.Kill();
            }
            Projectile.scale = 0.5f;
            Timer++;
            if(Timer == 1)
            {
                TraveledDistance = 0;
            }
            if(TraveledDistance >= Distance / 1.2f)
            {
                Projectile.hostile = true;
            }
            if (Timer == 1)
            {
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/STARSHOOT");
                shootSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(shootSound, Projectile.Center);

                //Shoot effects
                var part = FXUtil.GlowDonutParticle(Projectile.Center, -Projectile.velocity * 0.6f, Color.Gray, Color.Pink, Color.Purple);
                part.Scale *= 0.1f;

    
                SoundStyle shootSound2 = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
                shootSound2.PitchVariance = 0.3f;
                shootSound2.Volume = 0.15f;
                SoundEngine.PlaySound(shootSound2, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.position, Color.White, Color.Yellow, Color.Red, baseSize: 0.03f, duration: 15);

                for (float f = 0; f < 3; f++)
                {
                    float rot = f / 8f;
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    var p = LegacyParticle.NewParticle<ImpactParticle>(Projectile.position, Projectile.velocity.RotatedByRandom(0.7f));
                    p.fast = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlowDust>(), 
                        Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 1f) * 2, Scale: Main.rand.NextFloat(0.25f, 0.6f), newColor: Color.Gray);
                }
            }

            if (Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Sparkle>(), Vector2.Zero, newColor: Color.White, Scale: 0.2f);
            }

            Projectile.velocity *= 1.001f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
   
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D mainTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = mainTexture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float f = i;
                float numAfterImages = TrailCacheLength;
                float progress = f / numAfterImages;
                Vector2 oldPos = OldCenterPos[i];
                Vector2 drawPos = oldPos - Main.screenPosition;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, progress)) * 0.15f;
                spriteBatch.Draw(mainTexture, drawPos, null, drawColor, OldCenterRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;// + Projectile.Size / 2f;
            spriteBatch.Draw(mainTexture, drawPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Texture2D zuiEffect = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            drawOrigin = zuiEffect.Size() / 2f;
            Color glowColor = Color.Yellow;
            glowColor.A = 0;
            glowColor *= ExtraMath.Osc(0f, 1f, speed: 64);
            glowColor *= 0.25f;
            spriteBatch.Draw(zuiEffect, drawPosition, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle shotSound = AssetRegistry.Sounds.Jiitas.JiitasGunShot;
            shotSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shotSound, Projectile.position);

            //IMPACT EFFECT
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.DarkGray, duration: 25, baseSize: 0.03f);

            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.DarkGray,
                    baseSize: Main.rand.NextFloat(0.025f, 0.05f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
       //     throw new NotImplementedException();
        }
    }
}

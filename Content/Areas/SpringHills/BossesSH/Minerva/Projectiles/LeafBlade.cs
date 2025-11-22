using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva.Projectiles
{
    public class LeafBlade : ScarletProjectile
    {
        public SlashTrailer Trailer { get; set; }
        private ref float Timer => ref Projectile.ai[0];
        private bool NoDelay
        {
            get => Projectile.ai[1] == 1;
        }

        private Vector2 ShootVelocity;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShootVelocity = Projectile.velocity;
                SoundStyle soundStyle = AssetRegistry.Sounds.Jiitas.JiitasKnifeThrow;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                  innerColor: Color.White,
                  glowColor: Color.LightGray,
                  outerGlowColor: Color.DarkGray, duration: 12, baseSize: 0.03f);
            }
            if (Timer % 4 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
            }
            if (Timer < 60 && !NoDelay)
            {
                Projectile.velocity = Vector2.Zero;
            }

            Projectile.rotation = ShootVelocity.ToRotation();

            if(NoDelay && Timer % 8 == 0)
            {
                int gore1 = GoreHelper.TypeFallingLeafWhite;
                int gore2 = GoreHelper.TypeFallingLeafRed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(4, 4), velocity, gore1);
                }

                if(Timer < 30)
                {
                    var p = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                    p.fadeToColor = Color.DarkGreen;
                    p.shrink = true;
                    p.color *= 0.8f;
                    p.Scale *= 0.6f;
                }

            }
            if (Timer == 10)
            {
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
                        baseSize: Main.rand.NextFloat(0.035f, 0.065f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }

            if (Timer == 60 && !NoDelay)
            {
                SoundStyle soundStyle = AssetRegistry.Sounds.Jiitas.JiitasKnifeSlash;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Projectile.velocity = ShootVelocity;
                Projectile.extraUpdates = 7;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!NoDelay)
            {
                Trailer ??= new SlashTrailer();
                Trailer.DrawTrail(ref lightColor, OldCenterPos);
            }
     
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            if(Projectile.velocity.Length() > 0)
            {
                for (int i = 0; i < TrailCacheLength; i++)
                {
                    Vector2 oldPos = OldCenterPos[i];
                    Vector2 oldDrawPos = oldPos - Main.screenPosition;

                    float f = i;
                    float interpolant = f / (float)TrailCacheLength;
                    Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant);
                    fadeColor *= 0.5f;
                    if (NoDelay)
                        fadeColor *= 0.5f;
                    spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, OldCenterRot[i], drawOrigin, 1, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle shotSound = AssetRegistry.Sounds.Jiitas.JiitasGunShot;
            shotSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shotSound, Projectile.position);

            //IMPACT EFFECT
            FXUtil.ShakeCamera(Projectile.position, 1024, 2);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.DarkGray, duration: 25, baseSize: 0.03f);

            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 8; i++)
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
    }
}

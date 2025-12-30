using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarMissile : ModProjectile,
        IDrawOutlines
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
        }


        public override void AI()
        {
            base.AI();
            Timer++;
            Player closest = PlayerHelper.FindClosestPlayer(Projectile.position, 256);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, degreesToRotate: 1);
            }
            if (Timer == 1)
            {
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/STARSHOOT");
                shootSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(shootSound, Projectile.Center);

                //Shoot effects
                var part = FXUtil.GlowDonutParticle(Projectile.Center, -Projectile.velocity, Color.Gray, Color.Pink, Color.Purple);
                part.Scale *= 0.5f;


                SoundStyle shootSound2 = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
                shootSound2.PitchVariance = 0.3f;
                shootSound2.Pitch = -0.5f;
                shootSound2.Volume = 0.5f;
                SoundEngine.PlaySound(shootSound2, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.position, Color.White, Color.Yellow, Color.Red, baseSize: 0.03f, duration: 15);

                for (float f = 0; f < 3; f++)
                {
                    float rot = f / 8f;
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    var p = LegacyParticle.NewParticle<ImpactParticle>(Projectile.position, Projectile.velocity.RotatedByRandom(0.7f));
                    p.fast = true;
                }
                for (float f = 0; f < 12; f++)
                {
                    Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlyphDust>(),
                        Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 1f) * 2, Scale: Main.rand.NextFloat(1.5f, 3f), newColor: Color.Gray);
                }
            }
            if (Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), Vector2.Zero, newColor: Color.DarkKhaki,
                    Scale: 0.4f);
            }

            if(Timer >= 140)
            {
                Projectile.velocity *= 0.94f;
                if(Projectile.velocity.Length() <= 1f)
                {
                    Projectile.Kill();
                }
            } else
            {
                if (Projectile.velocity.Length() < 9)
                    Projectile.velocity *= 1.1f;
                if (Projectile.velocity.Length() < 20)
                    Projectile.velocity *= 1.01f;
            }
                Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            Vector2 drawScale = Vector2.One * Projectile.scale;
            SpriteEffects spriteEffects = SpriteEffects.None;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Gray, Color.Transparent, interpolant) * 0.15f;
                oldDrawPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, null, fadeColor, Projectile.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;// + Projectile.Size / 2f;
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

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
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<StarMissileBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = SpriteEffects.None;

            Color outlineColor = Color.Red;
            Rectangle? drawFrame = null;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 scale = Vector2.One * Projectile.scale;
            float rotation = Projectile.rotation;

            spriteBatch.Draw(texture, drawPos + left, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, drawFrame, outlineColor, rotation, drawOrigin, scale, spriteEffects, 0);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Safunai.Blackwhip
{
    public class DarkwhipProj : BaseSafunaiProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Halhurish");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.Size = new Vector2(16, 48);
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        private Player Owner => Main.player[Projectile.owner];


        float t = 0;

        private int afterImgCancelDrawCount;
        private int slamTimer = 0;
        public override void AI()
        {
            base.AI();
            if(Timer % 16 == 0)
            {
                for (int j = 0; j < 1; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    ParticleManager.NewParticle(Projectile.Center, speed * 2, ParticleManager.NewInstance<BurnParticle4>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }
    

            if (t > 1)
            {

                afterImgCancelDrawCount++;
            }

            t += 0.01f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) * 0.01f;
            if (Slam)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                  innerColor: Color.LightPink,
                  glowColor: Color.Pink,
                  outerGlowColor: Color.Purple, duration: 25, baseSize: 0.24f);
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.LightPink,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.Purple, duration: 25, baseSize: 0.12f);

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
                ShakeModSystem.Shake = 4;
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<LumiDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 170, Color.Purple, 1f).noGravity = true;
                }
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
                }
            }
        }




        /*
        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            if (Projectile.timeLeft > 2)
                return false;

            Texture2D projTexture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            //End control point for the chain
            Vector2 projBottom = Projectile.Center + new Vector2(0, projTexture.Height / 2).RotatedBy(Projectile.rotation + MathHelper.PiOver2) * 0.75f;
            DrawChainCurve(Main.spriteBatch, projBottom, out Vector2[] chainPositions);

            //Adjust rotation to face from the last point in the bezier curve
            float newRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() + MathHelper.PiOver2;

            //Draw from bottom center of texture
            Vector2 origin = new Vector2(projTexture.Width / 2, projTexture.Height);
            SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

            Main.spriteBatch.Draw(projTexture, projBottom - Main.screenPosition, null, lightColor, newRotation, origin, Projectile.scale, flip, 0);
            Main.spriteBatch.Draw(glowTexture, projBottom - Main.screenPosition, null, Color.White, newRotation, origin, Projectile.scale, flip, 0);

            CurrentBase = projBottom + (newRotation - 1.57f).ToRotationVector2() * (projTexture.Height / 2);
            if (!Slam)
                return false;

            Texture2D whiteTexture = ModContent.Request<Texture2D>(Texture + "_White").Value;
            if (slamTimer < 20 && slamTimer > 5)
            {
                float progress = (slamTimer - 5) / 15f;
                float transparency = (float)Math.Pow(1 - progress, 2);
                float scale = 1 + progress;
                Main.spriteBatch.Draw(whiteTexture, projBottom - Main.screenPosition, null, Color.White * transparency, newRotation, origin, Projectile.scale * scale, flip, 0);
            }


            Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
            float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
            afterImgColor.A = 90;
            afterImgColor.B = 118;
            afterImgColor.G = 195;
            afterImgColor.R = 236;
            Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
            {
                //if(i % 2 == 0)
                float rotationToDraw;
                Vector2 interpolatedPos;
                for (float j = 0; j < 1; j += 0.25f)
                {
                    if (i == 0)
                    {
                        rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
                        interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
                    }
                    else
                    {
                        interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
                        rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
                    }
                    rotationToDraw += MathHelper.PiOver2;
                    interpolatedPos -= new Vector2(0, projTexture.Height / 2).RotatedBy(rotationToDraw) * 0.75f;
                    Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }

            return false;
        }*/
    }
}

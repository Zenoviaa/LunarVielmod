using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS
{
    public class ScytheOfSouls : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 37;
            Item.shoot = ModContent.ProjectileType<ScytheOfSoulsSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<ScytheOfSoulsStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Scythe;
        }
    }






    public class ScytheOfSoulsSlash : BaseSwingProjectileV2
    {
        private bool _playedSound;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddScytheSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Blue;

            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 64;
                },

                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.LightBlue, Color.DarkBlue, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }
            };

            Trailer = devilsPeak;
            useAfterImage = true;
            hitStopTime = EXTRA_UPDATE_COUNT * 4;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.LightBlue, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 position = target.Center;
            Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.LightBlue,
                    fadeToColor: Color.DarkBlue,
                    distortOut: true);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY, ModContent.ProjectileType<ScytheOfSoulsProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (ComboIndex == ComboCount - 1)
            {
                modifiers.FinalDamage *= 2;
            }
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle scytheHit;

            int rand = Main.rand.Next(0, 3);
            switch (rand)
            {
                default:
                case 0:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit1;
                    break;
                case 1:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit2;
                    break;
            }

            scytheHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(scytheHit, Projectile.position);
        }
    }






    public class ScytheOfSoulsStaminaSlash : BaseSwingProjectileV2
    {
        private bool _playedSound;
        private bool _flareCircle;
        private float _projCount;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
            swingSound3.PitchVariance = 0.5f;
            Add(new OvalSwing
            {
                Duration = 32,
                XSwingRadius = 64,
                YSwingRadius = 64,
                SwingDegrees = 770,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                Sound = swingSound3,
                HitCount = 6
            });

            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Blue;

            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 64;
                },

                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.LightBlue, Color.DarkBlue, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }
            };

            Trailer = devilsPeak;
            useAfterImage = true;
            hitStopTime = EXTRA_UPDATE_COUNT * 4;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.LightBlue, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            if(Interpolant > 0.5f && _projCount < 1)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity.RotatedBy(-0.5f), ModContent.ProjectileType<ScytheOfSoulsProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity, ModContent.ProjectileType<ScytheOfSoulsProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity.RotatedBy(0.5f), ModContent.ProjectileType<ScytheOfSoulsProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                _projCount++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 position = target.Center;
            Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Goldenrod,
                    fadeToColor: Color.DarkBlue,
                    distortOut: true);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (ComboIndex == ComboCount - 1)
            {
                modifiers.FinalDamage *= 2;
            }
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle scytheHit;

            int rand = Main.rand.Next(0, 3);
            switch (rand)
            {
                default:
                case 0:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit1;
                    break;
                case 1:
                    scytheHit = AssetRegistry.Sounds.Melee.ScytheHit2;
                    break;
            }

            scytheHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(scytheHit, Projectile.position);
        }
    }


    public class ScytheOfSoulsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 22;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 5;
            Projectile.knockBack = 12.9f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity.Y = -Projectile.velocity.Y * 3;
            Projectile.velocity.X = -Projectile.velocity.X * 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
        public override void AI()
        {

            Projectile.ai[1]++;
            Projectile.spriteDirection = Projectile.direction;



            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

            bool flag25 = false;
            int jim = 1;
            for (int index1 = 0; index1 < 200; index1++)
            {
                if (Main.npc[index1].CanBeChasedBy(Projectile, false)
                    && Projectile.Distance(Main.npc[index1].Center) < 800
                    && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                {
                    flag25 = true;
                    jim = index1;
                }
            }

            if (flag25)
            {
                Projectile.velocity *= 1.02f;
                float num1 = 10f;
                Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num2 = Main.npc[jim].Center.X - vector2.X;
                float num3 = Main.npc[jim].Center.Y - vector2.Y;
                float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                float num5 = num1 / num4;
                float num6 = num2 * num5;
                float num7 = num3 * num5;
                int num8 = 10;
                Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
                Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
            }
            Projectile.rotation += 0.55f;
            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, 205, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default, 1f).noGravity = false;
            }


            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default, 1f).noGravity = false;
            }
        }

        Vector2 DrawOffset;
        float alphaCounter = 7;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;





            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);


            Main.instance.LoadProjectile(Projectile.type);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(106, 255, 255), new Color(151, 46, 175), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
 
        }
    }
}
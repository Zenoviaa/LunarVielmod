using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class HolmbergScythe : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 40;
            Item.shoot = ModContent.ProjectileType<HolmbergScytheSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<HolmbergScytheProj>();
            meleeWeaponType = MeleeWeaponType.Scythe;
        }

    }
    public class HolmbergScytheSlash : BaseSwingProjectileV2
    {
        private bool _playedSound;
        private bool _flareCircle;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddScytheSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.White;
            blackFireShader.OuterColor = Color.Goldenrod;
          
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 128 * MathHelper.Lerp(0.5f, 0.0f, EasingFunction.InOutSine(Interpolant));
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.LightBlue, Color.Blue, interpolant);
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
            glowColor = Color.Lerp(Color.Transparent, Color.LightGoldenrodYellow, EasingFunction.QuadraticBump(Interpolant));
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
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
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









    public class HolmbergScytheProj : ModProjectile
    {
        Vector2 StartVelocity;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (StartVelocity == Vector2.Zero)
                StartVelocity = Projectile.velocity;
            Projectile.rotation -= 0.2f;
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 2)
            {


                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] == 5 || Projectile.ai[1] == 10)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, StartVelocity.X, StartVelocity.Y,
                            ModContent.ProjectileType<HolmbergScytheMirageProjBlue>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Scything2"), Projectile.position);
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Scything3"), Projectile.position);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Scything1"), Projectile.position);
                    }
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, StartVelocity.X, StartVelocity.Y,
                        ModContent.ProjectileType<HolmbergScytheMirageProj>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                }

            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gold) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }






    public class HolmbergScytheMirageProjBlue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.2f;
            if (Projectile.alpha >= 10)
            {
                Projectile.alpha -= 10;
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] == 1)
            {
                Projectile.alpha = 255;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 120, Projectile.position.X + 120);
                    Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 120, Projectile.position.Y + 120);
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            NPC targetNPC = NPCHelper.FindClosestNPC(Projectile.position, 512);
            if (targetNPC != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, targetNPC.Center, 3);


            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
            float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);

            SoundStyle slashSound = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit");
            slashSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(slashSound, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(189, 79, 242), new Color(79, 165, 242), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }

    public class HolmbergScytheMirageProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.2f;
            if (Projectile.alpha >= 10)
            {
                Projectile.alpha -= 10;
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] == 1)
            {
                Projectile.alpha = 255;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 120, Projectile.position.X + 120);
                    Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 120, Projectile.position.Y + 120);
                    Projectile.netUpdate = true;
                }

            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .92f;

            }

            NPC targetNPC = NPCHelper.FindClosestNPC(Projectile.position, 512);
            if(targetNPC != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, targetNPC.Center, 3);


            }


            Lighting.AddLight(Projectile.Center, Color.PaleGoldenrod.ToVector3() * 1.75f * Main.essScale);

        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
            float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
            var part = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);
            part.Scale *= 3;


            var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.DarkBlue,
                baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                duration: Main.rand.NextFloat(15, 25));
            SoundStyle slashSound = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit");
            slashSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(slashSound, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(81, 26, 255), new Color(26, 255, 255), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {


        }
    }
}

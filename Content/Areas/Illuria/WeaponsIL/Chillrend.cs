using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Chillrend : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 144;
            Item.shoot = ModContent.ProjectileType<ChillrendSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<ChillrendStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Greatsword;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
    public class ChillrendSlash : BaseSwingProjectileV2
    {
        private NPCSucker _npcSucker;
        private bool _hit;
  
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddGreatswordSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.Gray;
            blackFireShader.OuterColor = Color.Cyan;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.InOutSine(interpolant) * 186 * EasingFunction.InOutSine(1f - Interpolant);
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.DarkViolet, Color.SkyBlue, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };
            Trailer = devilsPeak;
            swordBeamLength = 180;
            outlineColor = Color.Lerp(Color.SkyBlue, Color.BlueViolet, 0.5f);
            glowAfterImageColor = Color.SkyBlue * 0.1f;
            useAfterImage = false;
            hitStopTime = EXTRA_UPDATE_COUNT * 8;
        }
        
        public override Asset<Texture2D> RequestHologramTexture()
        {
            return TextureRegistry.GlowSword_Chillrend;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Cyan, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            _npcSucker ??= new NPCSucker();
            if (Interpolant > 0.5f)
            {
                _npcSucker.AI(Projectile.Center, strength: 0.8f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.Cyan,
                        fadeToColor: Color.Purple,
                        distortOut: true);

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                }
                _hit = true;
            }
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;

            }
        }
    }
    public class ChillrendStaminaSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        public bool AuroraProj2;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerColor = Color.Gray;
            blackFireShader.OuterColor = Color.Cyan;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.InOutSine(interpolant) * 186 * EasingFunction.InOutSine(1f - Interpolant);
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.DarkViolet, Color.SkyBlue, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };
            swordBeamLength = 180;
            outlineColor = Color.Cyan;
            glowAfterImageColor = Color.SkyBlue * 0.1f;
            useAfterImage = false;
            Trailer = devilsPeak;

            Add(new OvalSwing
            {
                Duration = 64,
                XSwingRadius = 108 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 720,
                Easing = (float lerpValue) => Easing.InOutExpo(lerpValue, 7),
                Sound = swingSound1,
                HitCount = 6
            });
        }
        private bool _thrust;
        public float thrustSpeed = 5;
        public float stabRange;

        public override Asset<Texture2D> RequestHologramTexture()
        {
            return TextureRegistry.GlowSword_Chillrend;
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Cyan, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !AuroraProj2)
            {
                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Cyan,
                   outerGlowColor: Color.Purple, duration: 25, baseSize: 0.24f);


                FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Cyan,
                   outerGlowColor: Color.Purple, duration: 25, baseSize: 0.2f);

                for (float f = 0; f < 12; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.Cyan);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero,
                        ModContent.ProjectileType<ChillrendBlizzardProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                AuroraProj2 = true;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn, 180);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }
    }







    public class ChillrendBlizzardProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 512;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;

        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation += 0.05f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Chilled, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frozen, 60);
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightCyan.R,
                Color.LightCyan.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float scale = 2f;
            float progress = Timer / 60;
            float easedProgress = Easing.InOutExpo(progress);
            scale *= easedProgress;

            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor *= Easing.SpikeInOutCirc((1 - progress)) * 0.3f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation,
                drawOrigin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

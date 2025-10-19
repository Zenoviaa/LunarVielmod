
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class DevilsPeak : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<DevilsPeakSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<DevilsPeakStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Scythe;
            staminaCost = 3;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankSword>();
        }
    }

    public class DevilsPeakSlash : BaseSwingProjectileV2
    {
        private bool _playedSound;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddScytheSwingStyle(this);
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = new SlashEffect()
                {
                    BaseColor = Color.Yellow,
                    HighlightColor = Color.Orange,
                    RimHighlightColor = Color.Red,
                    WindColor = Color.DarkRed,
                    BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                    TrailTexture = TrailRegistry.SpikyTrail1.Value,
                    HighlightTexture = TrailRegistry.SpikyTrail2.Value,
                    WindTexture = TrailRegistry.WhispyTrail.Value
                },
                TrailWidthFunction = (float interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 16 * MathHelper.Lerp(1f, 0.0f, EasingFunction.InOutSine(Interpolant));
                },
                TrailColorFunction = (float interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.OrangeRed, Color.RosyBrown, interpolant);
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
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            if (Timer % 8 == 0 && Interpolant >= 0.3f)
            {
                if (!_playedSound)
                {
                    SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadiantCast1;
                    fireSound.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(fireSound, Projectile.position);
                    _playedSound = true;
                }
                var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), newColor: Color.DarkRed);
                d.velocity = -Vector2.UnitY * 3;
                d.scale *= 0.5f;
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
                    innerColor: Color.Yellow,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Red,
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
            target.AddBuff(BuffID.OnFire, 120);
            scytheHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(scytheHit, Projectile.position);
        }
    }

    public class DevilsPeakStaminaSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle chargeSound = AssetRegistry.Sounds.Melee.ScythePull;
            chargeSound.PitchVariance = 0.1f;

            Trailer = TrailPresets.CinderBreaker;
            Add(new OvalSwing
            {
                Duration = 32,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutBack(lerpValue),
                Sound = chargeSound,

            });
            useAfterImage = true;
          
        }
        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            if (Timer % 8 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), newColor: Color.Black);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                Player player = Owner;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
                float recoilStrength = 8;
                Vector2 direction = target.DirectionTo(player.Center);
                Vector2 targetVelocity = direction * recoilStrength;
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DevilsPeakBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                _hit = true;
            }
            target.AddBuff(BuffID.OnFire, 120);

        }
    }

    public class DevilsPeakBoom : ModProjectile
    {
        private float _scale = 1f;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY * 2, 8, 8, 32);
                if (StellaMultiplayer.IsHost)
                {
                    float damage = Projectile.damage;
                    damage *= 3f;
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<AgreviBoom>(), (int)damage, 3);

                }

                int count = 32;
                float degreesPer = 360 / (float)count;
                for (int k = 0; k < count; k++)
                {
                    float degrees = k * degreesPer;
                    Vector2 d = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                    Vector2 vel = d * 8;
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.Lava, vel.X * 0.5f, vel.Y * 0.5f);
                }
                for(float f = 0; f < 16; f++)
                {
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(90, 90);
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<TSmokeDust>(), velocity, newColor: Color.DarkRed);
                }

                int sound = Main.rand.Next(0, 2);
                switch (sound)
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                        break;
                }
                for (float f = 0; f < 10; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(64, 64);
                    FXUtil.GlowStretch(Projectile.Center, velocity);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TrailRegistry.BeamTrail.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            var shader = RadialBlastShader.Instance;

            float prog = Timer / 30f;
            float interp = EasingFunction.OutExpo(prog);
            shader.Offset = Vector2.Lerp(Vector2.One * 0.25f, -Vector2.One * 0.25f, interp);
            shader.Tiling = Vector2.Lerp(Vector2.One * 4, Vector2.One * 32, interp);
            shader.InnerColor = Color.Lerp(Color.Yellow, Color.Black, interp);
            shader.OuterColor = Color.Lerp(Color.Red, Color.Black, EasingFunction.OutSine(prog));
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }

}
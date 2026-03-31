
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class IyxTheInfamous : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 33;
            Item.shoot = ModContent.ProjectileType<IyxTheInfamousSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<IyxTheInfamousStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankSword>();
        }
    }

    public class IxyFireFuryPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (Player.HasBuff<FireFury>() && Player.ownedProjectileCounts[ModContent.ProjectileType<IyxFireFury>()] == 0 && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<IyxFireFury>(), 1, 1, Player.whoAmI);
            }
        }
    }

    public class IyxFireFury : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.Center = Owner.Center;
            if (Owner.HasBuff<FireFury>())
                Projectile.timeLeft = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            BubbleShader bubbleShader = BubbleShader.Instance;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: bubbleShader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White * EasingFunction.InOutSine(Timer / 60f), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
    }
    public class FireFury : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (Main.rand.NextBool(32))
            {
                var particle = FXUtil.GlowCircleDetailedBoom1(player.Top + Main.rand.NextVector2Circular(8, 8),
                      innerColor: Color.Yellow,
                      glowColor: Color.Orange,
                      outerGlowColor: Color.Red,
                      baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                      duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = -Vector2.UnitY.RotatedByRandom(0.6f) * 8;
                particle.Scale *= 0.5f;
                particle.Rotation = particle.Velocity.ToRotation();
            }
            player.GetAttackSpeed(DamageClass.Melee) += 0.25f;
        }
    }


    public class IyxTheInfamousSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        private bool _playedSound;
        private float _traveledRotation;
        private float _oldRot;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            blackFireShader.InnerEmitColor = Color.Yellow * 0.2f; 
            blackFireShader.OuterEmiteColor = Color.Red;
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return MathHelper.SmoothStep(24, 64, interpolant);
                },
                TrailColorFunction = (interpolant) =>
                {
                    return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };

            outlineColor = Color.Yellow;

            //Bloom
            useBloom = true;
            bloom.innerBloomColor = Color.OrangeRed;
            bloom.outerBloomColor = Color.Red;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;

            additive = true;
            Trailer = devilsPeak;
            useAfterImage = true;
        }

        private float GetBloomWidth(float ratio)
        {
            return MathHelper.SmoothStep(24, 64, ratio) * 1.5f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Interpolant));
        }
        private Color GetBloomColor(float ratio)
        {
            return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
        }

        public override void AI()
        {
            base.AI();
            bloomScale = MathHelper.Lerp(0.12f, 0f, EasingFunction.InExpo(Interpolant));
            glowColor = Color.Lerp(Color.Transparent, Color.Red * 0.5f, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
            if (Owner.HasBuff<FireFury>())
            {
                glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            }
            if (Timer % 16 == 0 && Interpolant >= 0.3f)
            {
                if (!_playedSound)
                {
                    SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadiantCast1;
                    fireSound.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(fireSound, Projectile.position);
                    _playedSound = true;
                }
            }


            _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
            _oldRot = Projectile.rotation;
            if (Timer % 16 == 0)
            {
                int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
                Vector2 spawnPos = swingTrailCache[index];
                var p = LegacyParticle.NewParticle<EmberParticle>(spawnPos, Main.rand.NextVector2Circular(1, 1));
               
            }

            if (_traveledRotation > 0.1f)
            {
                _traveledRotation = 0f;
                int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
                Vector2 spawnPos = swingTrailCache[index];

                index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
                int nextIndex = index + 4;
                nextIndex %= swingTrailCache.Length;

                spawnPos = swingTrailCache[index];
                Vector2 spawnPos2 = swingTrailCache[nextIndex];
                Vector2 spawnVelocity = spawnPos2 - spawnPos;
                spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
                spawnVelocity *= 24;



   
                if (Main.rand.NextBool(12))
                {


                    DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                    dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                    dp.innerColor = Color.Goldenrod;
                    dp.outerColor = Color.Red;
                    dp.gravity = 0;
                    dp.noTileCollide = true;
                    dp.fast = true;
                    dp.superFast = true;
                }

            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                Vector2 position = target.Center;
                Vector2 lvelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.Yellow,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Red,
                        distortOut: true);

    
                }

                _hit = true;
            }
            if (ComboIndex == ComboCount - 1)
            {
                SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadiantCast1;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                    LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
                }
            
            }

            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (ComboIndex == ComboCount - 1 && _hit)
            {
                Owner.AddBuff(ModContent.BuffType<FireFury>(), 120);
            }
        }
    }

    public class IyxTheInfamousStaminaSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        public bool AuroraProj2;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;


            for (int i = 0; i < 7; i++)
            {
                Add(new OvalSwing
                {
                    Duration = 22,
                    XSwingRadius = 160 / 1.5f,
                    YSwingRadius = 80 / 1.5f,
                    SwingDegrees = 360,
                    //ThrowRadius = 32,
                    Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                    Sound = swingSound1,

                });
            }
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();
            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return MathHelper.SmoothStep(128, 200, interpolant) * MathHelper.Lerp(1f, 0.0f, EasingFunction.InOutSine(Interpolant));
                },
                TrailColorFunction = (interpolant) =>
                {
                    return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };
            outlineColor = Color.Yellow;
            useBloom = true;
            bloom.innerBloomColor = Color.OrangeRed;
            bloom.outerBloomColor = Color.Red;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;
            additive = true;
            Trailer = devilsPeak;
            useAfterImage = true;
            hitStopTime = EXTRA_UPDATE_COUNT * 4;
        }
        private float GetBloomWidth(float ratio)
        {
            return MathHelper.SmoothStep(24, 64, ratio) * 1.5f;
        }
        private Color GetBloomColor(float ratio)
        {
            return Color.Lerp(Color.Red * 0.9f, Color.Transparent, EasingFunction.InExpo(ratio));
        }

        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Owner.AddBuff(ModContent.BuffType<FireFury>(), 120);
            for (float f = 0; f < 4; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                LegacyParticle.NewParticle<EmberParticle>(Owner.Center, vel);
            }
            target.AddBuff(BuffID.OnFire, 180);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle fireSound = AssetRegistry.Sounds.Magic.RadianceHit1;
            fireSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(fireSound, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;

            if (ComboIndex < ComboCount && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, ai2: combo, ai1: dir);
            }
        }
    }
}
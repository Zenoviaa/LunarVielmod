using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Swords;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class Starvast : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<StarvastSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<StarvastStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankSword>();
        }
    }

    public class StarvastSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.Starvast;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                _hit = true;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Projectile.velocity + new Vector2(0, -16),
                    ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
            soundStyle.PitchVariance = 0.2f;
            SoundEngine.PlaySound(soundStyle, target.position);
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

    public class StarvastStaminaSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        public bool AuroraProj2;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            Trailer = TrailPresets.Starvast2;
            Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,

            });

            Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });
        }
        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !AuroraProj2)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);


                for (float i = 0; i < 2f; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.Yellow,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Purple,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }


                for (float f = 0; f < 7; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.Purple);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(0, -16),
                        ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(0, 16),
                        ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity + new Vector2(16, 0),
                        ModContent.ProjectileType<StarvastStarProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);

                }
                AuroraProj2 = true;
            }
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

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;

            if (ComboIndex < 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }



    }
}
using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Swords.Altride;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class MooningSlicer : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<MooningSlicerSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<MooningSlicerStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
            staminaCost = 3;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }

    public class MooningSlicerSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.MooningSlicer;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            target.AddBuff(BuffID.Frostburn, 120);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }

    public class MooningSlicerStaminaSlash : BaseSwingProjectileV2
    {
        private bool _fire;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            Trailer = TrailPresets.MooningSlicer;
            Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1
            });
        }

        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !_fire)
            {
                for (float i = 0; i < 2f; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.LightCyan,
                        glowColor: Color.LightCyan,
                        outerGlowColor: Color.Cyan,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }


                for (float f = 0; f < 7; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel *= Main.rand.NextFloat(0.5f, 1.5f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(65));
                    Dust.NewDustPerfect(Owner.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightCyan);
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, -4), Projectile.velocity,
                        ModContent.ProjectileType<MooningProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, 4), Projectile.velocity,
                        ModContent.ProjectileType<MooningProj>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                _fire = true;
            }
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
}
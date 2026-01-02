using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Gores;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IvynAxe : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 8;
            Item.shoot = ModContent.ProjectileType<IvynAxeSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<IvynAxeStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Hammer;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(),
                material: ModContent.ItemType<Ivythorn>());
        }
    }

    public class IvynAxeSlash : BaseSwingProjectileV2
    {
        private float _hitCount;
        private bool _hit;
        private bool _playSound;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SlashTrailer slashTrailer = TrailPresets.CreateIvynSlashTrail();
            slashTrailer.TrailWidthFunction = GetTrailWidth;
            Trailer = slashTrailer;
            SwingV2Helper.AddHammerSwingStyle(this);
            useAfterImage = true;
            hitStopTime = 4 * EXTRA_UPDATE_COUNT;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 8;
        }

        public override void AI()
        {
            base.AI();
            if (!_playSound && Interpolant >= 0.5f)
            {
                SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
                leafSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(leafSound, Projectile.position);
                _playSound = true;
            }
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 oldMouseWorld = Main.MouseWorld;
            int[] gores = AutoGoreLoader.FindGores("IvynWood");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
            }
            _hitCount++;
            float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
            SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);

            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                Bounce(8);
                FXUtil.ShakeCamera(target.Center, 1024, 16);
                FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
                _hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!_hit)
            {
                modifiers.Knockback *= 0.5f;
            }
            else
            {
                modifiers.Knockback *= 2;
            }

            if (ComboIndex == ComboCount - 1)
            {
                modifiers.FinalDamage += 0.5f;
            }
        }
    }

    public class IvynAxeStaminaSlash : BaseSwingProjectileV2
    {
        private bool _thornEruption;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HammerSmash1;
            swingSound1.PitchVariance = 0.5f;

            SlashTrailer slashTrailer = TrailPresets.CreateIvynSlashTrail();
            slashTrailer.TrailWidthFunction = GetTrailWidth;
            Trailer = slashTrailer;
            Add(new OvalSwing
            {
                Duration = 70,
                XSwingRadius = 64,
                YSwingRadius = 64,
                SwingDegrees = 330,
                Easing = (lerpValue) => Easing.InOutBack(lerpValue),
                Sound = swingSound1,

            });

            useAfterImage = true;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 12;
        }

        public override void AI()
        {
            base.AI();
            growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Poisoned, 180);

            if (!_thornEruption)
            {
                Bounce(8);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Bottom - new Vector2(0, 64), (-Vector2.UnitY * 3).RotatedByRandom(2), ModContent.ProjectileType<LeafShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Bottom - new Vector2(0, 64), (-Vector2.UnitY * 3).RotatedByRandom(2), ModContent.ProjectileType<LeafShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Bottom - new Vector2(0, 64), (-Vector2.UnitY * 8).RotatedByRandom(0.3f), ModContent.ProjectileType<IvynRoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                _thornEruption = true;
            }

            SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
            boom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(boom, target.position);
            for (int i = 0; i < 16; i++)
            {
                float radius = 150;
                Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                offset *= Main.rand.NextFloat(1f, radius);
                offset += new Vector2(radius / 2, 0);

                Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                velocity *= Main.rand.NextFloat(1f, 2f);
                Dust.NewDustPerfect(target.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                    Main.rand.NextFloat(0.3f, 0.7f));
            }

            FXUtil.GlowCircleBoom(target.Bottom,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(target.Bottom,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(target.Bottom, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle hammerSmash = SoundRegistry.HammerSmash3;
            hammerSmash.PitchVariance = 0.5f;
            SoundEngine.PlaySound(hammerSmash, Projectile.position);

            modifiers.FinalDamage += 1;
            modifiers.Knockback *= 4;
        }
    }

    public class IvynRoot : ModProjectile
    {
        private Vector2 _scale;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 100;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 25;
            Projectile.localNPCHitCooldown = 25;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                int[] gores = AutoGoreLoader.FindGores("IvynWood");
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
                }

                SoundStyle soundStyle = AssetRegistry.Sounds.Magic.VineWrap;
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 32, 1);
                for (float f = 0; f < 16; f++)
                {
                    Vector2 velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                    velocity *= Main.rand.NextFloat(0f, 1f);
                    Dust.NewDustPerfect(Projectile.Center, DustID.t_LivingWood, velocity, newColor: Color.White);
                }
            }
            float interpolant = Timer / 25f;
            float ease = EasingFunction.InOutBack(interpolant);
            _scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 0.85f, ease);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer >= 24)
            {
                int[] gores = AutoGoreLoader.FindGores("IvynWood");
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin, _scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

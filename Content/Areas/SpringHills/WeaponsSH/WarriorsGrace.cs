using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    // This is a basic item template.
    // Please see tModLoader's ExampleMod for every other example:
    // https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
    public class WarriorsGrace : BaseSwingItemV2
    {
        // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.Stellamod.hjson' file.
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 126;
            Item.useAnimation = 126;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<WarriorsSwordSlash>();
            Item.autoReuse = true;
            staminaProjectileShoot = ModContent.ProjectileType<WarriorsSwordStaminaSlash>();
        }
    }

    public class WarriorsSwordSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        public override void SetDefaults2()
        {
            base.SetDefaults2();
        }
        
        private float GetBloomWidth(float ratio)
        {
            return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
        }

        private Color GetBloomColor(float ratio)
        {
            Color blue = Color.Lerp(Color.Lerp(Color.White, Color.DarkGray, 0.5f), Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 4));
            return Color.Lerp(blue * 0.9f, Color.Black, ratio);
        }


        public override void DefineCombo()
        {
            base.DefineCombo();
            SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
            SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
            slashTrailer.invert = ComboIndex % 2 != 0;
            Trailer = slashTrailer;

            useBloom = true;
            bloom.innerBloomColor = Color.White;
            bloom.outerBloomColor = Color.DarkGray;
            bloom.bloomWidthFunction = GetBloomWidth;
            bloom.bloomColorFunction = GetBloomColor;       
            SwingV2Helper.AddSwordSwingStyle(this); 
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit && ComboIndex == 5)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black, duration: 12, baseSize: 0.24f);

                Hit = true;
            }

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                    duration: Main.rand.NextFloat(5, 10));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }

    public class WarriorsSwordStaminaSlash : BaseSwingProjectileV2
    {
        public bool Hit;
        private bool _thrust;
        public float thrustSpeed = 2;
        public float stabRange;

        public SlashEffect SlashEffect { get; set; }
        public SlashTrailer SlashTrailer { get; set; }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 12;
        }

        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            swingDirection.Y = 0;
            if (EasingFunction.InOutExpo(Interpolant) > 0.5f)
            {
                if (!_thrust)
                {
                    Owner.velocity += swingDirection * thrustSpeed;
                    _thrust = true;
                }
            }
        }

        public override void DefineCombo()
        {
            base.DefineCombo();

            //Setup the slash effect
            SlashEffect = new SlashEffect()
            {
                BaseColor = Color.Gray,
                WindColor = Color.Gray,
                LightColor = Color.LightGray,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };
            SlashTrailer = new();
            SlashTrailer.TrailWidthFunction = GetTrailWidth;
            SlashTrailer.Shader = SlashEffect;
            Trailer = SlashTrailer;

            SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.HeavySwordSlash1;
            swingSound1.PitchVariance = 1f;

            float deg = 135;
            Add(new OvalSwing
            {
                Duration = 18,
                XSwingRadius = 64 / 1.5f,
                YSwingRadius = 40 / 1.5f,
                SwingDegrees = deg,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });

            Add(new OvalSwing
            {
                Duration = 18,
                XSwingRadius = 64 / 1.5f,
                YSwingRadius = 40 / 1.5f,
                SwingDegrees = deg,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1

            });

            Add(new OvalSwing
            {
                Duration = 18,
                XSwingRadius = 64 / 1.5f,
                YSwingRadius = 40 / 1.5f,
                SwingDegrees = deg,
                Easing = EasingFunction.InOutExpo,
                Sound = swingSound1
            });
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!Hit)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                Hit = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                    duration: Main.rand.NextFloat(5, 10));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = AssetRegistry.Sounds.Melee.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = AssetRegistry.Sounds.Melee.NormalSwordHit1;
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
            if (ComboIndex < 2 && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Owner.whoAmI, ai2: combo, ai1: dir);
            }
        }
    }
}
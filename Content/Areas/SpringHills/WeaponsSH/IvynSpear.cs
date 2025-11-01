using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trailing;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Core.Effects.ITrailer;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IvynSpear : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<IvynSpearSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<IvynSpearStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<Ivythorn>());
        }
    }

    public class IvynSpearSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SlashTrailer slashTrailer = TrailPresets.CreateIvynSlashTrail();
            slashTrailer.TrailWidthFunction = GetTrailWidth;
            Trailer = slashTrailer;
            SwingV2Helper.AddSpearSwingStyle(this);
            useAfterImage = true;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 8;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            target.AddBuff(BuffID.Poisoned, 60);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }

    public class IvynSpearStaminaSlash : BaseSwingProjectileV2
    {
        private bool _fire;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            SlashTrailer slashTrailer = TrailPresets.CreateIvynSlashTrail();
            Trailer = slashTrailer;
            Add(new OvalSwing
            {
                Duration = 8,
                XSwingRadius = 48,
                YSwingRadius = 24,
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
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                        ModContent.ProjectileType<IvynSpearThrow>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
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
                modifiers.FinalDamage += 0.5f;
            }
        }
    }

    public class IvynSpearThrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
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

                SoundStyle leafSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Nature.LeafRustle1 : AssetRegistry.Sounds.Nature.LeafRustle2;
                leafSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(leafSound, Projectile.position);
            }
            if (Timer % 12 == 0)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_LivingWood);
                Main.dust[d].noGravity = true;
            }
            if(Timer >= 15)
            {
                Projectile.tileCollide = true;
            }
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
        public SlashTrailer Trailer { get; set; }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int trailLength = Projectile.oldPos.Length;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;
            for (int t = 0; t < trailLength; t++)
            {
                float l = trailLength;
                float interpolant = (float)t / l;
                Vector2 oldPos = Projectile.oldPos[t];
                oldPos -= Main.screenPosition;
                oldPos += Projectile.Size / 2f;
                spriteBatch.Draw(texture, oldPos, frame, drawColor * MathHelper.SmoothStep(0.5f, 0f, interpolant), Projectile.oldRot[t], drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(Projectile.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }
            int[] gores = AutoGoreLoader.FindGores("IvynWood");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
            }
        }
    }
}

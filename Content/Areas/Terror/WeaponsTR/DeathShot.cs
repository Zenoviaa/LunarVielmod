using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class DeathShot : BaseGun
    {
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 100000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DeathSnipe>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
        }


        public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeathSnipe>(), damage, knockback, player.whoAmI, ai0: remainingAmmo);
            return false;
        }
        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            int rand = Main.rand.Next(0, 3);
            SoundStyle shootSound;
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/DeathShot");
            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/DeathShot2");
            }

            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.5f;
            SoundEngine.PlaySound(shootSound, position);
            FXUtil.GlowCircleBoom(position, Color.White, Color.Red, Color.DarkRed, baseSize: 0.03f, duration: 15);

            for (float f = 0; f < 3; f++)
            {
                float rot = f / 8f;
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                var p = LegacyParticle.NewParticle<ImpactParticle>(position, velocity.RotatedByRandom(0.7f));
                p.fast = true;
                p.color = Color.DarkRed;
            }

            for (float f = 0; f < 5; f++)
            {
                var dp = Particle<DustParticle>.Spawn(position, velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.25f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
                dp.gravity = 0.02f;
                dp.outerColor = Color.Red;
                dp.dampening = 0.1f;
            }
        }

        public override void SetMagazine(ref GunReloadParams fireParams)
        {
            base.SetMagazine(ref fireParams);
            fireParams.maxAmmo = 3;
            fireParams.reloadWindow = 150;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<TerrorFragments>());
        }
    }
    public class DeathSnipe : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float RemainingAmmo => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 10 == 0)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.Red,
                    outerColor = Color.DarkRed
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.3f;
            }

            if (Timer % 20 == 0)
            {
                DustParticle dp = DustParticle.Spawn(Projectile.Center, Vector2.Zero, DustParticleSpawnParams.Default);
                dp.gravity = 0;
                dp.Scale *= 0.5f;
            }

            Projectile.velocity *= 1.005f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            float damageMultiplier = MathHelper.Lerp(1.5f, 1f, RemainingAmmo / 3f);
            modifiers.FinalDamage *= damageMultiplier;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);
            float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            return MathHelper.SmoothStep(8, 0, completionRatio) * EasingFunction.QuadraticBump(completionRatio) * MathF.Sin(completionRatio * 8 + Main.GlobalTimeWrappedHourly * 8);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Red, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Red, Color.DarkRed, 0.75f);
            shader.OuterColor = Color.DarkRed;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size * 0.5f);
        }
        public override void OnKill(int timeLeft)
        {
            if (RemainingAmmo == 0)
            {
                var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.White,
                   glowColor: Color.Red,
                   outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<DeathShotBomb>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 0);
                }
            }
            else
            {
                var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Red,
            outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
                p3.Scale *= 0.66f;
            }
            for (float n = 0; n < 6f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.White;
                spawnParams.outerColor = Color.DarkRed;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.Lerp(Color.White,Color.Black, 0.8f);

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }
    }
}
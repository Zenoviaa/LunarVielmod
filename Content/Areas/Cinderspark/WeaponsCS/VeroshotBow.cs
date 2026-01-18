using Stellamod.Assets;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class VeroshotBlast : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.OrangeRed,
                    outerColor = Color.Red
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.6f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.85f;
                sp.Scale *= 0.3f;

                SoundStyle fireballShot = AssetManager.GetSound("Fire/FireballShoot2");
                fireballShot.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireballShot, Projectile.position);
            }

            if(Timer % 8 == 0 && this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY * Main.rand.NextFloat(3f, 6f), 
                    ModContent.ProjectileType<VeroshotFlare>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if (Main.rand.NextBool(16))
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Yellow,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(0.5f, 0.9f) * Projectile.scale
                };
                DustParticle.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(1.5f), spawnParams);
            }

            Projectile.scale = MathHelper.SmoothStep(0f, 1f, ((float)Projectile.timeLeft / 120f));
            Projectile.velocity *= 1.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            for(int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawCenter = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                float completionRatio = ((float)i / (float)Projectile.oldPos.Length);
                Color glowColor = Color.Lerp(Color.White, Color.Black, MathHelper.SmoothStep(0f, 1f, completionRatio));
                glowColor *= 0.3f;
                glowColor.A = 0;
                spriteBatch.Draw(texture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            this.DrawCentered(ref lightColor);
            return false;
        }
    }


    public class VeroshotFlare : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 24;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 7 == 0)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Yellow,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(0.5f, 0.9f) * Projectile.scale
                };
                var dp = DustParticle.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(1.5f), spawnParams);
                dp.parent = Projectile;
                if (Main.rand.NextBool(3))
                {
                    DustParticle.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(1.5f), spawnParams);
                }
                if (Main.rand.NextBool(6))
                {
                    SmokeParticle sp = SmokeParticle.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(1.5f), Scale: Main.rand.NextFloat(0.25f, 0.5f));
                    sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.75f);
                    sp.Scale *= Projectile.scale;
                }
            }
            Projectile.velocity.Y += 0.5f;
            Projectile.scale = MathHelper.SmoothStep(0f, 1f, ((float)Projectile.timeLeft / 180f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
        }
    }

    public class VeroshotBow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 25;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 40f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.channel = true;
            Item.scale = 1f;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.ShootBow(player, source, shootParams);
            int Sound = Main.rand.Next(1, 3);
            SoundStyle shootSound;
            if (Sound == 1)
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot");
            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2");
            }

            shootSound.Volume = 0.5f;
            shootSound.PitchVariance = 0.25f;
            Vector2 position = shootParams.position;
            Vector2 velocity = shootParams.velocity * shootParams.speed;
            int damage = shootParams.damage;
            float knockback = shootParams.knockBack;
            SoundEngine.PlaySound(shootSound, position);

            float numberProjectiles = 4;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.HellfireArrow, damage, knockback, player.whoAmI);
            }
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            Projectile.NewProjectile(source, shootParams.position, shootParams.fireVelocity, ModContent.ProjectileType<VeroshotBlast>(), shootParams.damage, shootParams.knockBack, player.whoAmI);
        }
    }
}

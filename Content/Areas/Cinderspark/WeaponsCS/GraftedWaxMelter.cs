using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class GraftedWaxMelter : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToArtifact();
            Item.damage = 39; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.width = 20; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 18; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 18; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 3; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Orange; // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item42; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<WaxBall>();
            Item.shootSpeed = 18f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.mana = 8;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class WaxBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle sound = AssetManager.GetSound("Waxing");
                sound.PitchVariance = 0.3f;
                sound.Volume = 0.75f;
                SoundEngine.PlaySound(sound, Projectile.position);

                float numDust = 24;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    WaxParticle.SpawnInAlphaLayer(Projectile.Center, velocity, Color.White, Scale: Main.rand.NextFloat(0.75f, 1f));
                }

                numDust = 12;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16) * 2f;
                    SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, velocity, Color.White, Scale: Main.rand.NextFloat(0.75f, 1f));
                    sp.flickering = true;
                    sp.innerColor = Color.OrangeRed;
                    sp.outerColor = Color.DarkRed;
                }

                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
                }
                for (int i = 0; i < 1; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
                }

                numDust = 4;
                for (int n = 0; n < numDust; n++)
                {
                    var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
                    sp.initialColor = Color.Brown;
                }

                for (int n = 0; n < numDust; n++)
                {
                    var dp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 25), Scale: Main.rand.NextFloat(1f, 2f));
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 2);
            }
        }
    }

    public class WaxBall : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 35;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaxBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity *= 0.33f;
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.98f;
            if (Projectile.velocity.Length() <= 0.5f)
            {
                Projectile.scale *= 1.01f;
                if (Projectile.scale >= 1.5f)
                {
                    Projectile.Kill();
                }
            }

            if (Main.rand.NextBool(5))
            {
                WaxParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Color.White, Main.rand.NextFloat(0.5f, 1f));
            }

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        break;
                }
            }
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor);
            Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = glowTexture.Size() * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.Red;
            glowColor *= ExtraMath.Osc(0.5f, 1f);
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}
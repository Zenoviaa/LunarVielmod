using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
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
            Item.damage = 17; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.width = 20; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 36; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 36; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 3; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Orange; // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item42; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<WaxBall>();
            Item.shootSpeed = 18f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.mana = 25;
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle sound = AssetManager.GetSound("Fire/Waxing");
                sound.PitchVariance = 0.3f;
                sound.Volume = 0.75f;
                SoundEngine.PlaySound(sound, Projectile.position);

                var p = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Red);
                p.Scale *= 1.5f;
                
                float numDust = 12;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(6, 6);
                    WaxParticle w = WaxParticle.SpawnInAlphaLayer(Projectile.Center, velocity, Color.White, Scale: Main.rand.NextFloat(0.75f, 1f));
                    
                }

                numDust = 12;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8) * 2f;
                    SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, velocity, Color.White, Scale: Main.rand.NextFloat(0.25f, 1f));
                    sp.flickering = true;
                    sp.innerColor = Color.OrangeRed;
                    sp.outerColor = Color.Red;
                    sp.gravity = 0f;
                    sp.dampening = 0.1f;
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
                    sp.initialColor = Color.White;
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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaxBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
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
            if(Timer < 15)
            {
                Projectile.scale = MathHelper.SmoothStep(0f, 1f, Timer / 15f);
            }
            Projectile.velocity *= 0.98f;
            if (Projectile.velocity.Length() <= 0.5f || Projectile.timeLeft < 15)
            {
                Projectile.scale *= 1.1f;
                if (Projectile.scale >= 1.5f)
                {
                    Projectile.Kill();
                }
            }
            if (Timer % 16 == 0)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.OrangeRed,
                    outerColor = Color.Red,
                    scaleRange = new Vector2(1.5f, 2f) * Projectile.scale
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

            if (Main.rand.NextBool(12))
            {
                WaxParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Color.White, Main.rand.NextFloat(0.5f, 1f));
            }

            Projectile.rotation = Projectile.velocity.X * -0.05f;
            NPC closest = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
            if (closest != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 1);
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = glowTexture.Size() * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color glowColor = Color.Red;
            glowColor *= ExtraMath.Osc(0.5f, 1f);
            glowColor *= 0.3f;
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            this.DrawCentered(ref lightColor);


            float interp = Timer / 180f;
            float ease = MathHelper.SmoothStep(1f, 0f, interp);
            glowColor = Color.Red;
            glowColor *= ExtraMath.Osc(0.5f, 1f, speed: 16);
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter - new Vector2(0, 16 + 24 * ease).RotatedBy(Projectile.rotation), null, glowColor, 0, drawOrigin, Projectile.scale * 0.08f, SpriteEffects.None, 0);

            glowColor = Color.Yellow;
            glowColor *= ExtraMath.Osc(0.5f, 1f, speed: 16);
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter - new Vector2(0, 16 + 24 * ease).RotatedBy(Projectile.rotation), null, glowColor, 0, drawOrigin, Projectile.scale * 0.05f, SpriteEffects.None, 0);
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
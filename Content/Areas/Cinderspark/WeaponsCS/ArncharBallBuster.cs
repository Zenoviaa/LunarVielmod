using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class FireballShot : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.timeLeft = 180;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Charge *= 0.66f;
            Projectile.velocity *= 0.5f;
            Projectile.netUpdate = true;

            FXUtil.ShakeCamera(target.Center, 1024, 32);
            var b = FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);
            b.Scale *= Charge;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 4; f++)
            {
                var d = Particle<DustParticle>.Spawn(target.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
                d.Scale *= Charge;
            }

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
                smoke.Scale *= Charge;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
                particle.Scale *= Charge;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage *= Charge * 2;
        }

        public override void AI()
        {
            base.AI();

            Projectile.scale = Charge;
            Timer++;
            if(Timer < 10)
            {
                Projectile.velocity *= 1.2f;
            } else if (Timer < 20)
            {
                Projectile.velocity *= 0.8f;
            }
            if (Timer == 1)
            {
                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 4 * MathHelper.Lerp(15, 1f, i / 3f));
                    donutParticle.Scale *= MathHelper.Lerp(0.3f, 1f, i / 3f);
                    donutParticle.Velocity *= 0.1f;
                    donutParticle.innerColor = Color.Red;
                    donutParticle.outerColor = Color.DarkRed;
                    donutParticle.Scale *= Projectile.scale;
                }

                SoundStyle impact = AssetManager.GetSound("Fire/FireballShoot1");
                impact.PitchVariance = 0.3f;
                SoundEngine.PlaySound(impact, Projectile.position);
            }

            if(Timer > 60)
            {
                Projectile.tileCollide = true;
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
                        sp.Scale *= Projectile.scale;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        sp2.Scale *= Projectile.scale;
                        break;
                }

            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
                sp.Scale *= Projectile.scale;
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            CreateImpactEffects();
        }
        private void CreateImpactEffects()
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
            }

            int numDust = 8;
            for (int n = 0; n < numDust; n++)
            {
                var sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
                sp.initialColor = Color.Brown;
            }

            for (int n = 0; n < numDust; n++)
            {
                var dp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 45), Scale: Main.rand.NextFloat(1f, 2f));
            }


            SoundStyle impact = AssetManager.GetSound("Fire/FireballImpact1");
            impact.PitchVariance = 0.3f;
            SoundEngine.PlaySound(impact, Projectile.position);
            ShakeScreenPosition.Shake = 3;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sawTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = sawTexture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.Yellow;
            drawColor.A = 0;
            spriteBatch.Draw(sawTexture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);


            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);
            // spriteBatch.RestartDefaults();


            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color afo = glowColor;
                afo = Color.Lerp(afo, Color.Black, MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutExpo7(ratio)));
                afo.A = 0;
                afo *= 0.15f;
                spriteBatch.Draw(glowMask, oldCenter, null, afo, Projectile.oldRot[i], glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
    }

    public class FireballCharge : ModProjectile
    {
        private float _charge;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle impact = AssetManager.GetSound("Fire/FireballCharge1");
                impact.PitchVariance = 0.3f;
                SoundEngine.PlaySound(impact, Projectile.position);
            }

            if (this.OwnedByLocalClient())
            {
                Vector2 targetPosition = Owner.Center + (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 64;
                Vector2 targetVelocity = targetPosition - Projectile.Center;
                Projectile.velocity = targetVelocity;
                Projectile.netUpdate = true;
            }

            if (this.OwnedByLocalClient() && !Owner.channel)
            {
                Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * _charge * 15;
                int damage = (int)(Projectile.damage * _charge);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<FireballShot>(), damage, Projectile.knockBack, Projectile.owner, ai1: _charge);
                Projectile.Kill();
            }

            if (Timer % 4 == 0 && _charge < 1)
            {
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2CircularEdge(256, 256);
                Vector2 velocity = Projectile.Center - spawnPoint;
                velocity *= 0.05f;
                var p = FXUtil.GlowStretch(spawnPoint, velocity);
                p.InnerColor = Color.Yellow;
                p.OuterGlowColor = Color.Red;
                p.VectorScale *= 0.4f;
            }

            if (Main.rand.NextBool(3))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                            Main.rand.NextVector2Circular(12, 12), Scale: Main.rand.NextFloat(1f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(12, 12), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        break;
                }

            }

            _charge = MathHelper.Clamp(Timer / 120f, 0f, 1f);
       

            Projectile.scale = MathHelper.SmoothStep(0f, 1f, Timer / 120f);
            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sawTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = sawTexture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.Yellow;
            drawColor.A = 0;
            spriteBatch.Draw(sawTexture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color afo = glowColor;
                afo = Color.Lerp(afo, Color.Black, MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutExpo7(ratio)));
                afo.A = 0;
                afo *= 0.15f;
                spriteBatch.Draw(glowMask, oldCenter, null, afo, Projectile.oldRot[i], glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
    }

    public class ArncharBallBuster : BaseGun
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //base.SetDefaults();
            Item.damage = 45;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 40f;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noUseGraphic = true;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<FireballCharge>();
        }

        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            //base.ShootEffects(position, velocity);
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player) && player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
}

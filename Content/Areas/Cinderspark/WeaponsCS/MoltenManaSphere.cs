using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class MoltenManaSphere : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToManaSphere(ModContent.ProjectileType<MoltenManaSphereHold>(), ModContent.ProjectileType<MoltenFireball>());
            Item.shoot = ModContent.ProjectileType<MoltenManaBlast>();
            Item.shootSpeed = 15;
            Item.damage = 13;
            Item.UseSound = SoundID.DD2_BetsyFireballShot;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
    public class MoltenFireball : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
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

            if (Projectile.velocity.Length() > 2f)
                Projectile.velocity *= 0.99f;

            Projectile.scale = MathHelper.SmoothStep(0f, 1f, Timer / 15f);

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.rotation += 0.25f;

            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 800);
            if (nearest == null)
                return;
            if (Timer > 15)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 12);
                Projectile.velocity = homingVelocity;
            }
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 3f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 120);
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


            ShakeScreenPosition.Shake = 3;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 3f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            for (int n = 0; n < 3; n++)
            {
                SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY, Color.White, Scale: 1.5f);
                sp.initialColor = Color.White * 0.24f;
            }
            CreateImpactEffects();

        }
    }

    public class MoltenManaBlast : ModProjectile
    {
        private Vector2 _initialVelocity;
        private Vector2 HomingTarget;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_initialVelocity);
            writer.WriteVector2(HomingTarget);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _initialVelocity = reader.ReadVector2();
            HomingTarget = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _initialVelocity = Projectile.velocity;

            }
            if(Timer > 30)
            {
                Projectile.tileCollide = true;
            }

            if (Timer == 5)
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
            }
            if (Projectile.velocity.Length() < 25)
            {
                Projectile.velocity *= 1.01f;
            }

            FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.Red;
            dp.parent = Projectile;
            dp.gravity = 0f;
            dp.dampening = 0.05f;
            dp.fast = true;
            dp.Scale *= 0.1f;

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        sp.Scale *= 0.25f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        sp2.Scale *= 0.25f;
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
                sp.Scale *= 0.25f;
            }


            if (Main.myPlayer == Projectile.owner)
            {
                HomingTarget = Main.MouseWorld;


                Projectile.netUpdate = true;
            }
            if (Timer < 30f)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, HomingTarget);
                Projectile.velocity = Vector2.Lerp(_initialVelocity, homingVelocity, EasingFunction.InOutSine(Timer / 30f));
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One * 0.1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(3f, 1.3f) * 0.75f;
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Yellow;
            shader.InnerColor = Color.Orange;
            shader.OuterColor = Color.Red;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;

            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
        }
    }

    public class MoltenManaSphereHold : AbstractManaSphereHold
    {
        public override string Texture => ModContent.GetInstance<MoltenManaSphere>().Texture;
        public override void AI_OrbitPlayer()
        {
            base.AI_OrbitPlayer();

        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Timer % 5 == 0)
            {
                FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                dp.innerColor = Color.Goldenrod;
                dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                dp.fast = true;
                dp.Scale *= 0.25f;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHead(Main.spriteBatch, Main.screenPosition);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Yellow;
            shader.InnerColor = Color.Orange;
            shader.OuterColor = Color.Red;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;

            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        private void DrawHead(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One * 0.1f;

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(1.5f, 1.3f);
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.WeaponsSN
{
    public class IceManaSphere : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToManaSphere(ModContent.ProjectileType<IceManaSphereHold>());
            Item.shoot = ModContent.ProjectileType<IceManaBlast>();
            Item.shootSpeed = 15;
            Item.damage = 13;
            Item.UseSound = SoundID.DD2_BetsyFireballShot;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }
    public class IceManaBlast : ModProjectile
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
            Projectile.tileCollide = true;
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

            if (Timer == 5)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.LightSkyBlue,
                    outerColor = Color.DarkBlue
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

            if (Main.rand.NextBool(3))
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;

            }

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
                        FlakeParticle sp2 = Particle<FlakeParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 8), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        //sp2.fast = true;
                        sp2.dampening = 0.1f;
                       
                        break;
                }

            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.White, Scale: Main.rand.NextFloat(0.35f, 0.75f));
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
            Color glowColor = Color.Lerp(Color.LightCyan, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
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
            return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.LightCyan;
            shader.InnerColor = Color.Cyan;
            shader.OuterColor = Color.Blue;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
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
                spawnParams.innerColor = Color.LightSkyBlue;
                spawnParams.outerColor = Color.DarkBlue;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;
        }
    }

    public class IceManaSphereHold : AbstractManaSphereHold
    {
        public override string Texture => ModContent.GetInstance<IceManaSphere>().Texture;
        public override void AI_OrbitPlayer()
        {
            base.AI_OrbitPlayer();

        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Timer % 5 == 0)
            {
                FlakeParticle dp = Particle<FlakeParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2.5f, 2.5f), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                //   dp.innerColor = Color.Goldenrod;
                // dp.outerColor = Color.Red;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                // dp.fast = true;
                dp.Scale *= 0.8f;
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
            shader.LaserColor = Color.LightCyan;
            shader.InnerColor = Color.Cyan;
            shader.OuterColor = Color.Blue;
            shader.LaserTexture = TrailRegistry.SpikyTrail1;
            shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
        private void DrawHead(SpriteBatch spriteBatch, Vector2 screenPos)
        {

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawScale = Vector2.One * 0.1f;

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.LightCyan, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);

            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.SkyBlue;
            glowColor.A = 0;
            glowColor *= 0.5f;
            Vector2 stretch = new Vector2(1.5f, 1.3f);
            spriteBatch.Draw(glowMask, drawPos - Projectile.velocity * 1.5f, null, glowColor, Projectile.velocity.ToRotation(), glowDrawOrigin, drawScale * Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * stretch, SpriteEffects.None, 0);
        }
    }
}

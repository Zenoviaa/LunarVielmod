using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
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
            Item.DefaultToManaSphere(ModContent.ProjectileType<MoltenManaSphereHold>());
            Item.shoot = ModContent.ProjectileType<MoltenManaBlast>();
            Item.UseSound = SoundID.DD2_BetsyFireballShot;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
    public class MoltenManaBlast : ModProjectile
    {
        private Vector2 _initialVelocity;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_initialVelocity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _initialVelocity = reader.ReadVector2();
        }
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


            FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(0.2f, 0.35f));
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.Red;
            dp.parent = Projectile;
            dp.gravity = 0f;
            dp.dampening = 0.05f;
            dp.fast = true;

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

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
            }

            NPC nearest = NPCHelper.FindClosestNPC(Projectile.Center, 384);
            if (nearest == null)
                return;

            Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center);
            Projectile.velocity = Vector2.Lerp(_initialVelocity, homingVelocity, EasingFunction.InOutSine(Timer / 30f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
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
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);
            float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            return (Projectile.width * Projectile.scale) * osc * 2 * w;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 32));
        }
        private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Yellow, Color.OrangeRed, ExtraMath.Osc(0f, 1f, speed: 3));
            shader.OuterColor = Color.Red;
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
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.Yellow,
                glowColor: Color.Red,
                outerGlowColor: Color.DarkRed, duration: 25, baseSize: boomSize);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Yellow,
               glowColor: Color.Red,
               outerGlowColor: Color.DarkRed, duration: 15, baseSize: boomSize * 2);
        }
    }

    public class MoltenManaSphereHold : AbstractManaSphereHold
    {
        public override string Texture => ModContent.GetInstance<MoltenManaSphere>().Texture;
        public override void AI_OrbitPlayer()
        {
            base.AI_OrbitPlayer();
            if (Timer % 8 == 0)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Vector2.Zero);
                dp.gravity = 0;
            }
        }
    }
}

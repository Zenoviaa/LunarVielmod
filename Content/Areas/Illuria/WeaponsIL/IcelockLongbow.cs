using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class IcelockArrow : ScarletProjectile
    {
        private BlackFireShader FireShader;
        private SlashTrailer Trailer;
        public override string Texture => TextureRegistry.ZuiEffect;
        private ref float Timer => ref Projectile.ai[0];
        private bool Small => Projectile.ai[1] == 1;
        public override void SetDefaults()
        {
            TrailCacheLength = 24;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (Small)
                Projectile.extraUpdates = 0;

            if (Timer >= 10)
                Projectile.tileCollide = true;
            Timer++;
            if (Timer % 15 == 0)
            {
                Particle<DustParticle>.Spawn(Projectile.Center, Vector2.Zero, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if(nearest != null)
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center);
            if(Projectile.velocity.Length() < 15f)
                Projectile.velocity *= 1.01f;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.LightCyan.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }


        public float GetTrailWidth(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color GetTrailColor(float completionRatio)
        {
            Color lerp1 = Color.Lerp(Color.DarkViolet, Color.SkyBlue, completionRatio);
            return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(completionRatio));
        }


        private void DrawPixelTrail(GraphicsDevice graphicsDevice)
        {
            FireShader ??= new BlackFireShader();
            FireShader.SetDefaults();
            FireShader.InnerColor = Color.Gray;
            FireShader.OuterColor = Color.Cyan;
            Trailer ??= new SlashTrailer
            {
                Shader = FireShader,
                TrailWidthFunction = GetTrailWidth,
                TrailColorFunction = GetTrailColor
            };

            Color lightColor = Color.White;
            Trailer.DrawTrail(ref lightColor, OldCenterPos);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float scale = 1f;
            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                float rotOffset = MathHelper.TwoPi * (i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelTrail, DrawLayer.OverNPCsAdditive);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (this.OwnedByLocalClient())
            {
                Vector2 initialVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 2;

                int steps = 16;
                Vector2 velocity = initialVelocity.RotatedByRandom(0.6f);
                Vector2 icicleCenter = Projectile.Center + oldVelocity;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), icicleCenter,
                    velocity, ModContent.ProjectileType<IcicleFormation>(), 1, 1, Projectile.owner, ai1: steps, ai2: -1);
            }
  
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            initialVelocity *= 2;


            int steps = Main.rand.Next(2, 3);
            Vector2 velocity = initialVelocity.RotatedByRandom(0.6f);
            Vector2 icicleCenter = target.Center;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), icicleCenter,
                velocity, ModContent.ProjectileType<ShortIcicleFormation>(), 1, 1, Projectile.owner, ai1: steps, ai2: target.whoAmI);
            target.AddBuff(ModContent.BuffType<Frosting>(), 360);

        }
        public override void OnKill(int timeLeft)
        {
            if (!Small)
            {
                for (float f = 0; f < 6; f++)
                {
                    Vector2 initialVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.2f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }
                SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine;
                parendineHitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(parendineHitSound, Projectile.Center);

                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);


                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightBlue,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            for (float f = 0; f < 2; f++)
            {
                Vector2 initialVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 12;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                DustParticle dustParticle = Particle<DustParticle>.Spawn(Projectile.velocity, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }
            for (float f = 0; f < 6; f++)
            {
                Vector2 initialVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 12;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                SparkParticle dustParticle = LegacyParticle.NewParticle<SparkParticle>(Projectile.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }

            for (float f = 0; f < 3; f++)
            {
                Vector2 initialVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 4;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center + initialVelocity,
                    initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                smokeParticle.fadeToColor = Color.Black;
            }

            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightSkyBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: 0.06f);
        }
    }

    public class IcelockLongbow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 212;
        }

        private void ShootArrow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            float bowDamage = shootParams.damage * shootParams.chargeStrength * 2;
            Vector2 bulletVelocity = shootParams.velocity * shootParams.chargeStrength * 32;
            Projectile.NewProjectile(source, shootParams.position, bulletVelocity,
                ModContent.ProjectileType<IcelockArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI, ai1: 1);
        }
        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            FunctionRepeatHelper.Repeat(() => ShootArrow(player, source, shootParams), repeats: 2, rate: 4);
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            base.StaminaShootBow(player, source, shootParams);
            float bowDamage = shootParams.damage * shootParams.chargeStrength * 2;
            Vector2 bulletVelocity = shootParams.velocity * shootParams.chargeStrength * 32;
            Projectile.NewProjectile(source, shootParams.position, bulletVelocity,
                ModContent.ProjectileType<IcelockArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI);
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
}

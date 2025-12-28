using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.WeaponsMT
{
    public class CrescentMoonTome : BaseTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 28;
            Item.mana = 10;
            Item.shootSpeed = 2;
            Item.shoot = ModContent.ProjectileType<MiniCrescentMoon>();
        }
        public override Color GetTomeHintColor()
        {
            return Color.LightSkyBlue;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankStaff>();
        }
    }
    public class MiniCrescentMoon : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private bool Bounce
        {
            get => Projectile.ai[1] == 1;
            set
            {
                if (value)
                {
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 15 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(4, 4), Color.White, Scale: 0.5f);
                dp.gravity = 0;
                dp.dampening = 0.1f;
                dp.outerColor = Color.SkyBlue;
            }
            if (Bounce)
            {
                Projectile.velocity.Y += 0.02f;
            }
            else
            {
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= MathHelper.Lerp(1.0001f, 1.1f, Timer / 60f);
            }

            Projectile.rotation += 0.05f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f * MathF.Sign(Projectile.velocity.X);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Bounce)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                for (float f = 0; f < 1; f++)
                {
                    DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(0.3f), Color.White, Scale: 0.5f);
                    dp.dampening = 0.1f;
                    dp.outerColor = Color.SkyBlue;
                }
                Bounce = true;
            }
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 3; f++)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(0.3f), Color.White, Scale: 0.5f);
                dp.dampening = 0.1f;
                dp.outerColor = Color.SkyBlue;
            }
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
                particle.Scale *= 0.5f;
            }
            FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);

        }


        private Color GetTrailColor(float completionRatio)
        {
            Color color = Color.Lerp(Color.White, Color.LightSkyBlue, completionRatio);
            float alpha = EasingFunction.QuadraticBump(completionRatio);
            return color * alpha;
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(36, 0, completionRatio);
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserShader.Instance;
            shader.LaserTexture = TrailRegistry.StarTrail;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.SkyBlue;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size / 2f);
        }

        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = (float)i / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(Color.White, Color.Transparent, ratio);
                drawColor *= 0.25f;

                Vector2 drawCenter = Projectile.oldPos[i] + Projectile.Size / 2f - screenPos;
                float scale = MathHelper.Lerp(1f, 0f, ratio);
                spriteBatch.Draw(texture, drawCenter, null, drawColor, Projectile.oldRot[i], drawOrigin, scale, SpriteEffects.None, 0);
            }
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            spriteBatch.Draw(texture, drawCenter, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);
            DrawAfterImage(Main.spriteBatch, Main.screenPosition);
            DrawSprite(Main.spriteBatch, Main.screenPosition);
            return false;
        }
    }
}

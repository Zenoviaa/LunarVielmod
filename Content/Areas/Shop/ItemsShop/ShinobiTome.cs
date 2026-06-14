using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class ShinobiTome : AbstractMagicTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<ShinobiKnife>();
            Item.shootSpeed = 16f;
            Item.damage = 10;
            Item.mana = 12;
        }

        public override Color GetTomeHintColor()
        {
            return Color.Tan;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ShinobiKnife>(), damage, knockback, player.whoAmI);
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class ShinobiKnifeLodged : ScarletProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => ModContent.GetInstance<ShinobiKnife>().Texture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2;
            float scale = Projectile.scale;
            float rotation = Projectile.rotation;
            float alpha = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 180f));
            spriteBatch.Draw(texture, drawPos, drawFrame, Color.White.MultiplyRGB(lightColor) * alpha, rotation, drawOrigin, scale, spriteEffects, 0);
            return false;
        }
    }

    public class ShinobiKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + oldVelocity, oldVelocity,
                    ModContent.ProjectileType<ShinobiKnifeLodged>(), 0, 0, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return base.OnTileCollide(oldVelocity);
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
         
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(6, 0, completionRatio);
        }

        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 4f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCs);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 oldPos = Projectile.oldPos[k];
                Vector2 drawPosition = oldPos + Projectile.Size / 2f - Main.screenPosition;
                Color afterImageAlpha = drawColor;
                float f = (float)k / (float)Projectile.oldPos.Length;
                float alpha = MathHelper.Lerp(1f, 0f, f);
                afterImageAlpha *= alpha;
                spriteBatch.Draw(texture, drawPosition, null, afterImageAlpha, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
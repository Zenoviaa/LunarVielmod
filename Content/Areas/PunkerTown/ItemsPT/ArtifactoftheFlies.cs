using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT
{
    public class ArtifactoftheFlies : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToArtifact();
            Item.DamageType = DamageClass.Magic;
            Item.damage = 20;
            Item.mana = 8;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.shoot = ModContent.ProjectileType<FlyStorm>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MarshScrap, BlankStaff>();
        }
    }

    public class FlyStorm : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float Rotation => ref Projectile.ai[1];
        private ref float RandSize => ref Projectile.ai[2];
        private Vector2 CurrentOrigin;
        private Vector2 Origin;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(Origin);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Origin = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.timeLeft = 320;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                CurrentOrigin = Projectile.Center;
                SoundStyle sound = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
                sound.PitchVariance = 0.4f;
                sound.Volume = 0.5f;
                SoundEngine.PlaySound(sound, Projectile.position);
            }


            if (Timer == 1 && this.OwnedByLocalClient())
            {
                Rotation = Main.rand.NextFloat(-3f, 3f);
                RandSize = Main.rand.NextFloat(0.25f, 1f);
                Projectile.netUpdate = true;
            }
            if (this.OwnedByLocalClient())
            {
                Origin = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(32))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
            }


            CurrentOrigin = Vector2.Lerp(CurrentOrigin, Origin, 0.1f);
            float xRadius = MathF.Sin(Timer * 0.05f) * 64 * RandSize;
            float yRadius = MathF.Cos(Timer * 0.05f) * 32 * RandSize;
            Vector2 ovalOffset = new Vector2(xRadius, yRadius);
            ovalOffset = ovalOffset.RotatedBy(Rotation);
            Vector2 targetPoint = CurrentOrigin + ovalOffset;
            Vector2 targetVelocity = targetPoint - Projectile.Center;
            Projectile.velocity = targetVelocity;
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.scale = 0.75f;
            Projectile.scale *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        private Color GetTrailColor(float completionRatio)
        {
            return Color.Black * EasingFunction.QuadraticBump(completionRatio);
        }

        private float GetTrailWidth(float completionRatio)
        {
            float outScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
            return EasingFunction.QuadraticBump(completionRatio) * 10 * outScale;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderFlyTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Texture2D flyTexture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = Projectile.Frame();
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(flyTexture, drawCenter, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }

        private void RenderFlyTrail(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserAlphaShader.Instance;
            shader.BlendState = BlendState.AlphaBlend;
            shader.LaserTexture = TrailRegistry.LightningTrail2Outline;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader);
        }

    }
}

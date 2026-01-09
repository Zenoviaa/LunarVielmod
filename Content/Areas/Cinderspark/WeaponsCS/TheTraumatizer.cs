using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class TheTraumatizer : BaseGun
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 80;
            Item.height = 38;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<TraumatizingRay>();
            Item.scale = 1f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.noUseGraphic = true;
        }

        public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TraumatizingRay>(), damage, knockback, player.whoAmI, 1);
            return false;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
    public class TraumatizingRay : ModProjectile
    {
        private bool _setRotation;
        private float _radians;
        private float _targetRadians;
        private float BeamLength;

        public ref float Timer => ref Projectile.ai[0];
        private ref float Dir => ref Projectile.ai[1];

        private Player Owner => Main.player[Projectile.owner];

        public const float LaserLength = 2400f;
        private Vector2 EndPoint => Projectile.Center + Projectile.velocity * BeamLength;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Timer++;
            if (!_setRotation)
            {
                _radians = MathHelper.ToRadians(32);
                if (Dir == 1)
                    _radians = -_radians;
                _targetRadians = -_radians;
                _setRotation = true;
            }
            if (Timer % 5 == 0)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Lerp(Color.White, Color.Red, Main.rand.NextFloat(0.5f, 1f)),
                    outerColor = Color.DarkRed
                };
                DustParticle.Spawn(EndPoint, -Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(4f, 15f), spawnParams);
            }

            float progress = Timer / 60f;
            float easedProgress = Easing.InOutCubic(progress);
            float rads = MathHelper.Lerp(_radians, _targetRadians, easedProgress);
            if (Projectile.spriteDirection == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(rads);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(rads);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            Projectile.Center = Owner.MountedCenter + Projectile.velocity * 1f;
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile, 800);
            BeamLength = targetBeamLength;
            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Projectile.scale = MathF.Sin(Timer / 100 * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            if (Projectile.timeLeft < 50)
            {
                Projectile.scale = (float)Projectile.timeLeft / (float)50;
            }
            else
            {
                Projectile.scale = MathF.Sin(Timer / 100 * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 1.5f);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            FXUtil.GlowCircleBoom(target.Center,
               innerColor: Color.White,
               glowColor: Color.Red,
               outerGlowColor: Color.DarkRed, duration: 25, baseSize: 0.06f);

            ShakeModSystem.Shake = 1;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Red, 0.5f).noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * 0.5f;
        }

        public override bool ShouldUpdatePosition() => false;

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Red, Color.DarkRed, 0.65f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedImpact);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRedLaser);
            return false;
        }


        private void DrawPixelatedImpact(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
            Vector2 drawCenter = EndPoint - screenPos;
            Vector2 scale = Vector2.One * ExtraMath.Osc(0.2f, 0.4f, speed: 16);
            Vector2 drawOrigin = impactTexture.Size() / 2f;

            Color drawColor = Color.Red;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 1.2f, SpriteEffects.None, 0);

            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 0.8f, SpriteEffects.None, 0);
        }
        private void DrawPixelatedRedLaser(GraphicsDevice graphicsDevice)
        {
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, i / 8f));
            }
            points.Add(Projectile.Center + Projectile.velocity * BeamLength);
            points.Add(Projectile.Center + Projectile.velocity * BeamLength);

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Red;
            shader.OuterColor = Color.DarkRed;
            TrailDrawer.Draw(Main.spriteBatch, points.ToArray(), ColorFunction, WidthFunction, shader);
        }

        public override bool? CanDamage() => true;
    }
}

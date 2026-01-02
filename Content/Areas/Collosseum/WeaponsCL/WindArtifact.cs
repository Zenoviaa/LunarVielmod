using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class WindArtifact : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.damage = 7; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.useTime = 5; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 5; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 8; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.UseSound = SoundID.Item46; // The sound that this Item plays when used.
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<WindArtifactBlast>();
            Item.shootSpeed = 7; // the speed of the projectile (measured in pixels per frame)
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            position += Main.rand.NextVector2Circular(90, 90);
            
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankStaff>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
    public class WindArtifactBlast : ModProjectile
    {
        private int NumPoints => 32;
        private Vector2[] WindPoints;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Scale => ref Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            WindPoints = new Vector2[NumPoints];
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(WindPoints, projHitbox, targetHitbox, 32);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1 && this.OwnedByLocalClient())
            {
                Scale = Main.rand.NextFloat(0.5f, 1f);
                Projectile.netUpdate = true;
            }
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 50;

            float progress = Timer / 60f;
            float easeOut = EasingFunction.InOutSine(progress);
            start = Vector2.Lerp(start, end, easeOut * 0.5f);
            for (int i = 0; i < NumPoints; i++)
            {
                float f = (float)i;
                float ratio = f / (float)NumPoints;
                Vector2 point = Vector2.Lerp(start, end, ratio);
                WindPoints[i] = point;
            }
         
            //Projectile.velocity *= 0.98f;
        }
        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedWindTrail, DrawLayer.OverNPCs);
            return false;
        }

        private void DrawPixelatedWindTrail(GraphicsDevice graphicsDevice)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            TrailDrawer.Draw(Main.spriteBatch, WindPoints, null, StripColors, StripWidth, shader);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color stripColor = Color.Lerp(Color.Transparent, Color.LightGray, EasingFunction.QuadraticBump(progressOnStrip));
            float alpha = (float)Projectile.timeLeft / 60f;
            float easedAlpha = EasingFunction.QuadraticBump(alpha);
            Color finalColor = stripColor * easedAlpha;
            return finalColor;
        }


        private float StripWidth(float progressOnStrip)
        {
            float maxWidth = 12;
            float width = MathHelper.Lerp(0, maxWidth, EasingFunction.QuadraticBump(progressOnStrip));
            float outScale = Timer / 60f;
            float outEasedScale = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(outScale));
            return width * outEasedScale * Scale;
        }
    }
}
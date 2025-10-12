using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Test;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class TestFireball : ScarletProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            TrailCacheLength = 128; 
        }
        public override void AI()
        {
            base.AI();
            Timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var shader = GlowingTrailShader.Instance;

            shader.OuterColor = Color.LightBlue ;
            shader.InnerColor = Color.Red;
            shader.Distortion = 1;
          //  shader.NoiseTexture = TrailRegistry.CrystalTrail;
            shader.BlendState = BlendState.AlphaBlend;

            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, new float[OldCenterPos.Length], TrailColorFunction, TrailWidthFunction, shader);
            shader.BlendState = BlendState.Additive;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, new float[OldCenterPos.Length], TrailColorFunction, TrailWidthFunction, shader);
            // TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, new float[OldCenterPos.Length], TrailColorFunction, TrailWidthFunction, flamingTrailShader);
            return false;
        }

        private float TrailWidthFunction(float arg)
        {
            return MathHelper.Lerp(32, 8, EasingFunction.InOutSine(arg));
        }

        private Color TrailColorFunction(float arg)
        {
            return Color.Lerp(Color.White, Color.Lerp(Color.Blue, Color.Transparent, arg), arg);
        }
    }

    public class Test : ModItem
    {
        private int _dir = 1;
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 19;
            Item.knockBack = 0;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<ExampleMotionBlurProjectile>();
            Item.shootSpeed = 5;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        private void CoolAssFire(Vector2 position, Vector2 velocity)
        {
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity, 
                    innerColor: Color.Red, 
                    outerColor: Color.Orange, 
                    fadeToColor: Color.Purple, 
                    distortOut: true);
         
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {
                   
                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;
                }
            }

        }
        private void CoolAssFire2(Vector2 position, Vector2 velocity)
        {
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Yellow,
                    outerColor: Color.Green,
                    fadeToColor: Color.Pink,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 2;
                }
            }

        }
        private void CoolAssFire4(Vector2 position, Vector2 velocity)
        {
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Cyan * 0.4f,
                    outerColor: Color.Blue * 0.4f,
                    fadeToColor: Color.Purple * 0.2f,
                    distortOut: true);


            }

        }
        private void CoolAssFire3(Vector2 position, Vector2 velocity)
        {
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Green,
                    outerColor: Color.Blue,
                    fadeToColor: Color.AliceBlue,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
            }

        }

        private void GlowDonut(Vector2 position, Vector2 velocity)
        {
            var frag = Particle.NewParticle<GlowDonutParticle>(position, velocity);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            GlowDonut(position, velocity);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TestFireball>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles;
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
            TrailCacheLength = 48;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 8 == 0)
            {
                Particle.NewParticle<ConstellationParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
      
                var p = FXUtil.GlowStretch(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity * 2);
                p.Scale *= 0.25f;

            }
            if(Timer % 1 == 0)
            {

                SpawnParticle();
            }
            if (Timer % 8 == 0)
            {
                var starP = Particle.NewParticle<StarParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
                starP.fast = true;
            }
            if (Timer % 16 == 0)
            {
                int dustType = Main.rand.NextBool(2) ? ModContent.DustType<GlowDust>() : ModContent.DustType<GlyphDust>();
                var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64, 64), dustType, Velocity: -Projectile.velocity, newColor: Color.Pink, Scale: Main.rand.NextFloat(0.5f ,1f));
            }

            Projectile.velocity.Y += 0.125f;
            Projectile.velocity.X *= 1.01f;
 
        }
        private void SpawnParticle()
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            var p2 = FXUtil.GlowStretch(pos, -Projectile.velocity * 1.2f);
            p2.VectorScale.Y *= Main.rand.NextFloat(2f, 8);
            p2.Scale *= 0.5f;
            p2.InnerColor = Color.Lerp(Color.Pink, Color.Goldenrod, Main.rand.NextFloat(0f, 1f));
            p2.OuterGlowColor = Color.Lerp(Color.Blue, Color.DarkViolet, Main.rand.NextFloat(0f, 1f));
            p2.GlowColor = Color.Blue;
            p2.color *= 0.25f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
     
            var shader = StarryMagicShader.Instance;


            shader.GlowColor = Color.Goldenrod;
            shader.GlowColor2 = Color.Blue;
            shader.Tiling = new Vector2(8, 1);
            
            
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, TrailColorFunction2, TrailWidthFunction, shader);
    //    TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, TrailColorFunction, TrailWidthFunction2, shader);

            Texture2D glowStarTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SoftGlow").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = glowStarTexture.Size() / 2f;
            Color glowColor = Color.White;
     
            var shader2 = StarryGlowShader.Instance;
            shader2.GlowColor = Color.Violet;
            shader2.Apply();

            spriteBatch.Restart(effect: shader2.Effect, blendState: BlendState.Additive);
            for(float f = 0; f < 4; f++)
                spriteBatch.Draw(glowStarTexture, drawPos, null, glowColor, 0, drawOrigin, 0.5f + Main.rand.NextFloat(0.8f, 1.2f), SpriteEffects.None, 0);
   
            spriteBatch.RestartDefaults();
            return false;
        }

        private float TrailWidthFunction(float arg)
        {
            return MathHelper.Lerp(64, 48, EasingFunction.InOutSine(arg));
        }
        private float TrailWidthFunction2(float arg)
        {
            return TrailWidthFunction(arg) * 0.5f;
        }


        private Color TrailColorFunction(float arg)
        {
            Color trailColor = Color.Lerp(Color.White, Color.BlueViolet,arg);
        //    trailColor *= MathHelper.SmoothStep(1.0f, 0.0f, arg);
            return trailColor;
        }
        private Color TrailColorFunction2(float arg)
        {
            Color trailColor = Color.Lerp(Color.DarkBlue, Color.BlueViolet, EasingFunction.QuadraticBump(arg));
            trailColor *= MathHelper.SmoothStep(1.0f, 0.0f, arg);
            return trailColor * 0.5f;
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
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TestFireball>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.GunSystem;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{

    public class Dunderbustion : BaseGun
    {
        public override void SetDefaults()
        {
            remainingAmmo = 32;
            maxAmmo = 32;
            reloadWindow = 30;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DunderShot>();
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
    public class DunderShot : ScarletProjectile,
        IDrawPixelated
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private bool Ricochet
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

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.width = 8;
            Projectile.height = 8;
        }

        public override void AI()
        {
            base.AI();
            Timer++;

            //Add some inaccuracy to the gun, it's a machine gun after all!
            if (Timer == 1 && this.OwnedByLocalClient())
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(0.2f);
                Projectile.netUpdate = true;
            }

            if (Ricochet)
            {
                Projectile.velocity.Y += 0.5f;
            }
            if (Timer % 32 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Sparkle>(), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            if (Ricochet)
            {
                Projectile.velocity *= 1.01f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            //Ricochet once, gaining damage and speed

            if (!Ricochet)
            {
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = oldVelocity.X;
                }

                var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.innerColor = Color.Yellow;
                part.outerColor = Color.DarkGoldenrod;
                part.fadeToColor = Color.DarkBlue;
                part.Scale *= 0.5f;
                part.shrink = true;
                part.noStretch = true;
                Ricochet = true;

                SoundStyle hitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Magic.AutomationCast1 : AssetRegistry.Sounds.Magic.AutomationCast2;
                hitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hitSound, Projectile.Center);
                return false;
            }
            else
            {
                return true;
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.DarkGoldenrod, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(1, 0, completionRatio);
        }

        public void DrawPixelated()
        {
            Texture2D starTrailTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/CartoonyStar").Value;

            Vector2 startDrawOrigin = starTrailTexture.Size() / 2f;
            for (int i = 0; i < TrailCacheLength; i++)
            {
                float completionRatio = (float)i / (float)TrailCacheLength;
                Vector2 oldDrawCenter = OldCenterPos[i] - Main.screenPosition;
                SpriteBatch spriteBatch = Main.spriteBatch;
                Color drawColor = Color.Lerp(Color.Goldenrod, Color.Red, completionRatio);
                drawColor.A = 0;
                float drawScale = MathHelper.SmoothStep(1f, 0f, completionRatio);
                drawScale *= 0.3f;
                spriteBatch.Draw(starTrailTexture, oldDrawCenter, null, drawColor, Projectile.rotation, startDrawOrigin, drawScale, SpriteEffects.None, 0);
            }
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 2;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Goldenrod);
            }
        }
    }
}

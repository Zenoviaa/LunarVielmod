using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class BloodySpew : BaseTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<DreadVomit>();
            Item.shootSpeed = 10f;
            Item.mana = 10;
            Item.damage = 12;
        }

        public override Color GetTomeHintColor()
        {
            return Color.Red;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Item.mana = 8;
            Item.useAnimation = 8;
            Item.useTime = 8;
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
    }

    public class DreadVomit : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Main.rand.NextFloat(-0.2f, 0.2f);
            Projectile.spriteDirection = Projectile.direction;
            Timer++;
            if (Timer == 1)
            {


                int Sound = Main.rand.Next(1, 4);
                SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit2");
                if (Sound == 1)
                {

                }
                if (Sound == 2)
                {
                    shootSound = new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit3");

                }
                if (Sound == 3)
                {
                    shootSound = new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit1");
                }

                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, Projectile.position);
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
            for(int i =0; i < 2; i++)
            {
                Vector2 velocity = -Vector2.UnitY;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(4f, 10f);
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, velocity, Color.Red, Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Lerp(Color.Red, Color.Black, Main.rand.NextFloat(0f, 1f));
            }
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 6;
            return MathHelper.Lerp(width, 0, EasingFunction.InOutExpo(completionRatio));

        }
        private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkBlue;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, Projectile.oldRot, ColorFunction, WidthFunction, flamingTrailShader, offset: Projectile.Size / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawCenter, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.PaleVioletRed.ToVector3() * 0.15f * Main.essScale);
        }
    }

}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Projectiles.Bow;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class WintersStom : AbstractMagicTome
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<WinterStormProg>();
            Item.shootSpeed = 11;
            Item.mana = 12;
            Item.damage = 8;
        }

        public override Color GetTomeHintColor()
        {
            return Color.White;
        }
    }
    public class WinterStormProg : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winter Storm");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }


        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 180;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer >= 10)
            {
                Projectile.tileCollide = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.0f;
            Projectile.velocity.Y += 0.2f;
            if (Main.rand.NextBool(7))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].noGravity = false;
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            var source = Projectile.GetSource_Death();
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = -Projectile.oldVelocity;
                    vel = vel.RotatedByRandom(0.8f) * 0.6f;

                    Projectile.NewProjectile(source, Projectile.Center, vel,
                        ModContent.ProjectileType<WinterStormFragProg>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = new Vector2();
                    vel.X = Main.rand.Next(-2, 2);
                    vel.Y = Main.rand.Next(-2, 2);
                    Projectile.NewProjectile(source, Projectile.Center, vel,
                        ModContent.ProjectileType<WinterboundArrowFlake>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);
            float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            return MathHelper.SmoothStep(16f, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            PixelationManager.QueuePrimitivesDrawAction(DrawIceTrail, DrawLayer.OverNPCs);
            return false;
        }

        private void DrawIceTrail(GraphicsDevice graphicsDevice)
        {

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(Color.Gray, Color.Blue, 0.75f);
            shader.OuterColor = Color.Cyan;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
    }
    public class WinterStormFragProg : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winter Storm");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.75f * Main.essScale);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 700;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }

            Projectile.velocity.Y -= 0.01f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm2"), Projectile.position);

            for (float n = 0; n < 2; n++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(4f);
                velocity *= Main.rand.NextFloat(2, 5);
                FlakeParticle fp = FlakeParticle.Spawn(Projectile.Center, velocity);
                fp.gravity = 0f;
                fp.Scale *= 0.4f;
            }
            for (float n = 0; n < 2; n++)
            {
                Vector2 vel = -Projectile.oldVelocity;
                vel = vel.RotatedByRandom(0.8f);
                vel *= Main.rand.NextFloat(0.5f, 1f);
                DustParticle dp = DustParticle.Spawn(Projectile.Center, vel);
                dp.outerColor = Color.Cyan;
            }

            for (float n = 0; n < 2; n++)
            {
                SmokeParticle sp = Particle<SmokeParticle>.Spawn(Projectile.Center, -Vector2.UnitY.RotatedByRandom(4f) * Main.rand.NextFloat(0.5f, 3f), Color.White, Scale: Main.rand.NextFloat(0.15f, 1.5f));
                sp.initialColor = Color.White * 0.4f;

            }
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);
            float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
            return MathHelper.SmoothStep(16f, 0f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32)) * completionRatio;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            PixelationManager.QueuePrimitivesDrawAction(DrawIceTrail, DrawLayer.OverNPCs);
            return false;
        }

        private void DrawIceTrail(GraphicsDevice graphicsDevice)
        {

            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White * 0.4f;
            shader.InnerColor = Color.Lerp(Color.Gray, Color.Blue, 0.75f) * 0.4f;
            shader.OuterColor = Color.Cyan * 0.4f;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar.Projectiles
{
    public class FlamePillar : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private float _scale;
        private Vector2 InitialVelocity;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.light = 0.6f;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                _scale = 0.01f;
            }

            _scale *= 1.01f;
            _scale = MathHelper.Clamp(_scale, 0f, 1f);
            if (Timer < 60)
            {
                Projectile.velocity *= 0.25f;
            }

            if (Timer >= 60)
            {
                Projectile.hostile = true;
            }

            if (Timer == 60)
            {

                //Dust Particles
                for (int k = 0; k < 4; k++)
                {
                    Vector2 newVelocity = InitialVelocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Projectile.Bottom, 0, 0, DustID.Torch, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }

                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
            }

            if (Timer > 60)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, InitialVelocity, 0.1f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        private void Visuals()
        {
            if (!Main.rand.NextBool(3))
                return;

            float radius = 1 / 6f;
            for (int i = 0; i < 1; i++)
            {
                float speedX = Main.rand.NextFloat(-radius, radius);
                float speedY = Main.rand.NextFloat(-radius, radius);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(8, 8);
                Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 2.5f);
                vel = vel.RotatedByRandom(0.6f);
                var d = Dust.NewDustPerfect(pos, DustID.InfernoFork, vel);
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed, Scale: 3f);
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            
            return MathHelper.SmoothStep(12, 0, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, completionRatio) * MathHelper.Lerp(0.6f, 0f, completionRatio);
        }


        private void DrawFlameTrail(GraphicsDevice gDevice)
        {
            var laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
            laserShader.OuterColor = Color.Red;
            laserShader.InnerColor = Color.Yellow;
            laserShader.LaserColor = Color.LightGoldenrodYellow;
            TrailDrawer.Draw(Projectile.oldPos, ColorFunction, WidthFunction, laserShader, Projectile.Size * 0.5f);
        }
        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail);
            //FixedRichLaserShader laserShader = ShaderContent.getin
            SpritebatchDrawer flameDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            Main.spriteBatch.Draw(flameDrawer);

            for(float f = 0; f < MathHelper.TwoPi; f+= MathHelper.ToRadians(90))
            {
                SpritebatchDrawer glowDrawer = flameDrawer;
                glowDrawer.color = Color.Red * 0.3f;
                glowDrawer.color.A = 0;
                glowDrawer.worldPosition += (f+Main.GlobalTimeWrappedHourly).ToRotationVector2() * 4;
                Main.spriteBatch.Draw(glowDrawer);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                Color glowColor = new Color(85, 45, 15) * 0.5f;
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor,
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale * VectorHelper.Osc(0.75f, 1f, speed: 32, offset: Projectile.whoAmI), SpriteEffects.None, 0f);
            }
        }
    }
}

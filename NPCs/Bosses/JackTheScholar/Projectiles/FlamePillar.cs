using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.JackTheScholar.Projectiles
{
    internal class FlamePillar : ModProjectile
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
                    Dust.NewDust(Projectile.Bottom, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
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
            float radius = 1 / 6f;
            for (int i = 0; i < 1; i++)
            {
                float speedX = Main.rand.NextFloat(-radius, radius);
                float speedY = Main.rand.NextFloat(-radius, radius);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
                Main.dust[d].noGravity = true;
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
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.6f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++)
            {
                float rot = (float)i / 4f;
                Vector2 vel = rot.ToRotationVector2() * VectorHelper.Osc(0f, 4f, speed: 16);
                Vector2 flameDrawPos = drawPos + vel + Main.rand.NextVector2Circular(2, 2);
                flameDrawPos -= Vector2.UnitY * 4;
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return base.PreDraw(ref lightColor);
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

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
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

            if(Timer > 60)
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
            for (int i = 0; i < 2; i++)
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
            return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            for (float f = 0; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 offsetDrawPos = drawPos + rot.ToRotationVector2() * VectorHelper.Osc(1f, 2f, 5);
                Color color = Color.White.MultiplyRGB(lightColor);
                spriteBatch.Draw(texture, offsetDrawPos, null, color, drawRotation, drawOrigin, _scale, SpriteEffects.None, layerDepth: 0);
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}

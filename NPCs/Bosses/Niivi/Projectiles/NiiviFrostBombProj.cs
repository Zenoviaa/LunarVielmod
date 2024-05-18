using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostBombProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float MaxScale => 1f;
        private float LifeTime => 180;
        private string FrostTexture => "Stellamod/Assets/Effects/VoxTexture3";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.98f;
            Projectile.velocity.Y -= 0.05f;
            Projectile.rotation += Projectile.velocity.Length() * 0.04f;
            if(Timer < 90)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                    ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, 1f);
                }
            }
            else
            {
                if (Main.rand.NextBool(20))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                    ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, 0.5f);
                }
            }


            if(Timer == 150)
            {
                Vector2 velocity = Vector2.Zero;
                int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type,
                    0, 0, Main.myPlayer);
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NiiviFrostBlastProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(FrostTexture).Value;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float progress = Timer / LifeTime;
            float easedProgress = Easing.InCubic(1 - progress);
            float scale = easedProgress * MaxScale;
            scale += 0.05f;
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor = drawColor * Easing.InCubic(progress);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 drawPosition = oldPos - Main.screenPosition;
                float alpha = (float)i / (float)Projectile.oldPos.Length;
                alpha = 1 - alpha;
                Color oldDrawColor = drawColor * alpha;

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Draw(texture, drawPosition, null, oldDrawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }


    }
}

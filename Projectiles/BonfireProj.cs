using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Catacombs;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class BonfireProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
               new Vector3(255, 0, 68),
               new Vector3(252, 191, 84),
               new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, new Color(255, 0, 68), lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(255, 0, 68), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private ref float ai_Size => ref Projectile.ai[0];
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.GetModPlayer<BonfirePlayer>().hasBonfire)
            {
                Projectile.Kill();
                return;
            }

            float maxAiSize = 120;
            float aiSizeMult = ai_Size / maxAiSize;
            float offset = aiSizeMult * 16;

            Projectile.timeLeft = 2;
            Projectile.Center = owner.Center + new Vector2(0, -80 + VectorHelper.Osc(0, 8 + offset, 2));
            if (owner.velocity == Vector2.Zero)
            {
                ai_Size++;
                if (ai_Size >= maxAiSize)
                    ai_Size = maxAiSize;
            }
            else
            {
                ai_Size--;
                if (ai_Size <= 0)
                    ai_Size = 0;
            }
         
     
            float targetScale = 1.25f;
            float targetLightSize = 7;
            float lightSize = aiSizeMult * targetLightSize;
            lightSize = MathHelper.Clamp(lightSize, 1f, targetLightSize);


            Lighting.AddLight(Projectile.Center, new Vector3(lightSize, lightSize, lightSize));
            Projectile.scale = aiSizeMult * targetScale;
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 1f, targetScale);
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(8))
            {
                Vector2 randOffset = Main.rand.NextVector2Circular(4, 4);
                ParticleManager.NewParticle(Projectile.Center + randOffset, Vector2.Zero, 
                    ParticleManager.NewInstance<UnderworldParticle1>(), default(Color), 0.5f);
            }

            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }
    }
}

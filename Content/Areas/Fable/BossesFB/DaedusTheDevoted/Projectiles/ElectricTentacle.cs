using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class ElectricTentacle : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackTimer => ref Projectile.ai[1];
        private ref float RotationTime => ref Projectile.ai[2];
        public CoreLightning Lightning { get; set; } = new CoreLightning();
        public override string Texture => TextureRegistry.EmptyBigTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 128;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {

                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Main.rand.NextVector2Circular(8, 8));
                }
            }

            if (Timer % 3 == 0)
            {
                Lightning.RandomPositions(Projectile.oldPos);
            }

            if(RotationTime == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    RotationTime = Main.rand.NextFloat(30, 60);
                    Projectile.netUpdate = true;
                }
            }

            if(Timer >= RotationTime)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
                Timer = 0;
            }

            Lightning.WidthMultiplier = 2;
            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = MathHelper.Lerp(8, 2, progress);
                trail.LightningRandomExpand = MathHelper.Lerp(16, 4, progress);
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Yellow, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Yellow, progress);
            }


            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Main.rand.NextBool(5000))
                {
                    Vector2 prevPoint = Projectile.oldPos[i - 1];
                    Vector2 currentPoint = Projectile.oldPos[i];
                    Vector2 vel = currentPoint - prevPoint;
                    Dust.NewDustPerfect(prevPoint, DustID.GoldCoin, vel, Scale: 0.5f);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(Projectile.oldPos, projHitbox, targetHitbox, lineWidth: 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 prevPoint = Projectile.oldPos[i - 1];
                    Vector2 currentPoint = Projectile.oldPos[i];
                    Vector2 vel = currentPoint - prevPoint;
                    Dust.NewDustPerfect(prevPoint, DustID.GoldCoin, vel, Scale: 1);
                }

            }
        }
    }
}

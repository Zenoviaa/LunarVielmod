using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class ElectricSingularityBolt : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override string Texture => TextureRegistry.EmptyBigTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Main.rand.NextVector2Circular(8, 8));
                }
            }

            if (Timer % 3 == 0)
            {
                Lightning.RandomPositions(Projectile.oldPos);
            }

            Lightning.WidthMultiplier = 2;
            Lightning.SetBoltDefaults();

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
            Lightning.WidthMultiplier = 1f;
            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                var trail = Lightning.Trails[i];
                trail.PrimaryColor = Color.Yellow;
                trail.NoiseColor = Color.DarkGoldenrod;
            }
            Lightning.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot);


            Lightning.WidthMultiplier = 0.5f;
            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                var trail = Lightning.Trails[i];
                trail.PrimaryColor = Color.Black;
                trail.NoiseColor = Color.Black;
            }
            Lightning.DrawAlpha(spriteBatch, Projectile.oldPos, Projectile.oldRot);
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
                    Vector2 vel = prevPoint - currentPoint;
                    vel *= 0.3f;
                    Dust.NewDustPerfect(prevPoint, DustID.GoldCoin, vel, Scale: 1);
                }

            }
        }
    }
}

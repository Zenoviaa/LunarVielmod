using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Stellamod.Content.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarMissileBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 8; f++)
                {
                    Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = Projectile.Center,
                        innerColor = Color.White.ToVector4(),
                        outerColor = Color.Blue.ToVector4(),
                        velocity = Main.rand.NextVector2Circular(7, 7)
                    });
                }

                for (float f = 0; f < 8; f++)
                {
                    Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with {
                        position = Projectile.Center, 
                        innerColor = Color.Pink.ToVector4(), 
                        outerColor = Color.Purple.ToVector4(),
                        velocity = Main.rand.NextVector2Circular(12, 12)});
                }

                for (float f = 0; f < 8; f++)
                {
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                    {
                        position = Projectile.Center,
                        innerColor = Color.LightBlue.ToVector4(),
                        outerColor = Color.DarkBlue.ToVector4(),
                        velocity = Main.rand.NextVector2Circular(17, 17)
                    });
                }

                var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Pink, Color.Purple);
                fx.Scale *= 2;
                FXUtil.ShakeCamera(Projectile.position, 1024, 9);
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/STARGROP");
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);
            }
        }
    }
}

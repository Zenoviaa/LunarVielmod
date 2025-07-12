using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles
{
    internal class JiitasGunShot : ModProjectile
    {
        public override string Texture => AssetHelper.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 4;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                //Spawn effects
                SoundStyle shotSound = AssetRegistry.Sounds.Jiitas.JiitasGunShot;
                shotSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(shotSound, Projectile.position);

                //IMPACT EFFECT
                FXUtil.ShakeCamera(Projectile.position, 1024, 2);
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.03f);

                /*
                if(Main.rand.NextBool(8))
                    SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);*/
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.025f, 0.05f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }
}

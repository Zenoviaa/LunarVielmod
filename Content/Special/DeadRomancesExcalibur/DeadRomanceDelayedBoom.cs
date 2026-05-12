using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceDelayedBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.Red);
            donut.Scale *= 1f;
            donut.fadeToColor = Color.Goldenrod;
            donut.noStretch = true;
            donut.shrink = true;
        }
        if (Timer == 30)
        {
            PixelPrimitiveCircleFactory.CreateHeavenlyBoom(Projectile.Center);
            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel, Scale: 0.5f);
                sp.flickering = true;
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.outerColor = Color.Gold;
                sp.fast = true;
            }
            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel, Scale: 0.5f);
                sp.outerColor = Color.Gold;
            }


            SoundStyle sound = AssetRegistry.Sounds.Melee.ExcaliburHeavenlyExplosions;
            sound.PitchVariance = 0.5f;
        
            SoundEngine.PlaySound(sound, Projectile.position);
            ShakeScreenPosition.Shake = 2;
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
            boom.Scale *= 2;
            boom.OuterGlowColor *= 0.6f;
            boom.GlowColor *= 0.6f;
            boom.InnerColor *= 0.6f;
        }
        if (Timer >= 30)
            Projectile.friendly = true;
    }
}

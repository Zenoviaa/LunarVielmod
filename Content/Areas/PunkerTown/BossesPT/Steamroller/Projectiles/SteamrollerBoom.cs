using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

public class SteamrollerBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private bool Bigger => Projectile.ai[1] == 1;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 6;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle deathSound = AssetRegistry.Sounds.Melee.HammerSmash2;
            deathSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(deathSound, Projectile.position);
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
                ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
                if (Main.rand.NextBool(1))
                {
                    spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
                    Point point = Projectile.Center.ToTileCoordinates();
                    while (!WorldGen.SolidTile(point))
                        point.Y++;

                    int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
                    Dust dust = Main.dust[d];
                    dust.position += Main.rand.NextVector2Circular(32, 32);
                    dust.velocity = spawnVelocity;
                    dust.noLightEmittence = true;
                }
                spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Red, Color.DarkRed);
            if (Bigger)
                fx.Scale *= 1.75f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            for (int i = 0; i < 9; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var dp = DustParticle.Spawn(Projectile.Center, vel);
                dp.fast = true;
                dp.Scale *= 0.4f;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 4; f++)
            {
                Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8f), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

    }
}

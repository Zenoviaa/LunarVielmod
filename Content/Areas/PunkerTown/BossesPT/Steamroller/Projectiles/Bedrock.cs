using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

public class Bedrock : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.scale = Main.rand.NextFloat(0.7f, 1f);
        }
        if (Timer > 15)
            Projectile.tileCollide = true;
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation -= 0.05f;
        Projectile.rotation -= Projectile.velocity.Length() * 0.015f;
    }


    public override bool PreDraw(ref Color lightColor)
    {
        this.Outline(Color.Red, ref lightColor);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
        //        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
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
        for (float f = 0; f < 12; f++)
        {
            float lerp = f / 12f;
            float rot = lerp * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2();
            vel *= Main.rand.NextFloat(2, 5);
            Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
        }
    }
}

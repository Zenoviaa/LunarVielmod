using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    public void FaceTarget()
    {
        int dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = dir;
    }

    public bool IsGrounded()
    {
        Point b = NPC.Bottom.ToTileCoordinates();
        b.Y++;
        Tile tile = Main.tile[b];
        if (tile.HasTile && WorldGen.SolidOrSlopedTile(tile))
            return true;
        return false;
    }

    public void StayGrounded()
    {
        NPC.velocity.X *= 0.94f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }
    public void LaughSound()
    {

    }

    public void MakeCapeInEffect()
    {

    }

    public void MakeCapeOutEffect()
    {

    }
    public void TeleportToAnArenaSide()
    {

    }

    public void MakeEyeFlashParticle()
    {

    }

    public void MakeFallingCrashParticles()
    {

    }

    public void MakeJumpingParticles()
    {
        //Probably a bunch of rocks, stars, smoke and stuff
        //We can make a rock particle :P 
    }

    public void GruntSound()
    {

    }

    public void KickSound()
    {

    }

    public void BigGruntSound()
    {

    }

    public void AirSwooshSound()
    {

    }

    public void CrashVFX(Vector2 position, Vector2 velocity)
    {

    }

    public void EarthQuakeVFX(Vector2 position, Vector2 velocity)
    {
        for (float f = 0; f < 28; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-512, 512);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

    }
    public void KickImpactVFX(Vector2 position, Vector2 velocity)
    {
        //Thinking some like circles and dust or something
        FXUtil.ShakeCamera(position, 1024, 32);
        ShakeScreenPosition.Shake = 24;

        //Rock impact sound
        SoundStyle rockHitSound = AssetReferences.Assets.Sounds.STARR.RockSmash.Asset with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(rockHitSound, position);


        for (float f = 0; f < 16; f++)
        {
            Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(0.6f, 2);
            var spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 2f;
            spawnParams.outerColor = Color.DarkBlue;
            DustParticle.Spawn(position + Main.rand.NextVector2Circular(8, 8), vel, spawnParams);
        }

        for(float f = 0; f < 2; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        //Lemme grab the steamroller particles
        for(int i = 0; i < 4; i++)
        {
            MakeSoilParticle(position + Main.rand.NextVector2Circular(32, 32), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f));
        }

        if (Main.netMode == NetmodeID.Server)
            return;
        for (int i = 0; i < 2; i++)
        {
            MakeRockGore(position + Main.rand.NextVector2Circular(32, 32), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f));
        }
        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(position, (-velocity.SafeNormalize(Vector2.Zero) * 4), Color.SkyBlue);
        donut.Scale *= 2;
    }

    public void MakeRockGore(in Vector2 position, in Vector2 velocity)
    {
        Gore.NewGore(NPC.GetSource_FromThis(), position, velocity, ModContent.GoreType<IceRockGore>());
    }

    public static void MakeSoilParticle(in Vector2 position, in Vector2 velocity)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        Vector2 spawnPosition = position;
        Vector2 spawnVelocity = velocity;
        ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
    }

    public void PunchVFX(Vector2 position, Vector2 velocity)
    {
        //Thinking some like circles and dust or something
        FXUtil.ShakeCamera(position, 1024, 8);

        //Rock impact sound
        SoundStyle rockHitSound = AssetReferences.Assets.Sounds.STARR.RockSummon.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(rockHitSound, position);


        for (float f = 0; f < 8; f++)
        {
            Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(0.6f, 2);
            var spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 1f;
            spawnParams.outerColor = Color.DarkBlue;
            DustParticle.Spawn(position + Main.rand.NextVector2Circular(16, 16), vel, spawnParams);
        }

        for (float f = 0; f < 1; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(position, (-velocity.SafeNormalize(Vector2.Zero) * 4), Color.SkyBlue);
        donut.Scale *= 1.2f;
    }

    public void PunchBoulder(int index, Vector2 velocity)
    {
        int projType = ModContent.ProjectileType<STARBOULDER>();
        foreach(var proj in Main.ActiveProjectiles)
        {
            if (proj.type == projType && proj.ai[0] == NPC.whoAmI && proj.ai[1] == index)
            {
                proj.ai[2] = 10 + velocity.ToRotation();
            }
        }
    }

    public bool HasAnotherBoulder()
    {
        int projType = ModContent.ProjectileType<STARBOULDER>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == projType && proj.ai[2] <= -5)
            {
                return true;
            }
        }
        return false;
    }
}

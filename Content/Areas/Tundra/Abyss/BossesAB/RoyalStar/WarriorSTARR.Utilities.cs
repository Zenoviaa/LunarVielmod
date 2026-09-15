using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;

using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private float _stepDistance;
    private void CreateFootsteps()
    {
        float traveledDistance = Vector2.Distance(NPC.position, NPC.oldPosition);
        _stepDistance += traveledDistance;
        if (_stepDistance >= 100)
        {
            Vector2 pos = NPC.Bottom;
            var sp = MoonSpiralParticle.Spawn(NPC.Bottom, Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.color = Color.Gold;
            sp.fast = true;
            _stepDistance = 0;
        }
    }
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

    public void MakeCometParticles(Vector2 position, Vector2 velocity)
    {

        {
            var sp = SmokeParticle.SpawnInAlphaLayer(position + Main.rand.NextVector2Circular(32, 32), -velocity * 0.05f, Color.DarkBlue);
            sp.fast = true;
            sp.initialColor = Color.Purple;
            sp.fadeToColor = Color.DarkGray;
            sp.Scale *= 2.5f;
            sp.behindLayer = true;
        }
        if (Main.rand.NextBool(12))
        {
            var sp = SparkleParticle.Spawn(position + Main.rand.NextVector2Circular(32, 32), -velocity * 0.05f, Color.DarkBlue);
            sp.fast = true;
            sp.behindLayer = true;
            sp.Scale *= 0.5f;
            sp.gravity = 0;
        }
        Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = position + Main.rand.NextVector2Circular(32, 32),
            velocity = Main.rand.NextVector2Circular(8, 8) - velocity,
            innerColor = Color.LightBlue.ToVector4(),
            outerColor = Color.DarkBlue.ToVector4(),
            scale = Vector2.One * 0.6f
        });
    }

    public void MakeFallingCrashParticles()
    {
        if (Timer % 5 == 0 && NPC.velocity.Length() > 3)
        {
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);
            p2.Scale *= 0.5f;
        }
        if (Timer % 5 == 0)
        {
            Vector2 spawnPosition = NPC.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);
            spawnVelocity -= NPC.velocity * 0.2f;
            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        if (Main.rand.NextBool(6))
        {
            Vector2 pos = NPC.Center;
            pos.X += Main.rand.NextFloat(-64, 64);
            pos.Y += Main.rand.NextFloat(-64, 64);
            var sp = SparkleParticle.Spawn(pos, -NPC.velocity * 0.1f);
            sp.gravity = 0f;
            sp.dampening = 0.1f;
            sp.Scale *= 0.6f;
            sp.outerColor = Color.Gold;
            sp.innerColor = Color.LightGoldenrodYellow;
        }

        if (Main.rand.NextBool(8))
        {
            Vector2 pos = NPC.Center;
            pos.X += Main.rand.NextFloat(-64, 64);
            pos.Y += Main.rand.NextFloat(-64, 64);
            FXUtil.GlowStretch(pos, -NPC.velocity.SafeNormalize(Vector2.Zero) * 4);
        }
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

    public void Teleport(Vector2 position)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = position;
        }
    }

    public void CrashVFX(Vector2 position, Vector2 velocity)
    {

    }

    public void StarBoomVFX(Vector2 position)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        PixelPrimitiveCircleFactory.CreateSTARRBoom(position);
    }
    public void CrackVFX(Vector2 position)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, timeLeft = 200 });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, scale = 2f, timeLeft = 120 });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, scale = 3f, timeLeft = 45 });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, scale = 5f, timeLeft = 25 });
    }

    public void StarBitDust()
    {
        if (Main.rand.NextBool(2))
        {
            Vector2 pos = NPC.Center;
            pos.X += Main.rand.NextFloat(-64, 64);
            pos.Y += Main.rand.NextFloat(-64, 64);
            Vector2 vel = Main.rand.NextVector2Circular(8, 8);
            vel -= NPC.velocity;
            Dust.NewDustPerfect(pos, ModContent.DustType<StarBitDust>(), vel);
        }

    }
    public void MakeJumpVFX(Vector2 position, Vector2 velocity)
    {
        FXUtil.ShakeCamera(position, 1024, 4);
        ShakeScreenPosition.Shake = 8;

        int dustType = ModContent.DustType<StarBitDust>();
        for (float f = 0; f < 16; f++)
        {
            Vector2 pos = position;
            pos.X += Main.rand.NextFloat(-64, 64);
            pos.Y += Main.rand.NextFloat(-128, 0);
            Vector2 vel = velocity;
            vel *= Main.rand.NextFloat(0.5f, 1f);
            vel = vel.RotatedByRandom(1.5f);
            Dust.NewDustPerfect(pos, dustType, vel);
        }

        for (float f = 0; f < 5; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-512, 512);
            spawnPosition.Y += Main.rand.NextFloat(-64, 0);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        //Lemme grab the steamroller particles
        for (int i = 0; i < 4; i++)
        {
            FXUtil.MakeSoilParticle(position + Main.rand.NextVector2Circular(48, 32), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f));
        }

        //Rock impact sound
        SoundStyle rockHitSound = AssetReferences.Assets.Sounds.STARR.RockSmash.Asset with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(rockHitSound, position);
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, timeLeft = 200 });
    }

    public void StarBitVFX(Vector2 position, Vector2 velocity)
    {
        int dustType = ModContent.DustType<StarBitDust>();
        for (float f = 0; f < 32; f++)
        {
            Vector2 pos = position;
            pos.X += Main.rand.NextFloat(-64, 64);
            pos.Y += Main.rand.NextFloat(-64, 64);
            Vector2 vel = velocity;
            vel *= Main.rand.NextFloat(0.5f, 1f);
            vel = vel.RotatedByRandom(1.5f);
            Dust.NewDustPerfect(pos, dustType, vel);
        }



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

    public void MakeSparkleAroundVFX(Vector2 position)
    {
        var sp = SparkleParticle.Spawn(position + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(12, 12), Color.DarkBlue);
        sp.fast = true;
        sp.Scale *= 0.5f;
        sp.gravity = 0;
        sp.dampening = 0.6f;
        sp.outerColor = Color.Green;
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

        for (float f = 0; f < 2; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        //Lemme grab the steamroller particles
        for (int i = 0; i < 4; i++)
        {
            FXUtil.MakeSoilParticle(position + Main.rand.NextVector2Circular(32, 32), velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f));
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

    public void IFartedVFX(in Vector2 position, in Vector2 velocity)
    {
        for (float f = 0; f < 27; f++)
        {
            var ts = ThickSmokeParticle.Spawn(position + Main.rand.NextVector2Circular(164, 80), velocity, Color.Green);
            ts.color = Color.Green;
            ts.Scale *= 2.4f;
        }
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
        foreach (var proj in Main.ActiveProjectiles)
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

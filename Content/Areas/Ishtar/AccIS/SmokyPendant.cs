using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.AccIS;

public class SmokyPendant : AbstractMeleeAddon
{
    private float _oldSwingRot;
    private float _traveledSwingRotation;
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        _traveledSwingRotation += MathF.Abs(projectile.Projectile.rotation - _oldSwingRot);
        _oldSwingRot = projectile.Projectile.rotation;
        if (_traveledSwingRotation <= 0.25f)
            return;
        _traveledSwingRotation = 0f;
        int index = (int)(projectile.Interpolant * projectile.swingTrailCache.Length) % projectile.swingTrailCache.Length;
        Vector2 spawnPos = projectile.swingTrailCache[index];
        FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
        sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkOrange, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
        sp.color *= 0.5f;
        sp.fadeToColor = Color.Black * 0.5f;
        sp.Scale *= 0.4f;

        index = (int)(projectile.Interpolant * projectile.swingTrailCache.Length) % projectile.swingTrailCache.Length;
        int nextIndex = index + 4;
        nextIndex %= projectile.swingTrailCache.Length;

        spawnPos = projectile.swingTrailCache[index];
        Vector2 spawnPos2 = projectile.swingTrailCache[nextIndex];
        Vector2 spawnVelocity = spawnPos2 - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 24;

        if (Main.rand.NextBool(2))
        {
            Color color = new Color(41, 43, 66);
            var sp2 = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
            sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.5f;
            sp2.Scale *= 0.25f;
            sp2.fadeToColor = Color.Black * 0.5f;
        }

        foreach(NPC npc in NPCHelper.FindNPCsInRange(projectile.Projectile.Center, 100))
        {
            npc.AddBuff(ModContent.BuffType<PoisonousSmoke>(), 90);
        }
    }
    public override void OnHitNPC(BaseSwingProjectileV2 projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        if (projectile.IsFinishingSwing())
        {
            Projectile.NewProjectile(projectile.Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<PoisonousSmokeBoom>(), projectile.Projectile.damage, projectile.Projectile.knockBack, projectile.Projectile.owner);
        }
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<EreshkinCandle, BlankAccessory>();
    }
}

public class PoisonousSmokeBoom : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.light = 1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 120);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 14; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var smokeParitcle = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                smokeParitcle.dampening = 0.09f;
                smokeParitcle.fadeToColor = Color.Black * 0.5f;
                smokeParitcle.initialColor = Color.DarkRed * 0.5f;
                smokeParitcle.Scale *= 2f;
            }
            for (int i = 0; i < 14; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var smokeParitcle = FaintSmokeParticle.SpawnInAlphaLayer(pos, vel);
                smokeParitcle.dampening = 0.09f;
                smokeParitcle.fadeToColor = Color.Black * 0.5f;
                smokeParitcle.color = Color.DarkRed * 0.5f;
                smokeParitcle.Scale *= 0.9f;
                smokeParitcle.behindLayer = true;
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var dp = DustParticle.Spawn(pos, vel);
                dp.dampening = 0.05f;
                dp.innerColor = Color.OrangeRed;
                dp.fast = true;

            }

            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red, duration: 12, baseSize: 0.24f);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { PitchVariance = 0.6f }, Projectile.position);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }

    public void DrawToRenderTargets()
    {

    }
}

public class PoisonousSmoke : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 64;

    }
}
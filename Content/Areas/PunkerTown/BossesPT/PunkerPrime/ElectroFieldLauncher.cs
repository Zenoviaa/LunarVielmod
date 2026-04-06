using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;

public class ElectroLightningRenderer : PixelPrimitiveRenderer<ElectroLightningRenderer>
{
    public override BaseShader PrepareShader()
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Color.Red;
        shader.OuterColor = Color.DarkRed;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        return shader;
    }

    public override Color GetTrailColor(float completionRatio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Color.Red, osc);
    }

    public override float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(8, 16, completionRatio) * MathF.Sin(Main.GlobalTimeWrappedHourly * 8 + completionRatio * 8) * 0.5f + 0.5f;
    }
}

public class ElectroField : ModProjectile
{
    private Vector2[] _shockPos;
    private Vector2[] _sparkPos;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _shockPos = new Vector2[32];
        _sparkPos = new Vector2[32];
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 369;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer < 60)
        {
            Projectile.velocity *= 0.96f;
        }

        if (Timer % 8 == 0)
        {
            DustParticle sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(12, 12), Color.White, 0.7f);
            sp.fast = true;
            sp.gravity = 0;
            sp.noTileCollide = true;
        }

        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Vector2.Zero, Color.White, 0.3f);
            sp.gravity = 0;
        }

        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float outScale = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        for (int i = 0; i < _shockPos.Length; i++)
        {
            ref Vector2 position = ref _shockPos[i];
            Vector2 offset = new Vector2();

            float radians = i / (float)_shockPos.Length * MathHelper.TwoPi;
            radians += Timer * 0.03f;

            float radius = ExtraMath.Osc(100, 128, speed: 18, offset: Projectile.whoAmI);

            radius *= inScale * outScale;
            offset.X += MathF.Sin(radians) * radius;
            offset.Y += MathF.Cos(radians) * radius;
            offset = Vector2.Lerp(offset, Vector2.Zero, (MathF.Sin(Timer * 0.5f + i) + 0.5f) * 0.1f);
            offset += Main.rand.NextVector2Circular(6, 6);
            position = Projectile.Center + offset;

            _sparkPos[i] = Projectile.Center + offset.RotatedBy(MathHelper.PiOver4) * 0.2f * Main.rand.NextFloat(1f, 1.5f);
        }
        DrawHelper.AnimateTopToBottom(Projectile, 2);
    }

    private void DrawBloom(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D bloomTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowScale = Vector2.One * 0.25f;
        float rotation = Main.GlobalTimeWrappedHourly * 4;
        float outScale = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Color.Red;
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.1f;
        bloomDrawer.color *= outScale;
        spriteBatch.Draw(bloomDrawer);
        for (int i = 0; i < _shockPos.Length; i += 2)
        {
            Vector2 pos = _shockPos[i];

            Color glowColor = Color.Lerp(Color.White, Color.Red, 0.6f);
            glowColor.A = 0;
            glowColor *= 0.2f;
            glowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6, offset: i);
            glowColor *= outScale;

            bloomDrawer.worldPosition = pos;
            bloomDrawer.color = glowColor;
            bloomDrawer.scale = glowScale;
            spriteBatch.Draw(bloomDrawer);
        }

        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);

        Color spiralGlowColor = Color.Lerp(Color.White, Color.Red, 0.6f);
        spiralGlowColor.A = 0;
        spiralGlowColor *= 0.2f;
        spiralGlowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6);
        spiralGlowColor *= outScale;
        spiralDrawer.color = spiralGlowColor;
        spriteBatch.Draw(spiralDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
        ElectroLightningRenderer.Queue(_shockPos);
        ElectroLightningRenderer.Queue(_sparkPos);
        return false;
    }
}

public class ElectroFieldLauncher : PunkerPrimeArm
{
    private enum AIState
    {
        Idle,
        Shoot_Start,
        Shoot
    }

    private AIState State
    {
        get => (AIState)NPC.ai[3];
        set => NPC.ai[3] = (float)value;
    }

    private int ElectroSphereDamage => 28;
    private float BaseAngle => -75;
    public override void ArmAI()

    {
        base.ArmAI();
        SetRootToParentCenter();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Shoot_Start:
                AI_ShootStart();
                break;
            case AIState.Shoot:
                AI_Shoot();
                break;
        }
    }


    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            Timer = 0;
            NPC.netUpdate = true;
        }
    }
    private void SetAngles(float baseAngle)
    {
        float osc = MathF.Sin(Timer * 0.02f) * 0.5f + 0.5f;

        Segments[0].angle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
        Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(75);
        Segments[2].angle = Segments[1].angle;
        Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(80);
    }
    private void AI_Idle()
    {
        Timer++;
        isAttacking = false;
        heldLightningScale *= 0.9f;
        telegraphLineColor *= 0.2f;

        TargetOutlineColor = Color.Transparent;
        AimGunTowardTarget();
        SetAngles(BaseAngle);
        if (DoAttack)
        {
            DoAttack = false;
            SwitchState(AIState.Shoot_Start);
        }
    }

    private void SpawnSteamParticle()
    {
        Vector2 spawnPosition = NPC.Top;
        spawnPosition.X += Main.rand.NextFloat(-64, 64);

        Vector2 spawnVelocity = Vector2.Zero;
        spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

        float spawnScale = Main.rand.NextFloat(0.75f, 1f);
        var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
    }

    private void AI_ShootStart()
    {
        isAttacking = true;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }
        if (Timer == 1)
        {
            NPC.TargetClosest();
            SoundStyle revSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
            revSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(revSound, NPC.position);
            CreateMuzzleFlash();
        }

        if (Timer % 5 == 0)
        {
            SpawnSteamParticle();
        }

        TargetOutlineColor = Color.Yellow;

        AimGunTowardTarget();
        float revTime = 100;
        float completionRatio = Timer / revTime;
        telegraphLineColor = Color.Lerp(Color.Transparent, Color.Red, completionRatio);
        heldLightningScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
        SetAngles(MathHelper.Lerp(BaseAngle, BaseAngle - 90, EasingFunction.OutExpo(completionRatio)));

        Vector2 targetFireVelocity = (Target.Center - NPC.Center);
        float targetRotation = targetFireVelocity.ToRotation();
        NPC.rotation = targetRotation;

        if (Timer >= 60f)
        {
            SwitchState(AIState.Shoot);
        }
    }

    private void AI_Shoot()
    {
        isAttacking = true;
        Timer++;
        telegraphLineColor *= 0.2f;
        if (Timer % 10 == 0)
        {
            SpawnSteamParticle();
        }

        if (Timer % 5 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FireworkFountain_Red);
        }

        if (Timer % 10 == 0)
        {
            var spawnPos = NPC.Center;
            spawnPos += Main.rand.NextVector2Circular(8, 8);
            var p = LegacyParticle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
        }

        NPC.velocity *= 0.1f;

        int fireTime = 25;
        int fireCount = 2;

        AimGunTowardTarget();
        float fullFireTime = (fireTime * fireCount);
        float completionRatio = Timer / fullFireTime;
        SetAngles(MathHelper.Lerp(BaseAngle - 90, BaseAngle, completionRatio));
        telegraphLineColor = Color.Red;
        Vector2 targetFireVelocity = (Target.Center - NPC.Center);
        float targetRotation = targetFireVelocity.ToRotation();
        NPC.rotation = targetRotation;

        if (Timer % fireTime == 0)
        {
            SoundStyle mechShoot = AssetRegistry.Sounds.SteamPunking.MechShoot1;
            mechShoot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(mechShoot, NPC.position);

            CreateMuzzleFlash();
            if (MultiplayerHelper.IsHost)
            {
                Vector2 fireVelocity = NPC.rotation.ToRotationVector2();
                fireVelocity *= 21;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                    ModContent.ProjectileType<ElectroField>(), ElectroSphereDamage, 1, Main.myPlayer);
            }
            float numDust = 8;
            for (float f = 0; f < numDust; f++)
            {
                Vector2 dustVelocity = NPC.rotation.ToRotationVector2();
                dustVelocity *= Main.rand.NextFloat(1f, 10f);
                dustVelocity = dustVelocity.RotatedByRandom(0.5f);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            var stretchParticle = FXUtil.GlowStretch(NPC.Center, NPC.rotation.ToRotationVector2() * 5f);
            stretchParticle.InnerColor = Color.Red;
            stretchParticle.GlowColor = Color.Violet;
        }

        if (Timer >= fullFireTime)
        {
            SwitchState(AIState.Idle);
        }
    }
}

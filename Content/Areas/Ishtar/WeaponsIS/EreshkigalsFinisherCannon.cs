using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS;

public class EreshkigalCrack : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 196;
        Projectile.height = 196;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        if(Style == 1)
        {
            Projectile.extraUpdates = 3;
        }
        Timer++;
        if (Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Gold, 45, 384);
            PixelPrimitiveCircleFactory.CreateInGoldBoom(Projectile.Center);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 45);
            fx.Scale *= 3f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 45);
            fx2.Scale *= 1.8f;


            var fx3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 45);
            fx3.Scale *= 1.8f;
            fx3.VectorScale.X *= 8;
            fx3.VectorScale.Y *= 0.5f;

            if(Style == 1)
            {
                FXUtil.CreateRipple(Projectile.Center);
                fx.Scale *= 0.5f;
                fx2.Scale *= 0.5f;
                fx3.Scale *= 0.5f;
            }

            for (float f = 0; f < 8; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.Gold;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 1.5f;

            }

            if(Style != 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 64);
                SoundStyle spawnSound = AssetRegistry.Sounds.Celestia.ArrowCrash with { PitchVariance = 0.3f };
                SoundEngine.PlaySound(spawnSound, Projectile.position);
            }

 
        }

        if (Style != 1)
        {
            ShakeScreenPosition.Shake = MathHelper.Lerp(18, 1f, EasingFunction.InSine(Timer / 60f));
        }
        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.fast = true;
            sp.gravity = 0;
        }
    }


    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);

        return false;
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {


        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 1.5f;
        waveDrawer.color = Color.Gold;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkTurquoise * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);


        Main.spriteBatch.RestartDefaults();


        shearShader.Time = MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(Timer / 120f));
        Main.spriteBatch.Restart(effect: shearShader.Effect);

        SpritebatchDrawer crackDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Visual/Particles/BigCrackParticle"), Projectile.Center);
        crackDrawer.color = Color.Lerp(Color.White, Color.Gold, EasingFunction.InExpo(Timer / 60f));
        crackDrawer.color.A = 0;
        crackDrawer.scale = Vector2.One * 2f;
        if (Style == 1)
            crackDrawer.scale *= 0.5f;
        Main.spriteBatch.Draw(crackDrawer);


        Main.spriteBatch.RestartDefaults();
    }
}
public class EreshkigalsFinisherLaser : ModProjectile,
    IDrawToRenderTarget
{
    private HashSet<NPC> _trackedNPCs;
    private List<Vector2> _laserPoints;
    private List<Vector2> LaserPoints
    {
        get
        {
            _laserPoints ??= new List<Vector2>();
            return _laserPoints;
        }
    }
  
    private Vector2 _laserPosition;
    private float Time => 90;
    private int MaxSteps => 240;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Charge => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_laserPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _laserPosition = reader.ReadVector2();
    }
    
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (LaserPoints.Count <= 2)
            return false;
        return ProjectileHelper.OldPosColliding(LaserPoints, projHitbox, targetHitbox, 64);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.timeLeft = (int)Time;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ignoreWater = true;
        Projectile.light = 0.3f;

    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        float multiplier = MathHelper.Lerp(1, 20, Charge);
        modifiers.FinalDamage *= multiplier;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            _laserPosition = Projectile.Center;
            for (float i = 0; i < 3; i++)
            {
                float ratio = i / 3f;
                Vector2 v = -Projectile.velocity;
                v *= ratio * 6;
                var s = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center,
                    Projectile.velocity.SafeNormalize(Vector2.Zero) * v,
                    Scale: MathHelper.Lerp(4f, 3f, ratio));
                s.Scale *= 2;
            }

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), _laserPosition, Vector2.Zero, ModContent.ProjectileType<EreshkigalCrack>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            for (float f = 0; f < 24; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2f);
                vel *= Main.rand.NextFloat(5f, 15f);
                DustParticle dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel);
                dp.outerColor = Color.Gold;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
            }
            _trackedNPCs ??= new HashSet<NPC>();
            for (int i = 0; i < MaxSteps; i++)
            {
                NPC npc = NPCHelper.FindClosestNPC(_laserPosition, 1024, _trackedNPCs);
                if (npc != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(_laserPosition, npc.Center, Projectile.velocity, 0.6f);
                    float dist = Vector2.Distance(_laserPosition, npc.Center);
                    if (dist < 24)
                    {
                        _trackedNPCs.Add(npc);
                    }
                }

                LaserPoints.Add(_laserPosition);
                _laserPosition += Projectile.velocity;
            }
        }


        FXUtil.ApplyContrast(MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / Time)));
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
        //    return base.OnTileCollide(oldVelocity);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EreshkigalCrack>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawMuzzleFlash(SpriteBatch sb, Vector2 screenPos)
    {
        Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
        Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
        Vector2 drawCenter = Projectile.Center - screenPos;
        Color drawColor = Color.Gold;
        drawColor.A = 0;

        float width = Projectile.timeLeft / 30f;
        float outWidth = EasingFunction.InOutSine(width);
        float scale = outWidth;
        Vector2 flashScale = Vector2.One;
        flashScale.X *= 1.5f;
        flashScale.Y *= 1.2f;
        flashScale *= scale;
        sb.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);

        drawColor = Color.White;
        drawColor.A = 0;
        sb.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {
        return Color.White;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(90, 124, completionRatio) * MathHelper.SmoothStep(1f, 0f, Timer / Time) * 0.35f;
    }

    private float StarryTrailWidthFunction2(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 2.6f;
    }

    private float StarryTrailWidthFunction3(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 1.6f;
    }

    private float StarryTrailWidthFunction4(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 3.5f;
    }

    private void DrawLaser(GraphicsDevice gDevice)
    {
        if (LaserPoints.Count <= 2)
            return;
        float ratio = Timer / Time;
        Vector2[] trailPoints = LaserPoints.ToArray();
        Color flickerCOlor = Color.Lerp(Color.Gold, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 24));
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserTexture = TrailRegistry.SimpleTrail;
        laserShader.InnerColor = flickerCOlor;
        laserShader.OuterColor = Color.Lerp(Color.DarkGoldenrod, Color.Black, ratio);
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction4, laserShader);

        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = flickerCOlor;
        laserShader.OuterColor = Color.Lerp(Color.Gold, Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction3, laserShader);

        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader);
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawLaser);
        PixelationManager.QueueSpritebatchDrawAction(DrawMuzzleFlash);
        //throw new NotImplementedException();
    }
}
public class EreshkigalsFinisherCannonHold : ModProjectile,
    IDrawToRenderTarget
{
    private enum AIState
    {
        Charging,
        Fully_Ready,
        Shoot
    }

    private float _shootTimer;
    private bool _midPoint;
    private bool _hasCharged;
    private float _sinOsc;
    private float _whiteFlashAlpha;
    private Vector2 _shakeOffset;
    private float _recoil;
    private Player Owner => Main.player[Projectile.owner];
    private Asset<Texture2D> _partsTextureAsset;
    private Asset<Texture2D> _partsOutlineTextureAsset;
    private Asset<Texture2D> _partsWhiteTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float Charge => ref Projectile.ai[2];
    private float ChargeTime => 666;
    private float ShootTime => 60f;
    public override string Texture => ModContent.GetInstance<EreshkigalsFinisherCannon>().Texture;

    private Vector2 ParticleChargePoint => Projectile.Center + Projectile.velocity * 64;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = 5555;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.light = 0.6f;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            PlayStartupSound();
        }

        if (Timer % 8 == 0)
        {
            int type = Main.rand.NextBool(2) ? ModContent.DustType<Sparkle>() : ModContent.DustType<GlowSparkleDust>();
            Dust.NewDustPerfect(ParticleChargePoint + Main.rand.NextVector2Circular(80, 80), type, Vector2.Zero, Scale: 1f, newColor: Color.Gold);

        }

        if (Timer % 12 == 0)
        {
            var d = Dust.NewDustPerfect(ParticleChargePoint + Main.rand.NextVector2Circular(80, 80), DustID.GemTopaz, Vector2.Zero, Scale: 0.5f);
            d.noGravity = true;
        }

        if (Timer % 10 == 0)
        {
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(ParticleChargePoint + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 1.2f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            sp.parent = Projectile;
            sp.behindLayer = true;
        }

        switch (State)
        {
            case AIState.Charging:
                AI_Charging();
                break;
            case AIState.Fully_Ready:
                AI_FullyReady();
                break;
            case AIState.Shoot:
                AI_Shoot();
                break;
        }
        _whiteFlashAlpha = MathHelper.Lerp(_whiteFlashAlpha, 0f, 0.02f);
    }
    private void PlayFullyChargedSound()
    {
        SoundStyle s;
        switch (Main.rand.Next(1))
        {
            default:
            case 0:
                s = new SoundStyle("Stellamod/Assets/Sounds/GW3");
                break;
        }
        s.Volume = 0.8f;
        s.Pitch = 0.6f;
        SoundEngine.PlaySound(s, Projectile.position);
    }
    private void PlayMidpointSound()
    {
        SoundStyle s;
        switch (Main.rand.Next(1))
        {
            default:
            case 0:
                s = new SoundStyle("Stellamod/Assets/Sounds/GoldPrice4");
                break;
        }
        s.Volume = 0.8f;
        s.PitchVariance = 0.5f;
        SoundEngine.PlaySound(s, Projectile.position);
    }
    private void PlayStartupSound()
    {
        SoundStyle s;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                s = new SoundStyle("Stellamod/Assets/Sounds/GoldenStart1");
                break;
            case 1:
                s = new SoundStyle("Stellamod/Assets/Sounds/GoldenStart2");
                break;
        }
        s.Volume = 0.8f;
        s.PitchVariance = 0.5f;
        SoundEngine.PlaySound(s, Projectile.position);
    }
    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void Hold()
    {
        _recoil *= 0.96f;
        if (this.OwnedByLocalClient())
        {
            Vector2 targetVelocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 0.04f);
            Projectile.netUpdate = true;
        }

        Vector2 recoilOffset = Vector2.Lerp(Vector2.Zero, Projectile.velocity * -60, _recoil);
        Projectile.Center = Owner.Center + Projectile.velocity * 60 + recoilOffset;
        float dir = Projectile.velocity.X < 0 ? -1 : 1;
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(45 * _recoil * dir);
        AI_OrientHand();
    }

    private void AI_Shoot()
    {
        Hold();
        _shootTimer++;
        if (_shootTimer == 1)
            _recoil = 1;
        if (this.OwnedByLocalClient())
        {
            Vector2 targetVelocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 0.04f);
            Projectile.netUpdate = true;
            if (_shootTimer == 1)
            {
                Vector2 velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 5;
                int damage = (int)(Projectile.damage * Charge);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<EreshkigalsFinisherLaser>(), damage, Projectile.knockBack, Projectile.owner, ai1: Charge);
            }
        }

        if (_shootTimer >= ShootTime)
        {
            Projectile.Kill();
        }
    }
    private void AI_FullyReady()
    {
        Hold();
        if (this.OwnedByLocalClient() && !Owner.channel)
        {
            SwitchState(AIState.Shoot);

        }
    }

    private void AI_Charging()
    {
        Hold();
        if (Charge < 1f)
        {
            ChargeParticles(ParticleChargePoint);
        }

        if (Charge >= 0.5f && !_midPoint)
        {
            PlayMidpointSound();
            _midPoint = true;
        }

        if (!_hasCharged && Charge >= 1f)
        {
            ShakeScreenPosition.Shake = 4;
            PlayFullyChargedSound();
            _whiteFlashAlpha = 1f;
            _hasCharged = true;
        }

        Charge = MathHelper.Clamp(Timer / ChargeTime, 0f, 1f);
        float shakeStrength = MathHelper.Lerp(0f, 4f, Charge);
        if (Timer % 2 == 0)
        {
            _shakeOffset = Main.rand.NextVector2Circular(shakeStrength, shakeStrength);
        }
        if (_hasCharged || !Owner.channel)
        {
            SwitchState(AIState.Fully_Ready);
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public void ChargeParticles(Vector2 center)
    {
        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(384, 384);
            Vector2 vel = (center - pos);
            vel *= 0.05f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
            fx.OuterGlowColor = Color.Gold;
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(384, 384);
            Vector2 vel = (center - pos);
            vel *= 0.1f;
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.Scale *= 0.35f;
            dp.outerColor = Color.Gold;
            dp.gravity = 0;
        }

        if (Timer % 40 == 0)
        {
            PixelPrimitiveCircleFactory.CreateEreshkigalSuck(Projectile);
        }
    }
    private void AI_OrientHand()
    {

        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        //Owner.itemRotation = rotation * Owner.direction;

        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90));// set arm position (90 degree offset since arm starts lowered)
        Owner.heldProj = Projectile.whoAmI;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _partsTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts");
        _partsOutlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts_Outline");
        _partsWhiteTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts_White");

        float easing = EasingFunction.InOutExpo(Charge);
        float inEasing = EasingFunction.InOutSine(Timer / 30f);
        float alpha = MathHelper.Lerp(0f, 1f, inEasing);
        float shrinkIn = MathHelper.Lerp(1f, 0f, inEasing);
        float brighteningOsc = ExtraMath.Osc(0.55f, 1f, speed: 14);
        Vector2 normalVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 pos = Projectile.Center + _shakeOffset;
        SpritebatchDrawer backDrawer = SpritebatchDrawer.FromTextureAsset(_partsTextureAsset, pos);
        backDrawer.rotation = Projectile.rotation;
        float dir = 1;
        if (normalVelocity.X < 0)
        {
            backDrawer.spriteEffects = SpriteEffects.FlipVertically;
            dir = -1;
        }
        float shakeSpeed = MathHelper.Lerp(0f, 6, easing);

        dir *= MathHelper.Lerp(0.8f, 1f, _sinOsc);

        backDrawer.VerticalFrame(2, 3);
        backDrawer.CenterOrigin();

        backDrawer.color *= alpha;
        Vector2 right = normalVelocity;
        Vector2 left = -normalVelocity;
        backDrawer.worldPosition += Vector2.Lerp(Vector2.Zero, right * dir * 32, easing + shrinkIn);
        Main.spriteBatch.Draw(backDrawer);

        SpritebatchDrawer backGlowDrawer = backDrawer;
        backGlowDrawer.color = Color.Lerp(Color.Black, Color.Gold, easing) * brighteningOsc;
        backGlowDrawer.color.A = 0;
        Main.spriteBatch.Draw(backGlowDrawer);
;

        Vector2 topPosition = pos + Vector2.Lerp(Vector2.Zero, left * dir * 32, easing + shrinkIn);
        SpritebatchDrawer handleDrawer = backDrawer;
        handleDrawer.VerticalFrame(0, 3);
        handleDrawer.worldPosition = topPosition + Vector2.Lerp(right * dir * 100, Vector2.Zero, easing);
        handleDrawer.color *= alpha;
        //  handleDrawer.worldPosition = Projectile.Center + Vector2.Lerp(Vector2.Zero, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 32, easing);
        Main.spriteBatch.Draw(handleDrawer);


        SpritebatchDrawer gunDrawer = backDrawer;
        gunDrawer.VerticalFrame(1, 3);
        gunDrawer.color *= alpha;
 
        gunDrawer.worldPosition = topPosition;
        Main.spriteBatch.Draw(gunDrawer);

        SpritebatchDrawer gunGlowDrawer = gunDrawer;
        gunGlowDrawer.color = Color.Lerp(Color.Black, Color.Gold, easing) * brighteningOsc;
        gunGlowDrawer.color.A = 0;
        Main.spriteBatch.Draw(gunGlowDrawer);



        SpritebatchDrawer handleGlowDrawer = handleDrawer;
        handleGlowDrawer.color = Color.Lerp(Color.Black, Color.Gold, easing) * brighteningOsc;
        handleGlowDrawer.color.A = 0;
        Main.spriteBatch.Draw(handleGlowDrawer);


        SpritebatchDrawer whiteFlashBackDrawer = backDrawer;
        SpritebatchDrawer whiteFlashGunDrawer = gunDrawer;
        SpritebatchDrawer whiteFlashHandleDrawer = handleDrawer;


        whiteFlashBackDrawer.texture = _partsWhiteTextureAsset.Value;
        whiteFlashGunDrawer.texture = _partsWhiteTextureAsset.Value;
        whiteFlashHandleDrawer.texture = _partsWhiteTextureAsset.Value;

        whiteFlashBackDrawer.color = whiteFlashGunDrawer.color = whiteFlashHandleDrawer.color = Color.White * _whiteFlashAlpha;
        Main.spriteBatch.Draw(whiteFlashHandleDrawer);
        Main.spriteBatch.Draw(whiteFlashGunDrawer);
        Main.spriteBatch.Draw(whiteFlashBackDrawer);

        Color glowColor = Color.Lerp(Color.Gold, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 8));
        whiteFlashBackDrawer.texture = _partsOutlineTextureAsset.Value;
        whiteFlashGunDrawer.texture = _partsOutlineTextureAsset.Value;
        whiteFlashHandleDrawer.texture = _partsOutlineTextureAsset.Value;

        whiteFlashBackDrawer.color = whiteFlashGunDrawer.color = whiteFlashHandleDrawer.color = glowColor * easing;
        Main.spriteBatch.Draw(whiteFlashHandleDrawer);
        Main.spriteBatch.Draw(whiteFlashGunDrawer);
        Main.spriteBatch.Draw(whiteFlashBackDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    private void DrawGlowOrb(SpriteBatch sb, Vector2 screenPos)
    {
        float easing = EasingFunction.InOutExpo(Charge);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ParticleChargePoint);
        Color flickerColor = Color.Lerp(Color.Gold, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 16));
        glowDrawer.color = Color.Lerp(Color.Transparent, Color.Gold, easing);
        glowDrawer.color = Color.Lerp(glowDrawer.color, flickerColor, easing);
        glowDrawer.color = Color.Lerp(glowDrawer.color, Color.Black, EasingFunction.OutExpo(_shootTimer / 60f));
        glowDrawer.color.A = 0;
        glowDrawer.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, easing);
        glowDrawer.scale.Y *= 0.4f;
        glowDrawer.scale *= 0.35f;
        glowDrawer.rotation = Projectile.rotation;
        sb.Draw(glowDrawer);

        glowDrawer.color = Color.Lerp(Color.Transparent, Color.White, easing);
        glowDrawer.color = Color.Lerp(glowDrawer.color, Color.Black, EasingFunction.OutExpo(_shootTimer / 60f));
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.96f;
        sb.Draw(glowDrawer);


    }

    private void DrawPixelatedEffects(SpriteBatch sb, Vector2 screenPos)
    {
        float inEasing = EasingFunction.InOutSine(Timer / 30f);
        float alpha = MathHelper.Lerp(0f, 1f, inEasing);
        Color glowColor = Color.Lerp(Color.Gold, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 8));
        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, ParticleChargePoint);
        circleDrawer.color = glowColor * 0.16f * alpha;
        circleDrawer.color.A = 0;
        circleDrawer.scale = Vector2.Lerp(Vector2.One * 3f, Vector2.Zero, Charge);
   //     circleDrawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(circleDrawer);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawGlowOrb, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedEffects);
    }
}

public class EreshkigalsFinisherCannon : BaseGun
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        //base.SetDefaults();
        Item.damage = 45;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(0, 1, 1, 29);
        Item.rare = ItemRarityID.Green;
        Item.DamageType = DamageClass.Ranged;
        Item.shootSpeed = 0;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.noUseGraphic = true;
        Item.consumeAmmoOnLastShotOnly = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.autoReuse = false;
        Item.shoot = ModContent.ProjectileType<EreshkigalsFinisherCannonHold>();
    }

    public override bool UseDefaultHoldAnimation()
    {
        return false;
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        //base.ShootEffects(position, velocity);
    }

    public override bool CanUseItem(Player player)
    {
        Item.shootSpeed = 0;
        return base.CanUseItem(player) && player.ownedProjectileCounts[Item.shoot] == 0;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5f, 0f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<EreshkinCandle>());
    }
}
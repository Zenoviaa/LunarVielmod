using ReLogic.Content;
using ReLogic.Utilities;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.Ereshkigal;
using Stellamod.Effects.Generic;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS;

public class EreshkigalCrack : ModProjectile
{
    private VortexParticleSystem _vortexParticleSystemBackingField;
    private VortexParticleSystem VortexParticleSystem
    {
        get
        {
            _vortexParticleSystemBackingField ??= new(48);
            return _vortexParticleSystemBackingField;
        }
    }
    private float Time => 120;
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
        Projectile.timeLeft = (int)Time;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        if (Style == 1)
        {
            Projectile.extraUpdates = 3;
        }
        Timer++;
        if (Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Gold, 25, 384);
            PixelPrimitiveCircleFactory.CreateInGoldBoom(Projectile.Center);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 25);
            fx.Scale *= 3f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 20);
            fx2.Scale *= 1.8f;


            var fx3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, duration: 15);
            fx3.Scale *= 2.4f;


            if (Style == 2)
            {
                for(int i = 0; i < 4; i++)
                {
                    FXUtil.CreateRipple(Projectile.Center);
                    fx.Scale *= 0.5f;
                    fx2.Scale *= 0.5f;
                    fx3.Scale *= 0.5f;
                }

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

            if (Style == 0)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 64);

                SoundStyle shootSound = AssetRegistry.Sounds.Ereshkigal.HeavenlyShot;
                switch (Main.rand.Next(2))
                {
                    case 1:
                        shootSound = AssetRegistry.Sounds.Ereshkigal.HeavenlyShot2;
                        break;
                }
                SoundEngine.PlaySound(shootSound, Projectile.position);
            }


            if (Style == 2)
            {

                for (int i = 0; i < 48; i++)
                {
                    Vector2 vortexSpawnPos = Main.rand.NextVector2CircularEdge(252, 252);
                    Vector2 outwardVelocity = vortexSpawnPos - Vector2.Zero;
                    outwardVelocity = outwardVelocity.SafeNormalize(Vector2.Zero);
                    VortexParticleSystem.SpawnParticle(vortexSpawnPos, outwardVelocity);
                }

                if (this.OwnedByLocalClient())
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                        Vector2 pos = Projectile.Center;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<GoldenWisp>(), Projectile.damage / 12, 1, Projectile.owner);
                    }
                }


            }
        }
        VortexParticleSystem.Update();
        if (Style == 0)
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

        PixelationManager.QueueSpritebatchDrawAction(DrawVortexParticles);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        PixelationManager.QueueSpritebatchDrawAction(DrawWaveBoom);
        return false;
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawVortexParticles(SpriteBatch sb, Vector2 screenPos)
    {
        DrawParticles(sb, screenPos, VortexParticleSystem);
    }
    private void DrawParticles(SpriteBatch sb, Vector2 screenPos, VortexParticleSystem particleSystem)
    {
        float maxSize = 768;
        Rectangle worldRectangle = DrawUtilities.CenterRectangle(Projectile.Center, (int)maxSize, (int)maxSize);
        Vector2[] particles = new Vector2[particleSystem.particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            ref Vector2 pos = ref particles[i];
            pos = particleSystem.particles.positions[i];


            if (!particleSystem.particles.active[i])
            {
                //Invalidate position if the paritcle is not active
                //They'll have 0 contribution if there this far away from the rectangle
                pos = new Vector2(-9999);
                continue;
            }

            pos += Projectile.Center;

            //Normalize to screen coordinates
            pos = DrawUtilities.WorldToScreenCoordinates(pos, worldRectangle);
        }

        Rectangle screenRectangle = worldRectangle;
        screenRectangle.X -= (int)Main.screenPosition.X;
        screenRectangle.Y -= (int)Main.screenPosition.Y;

        int size = (int)MathHelper.Lerp(32, maxSize * 2, EasingFunction.OutExpo(Timer / Time));
        screenRectangle = DrawUtilities.CenterRectangle(screenRectangle, size, size);

        float particleRadius = MathF.Max(DrawUtilities.TexelSize.X, DrawUtilities.TexelSize.Y);
        particleRadius *= 100;

        StarSuckShader suckShader = ShaderContent.GetInstance<StarSuckShader>();
        suckShader.FarColor = Color.Lerp(Color.Purple, Color.White, 0.35f);
        suckShader.CloseColor = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 12));
        suckShader.BloomColor = Color.White;
        suckShader.CenterNormalizedCoord = DrawUtilities.WorldToScreenCoordinates(Projectile.Center);
        suckShader.ParticleRadius = particleRadius;
        suckShader.Particles = particles;
        suckShader.Time = Main.GlobalTimeWrappedHourly * 12f;
        suckShader.Swirliness = 8;
        sb.Restart(effect: suckShader.Effect);
        sb.Draw(TextureAssets.BlackTile.Value, screenRectangle, null, Color.Lerp(Color.White, Color.Transparent, EasingFunction.OutExpo(Timer / Time)), 0, Vector2.Zero, SpriteEffects.None, 0); ;
        sb.RestartDefaults();
    }

    private void DrawWaveBoom(SpriteBatch sb, Vector2 screenPos)
    {
        if (Style != 2)
            return;

        float t = Time * 0.66f;
        RoyalShockwaveCircleShader shockwaevShader = RoyalShockwaveCircleShader.Instance;
        shockwaevShader.Time = -Timer * 0.02f + 0.8f;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        sbDrawer.CenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1.7f, EasingFunction.OutExpo(Timer / t));
        sbDrawer.scale.Y += MathHelper.Lerp(2f, 0f, EasingFunction.InOutExpo(Timer / t));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 4f, EasingFunction.QuadraticBump(Timer / t));
        sbDrawer.color = Color.Lerp(Color.Gold, Color.Lavender, ExtraMath.Osc(0f, 1f, speed: 32));
        sbDrawer.color *= MathHelper.Lerp(1f, 0.0f, EasingFunction.OutExpo(Timer / t));
        sbDrawer.color.A = 0;
        sb.Draw(sbDrawer);


        sbDrawer.CenterOrigin();
        sbDrawer.color = Color.Lerp(Color.Lavender, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 32));
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / t));
        sbDrawer.scale.Y += MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / t));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 4f, EasingFunction.QuadraticBump(Timer / t));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sb.Draw(sbDrawer);

        sb.RestartDefaults();
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

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset, Projectile.Center);
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

public class GoldenWisp : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.timeLeft = 240;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 5;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity *= 0.98f;
    }

    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    private void DrawWisp(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Rectangle worldRectangle = DrawUtilities.CenterRectangle(Projectile.Center, 768, 768);
        Rectangle screenRectangle = worldRectangle;
        screenRectangle.X -= (int)Main.screenPosition.X;
        screenRectangle.Y -= (int)Main.screenPosition.Y;
        Vector2[] particles = DrawUtilities.TrailLocalRectanglePoints(Projectile.oldPos, Projectile.Center, worldRectangle);
        GlowyTrailShader trailShader = ShaderContent.GetInstance<GlowyTrailShader>();
        trailShader.ParticleRadius = 0.035f * MathHelper.Lerp(0f, 1f, (float)(Projectile.timeLeft / 240f));
        trailShader.InsideColor = Color.Gold;//Color.Lerp(Color.PaleGoldenrod, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 12, offset: Projectile.identity));
        trailShader.BloomColor = Color.Red;
        trailShader.Particles = particles;
        SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed() with { effect = trailShader };

        Color particleColor = Color.Lerp(Color.White, Color.DarkGoldenrod, Timer / 60f);
        using (var starter = SpritebatchStarter.Begin(spriteBatch, spritebatchParams))
        {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenRectangle, null, particleColor, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawWisp);
    }
}
public class EreshkigalsFinisherLaser : ModProjectile,
    IDrawToRenderTarget
{
    private NPC _firstHitNPC;
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

    private Vector2 _initialVelocity;
    private Vector2 _laserPosition;
    private float Time => 90;
    private int MaxSteps => 240;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Charge => ref Projectile.ai[1];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_laserPosition);
        writer.WriteVector2(_initialVelocity);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _laserPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
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
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ignoreWater = true;
        Projectile.light = 0.3f;
        Projectile.tileCollide = false;

    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        float multiplier = MathHelper.Lerp(1, 220, Charge);
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
            _initialVelocity = Projectile.velocity;
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
            Projectile.velocity = Projectile.velocity.Resize(15);
            for (int i = 0; i < MaxSteps; i++)
            {
                NPC npc = NPCHelper.FindClosestNPC(_laserPosition, 2048, _trackedNPCs);

                if (npc != null)
                {
                    _firstHitNPC ??= npc;
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(_laserPosition, npc.Center, Projectile.velocity, 3);
                    float dist = Vector2.Distance(_laserPosition, npc.Center);
                    if (dist < 24)
                    {
                        _trackedNPCs.Add(npc);
                    }
                }

                LaserPoints.Add(_laserPosition);
                _laserPosition += Projectile.velocity;
            }
            if (_firstHitNPC != null && Charge > 0.5f)
            {
                if (this.OwnedByLocalClient())
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), _firstHitNPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<EreshkigalCrack>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 2);
                }
            }
        }

        FXUtil.ApplyContrast(MathHelper.Lerp(0.6f, 0f, EasingFunction.InOutExpo(Timer / Time)));
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
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
            ModContent.ProjectileType<EreshkigalCrack>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawCircles(SpriteBatch sb, Vector2 screenPos)
    {
        float time = 25;
        StarBombBoomShader shockwave = ShaderContent.GetInstance<StarBombBoomShader>();
        shockwave.Time = MathHelper.Lerp(0f, 0.5f, EasingFunction.InExpo(Timer / time));
        sb.Restart(effect: shockwave.Effect);
        for (int i = 0; i < 3; i++)
        {
            float offset = 192;
            float between = 128;
            Vector2 offse2t = _initialVelocity.SafeNormalize(Vector2.Zero) * offset;
            Vector2 pos = Projectile.Center + offse2t + _initialVelocity.SafeNormalize(Vector2.Zero) * between * i;

            float scale = MathHelper.Lerp(1f, 0.2f, i / 4f);
            SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[Type], pos);
            float yScale = MathHelper.Lerp(0.2f, 2.3f, EasingFunction.OutExpo(Timer / time)) * scale * 0.7f;
            circleDrawer.scale.Y *= yScale;
            circleDrawer.scale *= 0.75f;
            circleDrawer.rotation = _initialVelocity.ToRotation();

            Color color = Color.Lerp(Color.Blue, Color.Pink, scale);
            color = Color.Lerp(color, Color.Pink, EasingFunction.OutExpo(Timer / (time / 2f)));
            //  color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / time)) * 0.8f;

            circleDrawer.color = color;
            Main.spriteBatch.Draw(circleDrawer);
        }
        sb.RestartDefaults();
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
        Color flickerCOlor = Color.Lerp(Color.Gold, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 24));
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
        PixelationManager.QueueSpritebatchDrawAction(DrawCircles);
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
    private float _whiteFlashAlpha;
    private Vector2 _shakeOffset;
    private float _recoil;
    private Player Owner => Main.player[Projectile.owner];

    private VortexParticleSystem _vortexParticleSystemBackingField;
    private VortexParticleSystem VortexParticleSystem
    {
        get
        {
            _vortexParticleSystemBackingField ??= new(48);
            return _vortexParticleSystemBackingField;
        }
    }
    private VortexParticleSystem _vortexParticleSystemBackingField2;
    private VortexParticleSystem DustParticleSystem
    {
        get
        {
            _vortexParticleSystemBackingField2 ??= new(48);
            return _vortexParticleSystemBackingField2;
        }
    }

    private SlotId _chargeSoundSlotID;
    private MagicCircleRenderer _magicCircleRenderer;
    private Asset<Texture2D> _clockHandleTextureAsset;
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

        if (Timer % 12 == 0)
        {
            var d = Dust.NewDustPerfect(ParticleChargePoint + Main.rand.NextVector2Circular(80, 80), ModContent.DustType<TSmokeDust>(), Vector2.Zero, Scale: 0.5f);
            d.color = Color.Black;
            d.noGravity = true;
        }

        if (Timer % 6 == 0)
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
        DustParticleSystem.Update();
        VortexParticleSystem.Update();
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
        SoundStyle s = AssetRegistry.Sounds.Ereshkigal.EreshkigalsFinisherCannon;
        _chargeSoundSlotID = SoundEngine.PlaySound(s, Projectile.position);

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
        {
            if (SoundEngine.TryGetActiveSound(_chargeSoundSlotID, out ActiveSound? result))
            {
                result.Stop();
            }
        }
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
                if (Charge > 0.5f)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity * 16, velocity,
                  ModContent.ProjectileType<EreshkigalsFinisherLaser>(), damage, Projectile.knockBack, Projectile.owner, ai1: Charge);
                }
                else
                {
                    for (float f = 0; f < 16; f++)
                    {
                        Vector2 spawnPoint = Projectile.Center + velocity * 16;
                        Vector2 vel = velocity;
                        vel = vel.RotatedByRandom(MathHelper.ToRadians(38));
                        vel *= Main.rand.NextFloat(0.5f, 1f);
                        var dp = DustParticle.Spawn(spawnPoint, vel);
                        dp.outerColor = Color.Gold;
                        dp.gravity = 0;
                        dp.dampening = 0.05f;
                    }
                }

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


        if(SoundEngine.TryGetActiveSound(_chargeSoundSlotID, out ActiveSound r))
        {
            r.Position = Projectile.Center;
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

            Vector2 vortexSpawnPos = Main.rand.NextVector2CircularEdge(252, 252);
            Vector2 outwardVelocity = vortexSpawnPos - Vector2.Zero;
            outwardVelocity = outwardVelocity.SafeNormalize(Vector2.Zero);
            VortexParticleSystem.SpawnParticle(vortexSpawnPos, outwardVelocity);


        }

        if (Timer % 3 == 0)
        {


            Vector2 vortexSpawnPos = Main.rand.NextVector2CircularEdge(300, 300);
            Vector2 outwardVelocity = vortexSpawnPos - Vector2.Zero;
            outwardVelocity = outwardVelocity.SafeNormalize(Vector2.Zero);
            DustParticleSystem.SpawnParticle(vortexSpawnPos, outwardVelocity * 3);
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

        if (Timer % 20 == 0)
        {
            PixelPrimitiveCircleFactory.CreateGenericInBoom(center, Color.Gold, Color.SkyBlue, 25, 200);
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
                                                                                                                                // Owner.heldProj = Projectile.whoAmI;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color glowColor = Color.Lerp(Color.Gold, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 8));
        _partsTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts");
        _partsOutlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts_Outline");
        _partsWhiteTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Parts_White");
        _clockHandleTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_ClockHandle");

        float easing = EasingFunction.InOutExpo(Charge);
        float inEasing = EasingFunction.InOutSine(Timer / 30f);
        float alpha = MathHelper.Lerp(0f, 1f, inEasing);
        float shrinkIn = MathHelper.Lerp(1f, 0f, inEasing);
        float brighteningOsc = ExtraMath.Osc(0.55f, 1f, speed: 14);
        Vector2 normalVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 pos = Projectile.Center + _shakeOffset;
        SpritebatchDrawer backDrawer = SpritebatchDrawer.FromTextureAsset(_partsTextureAsset, pos);

        backDrawer.VerticalFrame(2, 3);
        backDrawer.CenterOrigin();
        backDrawer.color *= alpha;
        backDrawer.rotation = Projectile.rotation;
        float dir = 1;
        if (normalVelocity.X < 0)
        {
            backDrawer.spriteEffects = SpriteEffects.FlipVertically;
            dir = -1;
        }
        Vector2 right = normalVelocity;
        Vector2 left = -normalVelocity;
        backDrawer.worldPosition += Vector2.Lerp(Vector2.Zero, right * dir * 32, easing + shrinkIn);




        float easing2 = EasingFunction.InExpo(Charge);
        Vector2 topPosition = pos + Vector2.Lerp(Vector2.Zero, left * 32, easing + shrinkIn);

        SpritebatchDrawer handleDrawer = backDrawer;
        handleDrawer.VerticalFrame(0, 3);
        handleDrawer.worldPosition = topPosition + Vector2.Lerp(right * 100, Vector2.Zero, Charge);
        handleDrawer.color *= alpha;




        float shakeSpeed = MathHelper.Lerp(0f, 6, easing);
        dir *= 0.8f;


        SpritebatchDrawer gunDrawer = backDrawer;
        gunDrawer.VerticalFrame(1, 3);
        gunDrawer.color *= alpha;
        gunDrawer.worldPosition = topPosition;

        Vector2 position = topPosition;
        Vector2 offset = new Vector2(-14, -14 * dir).RotatedBy(Projectile.rotation);
        position += offset;

        SpritebatchDrawer clockHand1 = SpritebatchDrawer.FromTextureAsset(_clockHandleTextureAsset, position);
        clockHand1.VerticalFrame(1, 2);
        clockHand1.LeftCenterOrigin();
        float radians = MathHelper.TwoPi * 6;
        clockHand1.rotation = Projectile.rotation + MathHelper.Lerp(radians, 0, Charge);


        SpritebatchDrawer clockHand2 = clockHand1;
        clockHand2.VerticalFrame(0, 2);
        clockHand2.LeftCenterOrigin();
        clockHand2.rotation = Projectile.rotation + MathHelper.Lerp(radians, 0, Charge) * 0.5f;

        SpritebatchDrawer outlineBackDrawer = backDrawer;
        SpritebatchDrawer outlineHandleDrawer = handleDrawer;
        SpritebatchDrawer outlineGunDrawer = gunDrawer;
        outlineBackDrawer.texture = _partsOutlineTextureAsset.Value;
        outlineHandleDrawer.texture = _partsOutlineTextureAsset.Value;
        outlineGunDrawer.texture = _partsOutlineTextureAsset.Value;

        outlineBackDrawer.color = outlineHandleDrawer.color = outlineGunDrawer.color = glowColor * easing;
        Main.spriteBatch.Draw(outlineBackDrawer);
        Main.spriteBatch.Draw(outlineHandleDrawer);
        Main.spriteBatch.Draw(outlineGunDrawer);



        SpritebatchDrawer glowingClockHand1 = clockHand1;
        SpritebatchDrawer glowingClockHand2 = clockHand2;

        glowingClockHand1.color = glowingClockHand2.color = Color.White * 0.65f * ExtraMath.Osc(0.35f, 1f, speed: 12);
        glowingClockHand1.color.A = glowingClockHand2.color.A = 0;
        glowingClockHand1.scale = glowingClockHand2.scale *= 1.6f;
        Main.spriteBatch.Draw(glowingClockHand1);
        Main.spriteBatch.Draw(glowingClockHand2);


        glowingClockHand1.color = Color.Gold * 0.5f;
        glowingClockHand1.color.A = 0;
        glowingClockHand1.scale *= new Vector2(1f, 0.2f) * 0.09f;
        glowingClockHand1.texture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        glowingClockHand1.sourceRect = null;
        glowingClockHand1.LeftCenterOrigin();
        Main.spriteBatch.Draw(glowingClockHand1);

        glowingClockHand2 = glowingClockHand1;
        glowingClockHand2.rotation = clockHand2.rotation;
        glowingClockHand2.scale.X *= 1.85f;
        Main.spriteBatch.Draw(glowingClockHand2);

        Main.spriteBatch.Draw(backDrawer);

        SpritebatchDrawer backGlowDrawer = backDrawer;
        backGlowDrawer.color = Color.Lerp(Color.Black, Color.Gold, easing) * brighteningOsc;
        backGlowDrawer.color.A = 0;
        Main.spriteBatch.Draw(backGlowDrawer);
        Main.spriteBatch.Draw(handleDrawer);
        Main.spriteBatch.Draw(gunDrawer);



        Main.spriteBatch.Draw(clockHand1);
        Main.spriteBatch.Draw(clockHand2);

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

     
        return false;
        //return base.PreDraw(ref lightColor);
    }

    private void DrawPixelatedGlow(SpriteBatch sb, Vector2 screenPos)
    {
        float qb = EasingFunction.InOutExpo(Timer / ChargeTime);
        float chargeRatio = EasingFunction.InOutSine(Timer / ChargeTime);
        for (int i = 1; i < 4; i++)
        {
            Vector2 pos = ParticleChargePoint + Projectile.velocity * 64 * i * MathHelper.Lerp(0.75f, 1f, qb);
            Vector2 velociy = Projectile.velocity;
            Color targetColor = Color.Gold;

            float ratio = EasingFunction.InOutSine((Timer - (ChargeTime / 6f) * i) / 30f);
            Color glowColor = Color.Lerp(Color.Black, targetColor, ratio);

            float size = MathHelper.Lerp(256, 128, i / 3f);
            size *= MathHelper.Lerp(4f, 1f, ratio);

            SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, pos);
            bloomDrawer.color = Color.Lerp(Color.Black, Color.Lerp(Color.Gold, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 12)), ratio) * 0.6f;
            bloomDrawer.color.A = 0;
            bloomDrawer.scale *= 0.3f * MathHelper.Lerp(1f, 0f, (float)(i - 1) / 3f);
            bloomDrawer.scale.X *= 0.5f;
            bloomDrawer.scale.Y *= 3;
            bloomDrawer.rotation = velociy.ToRotation();
            Main.spriteBatch.Draw(bloomDrawer);
        }
    }

    private void DrawPixelatedRings(GraphicsDevice gDevice)
    {
        Asset<Texture2D> magicCircleTexture = AssetManager.GlowMask.MagicCircleVampiricVine;
        _magicCircleRenderer ??= new MagicCircleRenderer(magicCircleTexture);
        float qb = EasingFunction.InOutExpo(Timer / ChargeTime);
        float chargeRatio = EasingFunction.InOutSine(Timer / ChargeTime);
        for (int i = 1; i < 4; i++)
        {
            Vector2 pos = ParticleChargePoint + Projectile.velocity * 64 * i * MathHelper.Lerp(0.75f, 1f, qb);
            Vector2 velociy = Projectile.velocity;
            Color targetColor = Color.Gold;

            float ratio = EasingFunction.InOutSine((Timer - (ChargeTime / 6f) * i) / 30f);
            Color glowColor = Color.Lerp(Color.Black, targetColor, ratio);

            float size = MathHelper.Lerp(256, 128, i / 3f);
            size *= MathHelper.Lerp(4f, 1f, ratio);
            _magicCircleRenderer.DrawRing(pos, velociy, 0, 1, glowColor, Main.GlobalTimeWrappedHourly * 3 * i, size);
        }
    }

    private void DrawGlowOrb(SpriteBatch sb, Vector2 screenPos)
    {
        float easing = EasingFunction.InOutExpo(Charge);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ParticleChargePoint);
        Color flickerColor = Color.Lerp(Color.Gold, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
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

    private void DrawParticles(SpriteBatch sb, Vector2 screenPos, VortexParticleSystem particleSystem)
    {
        Rectangle worldRectangle = DrawUtilities.CenterRectangle(ParticleChargePoint, 768, 768);
        Vector2[] particles = new Vector2[particleSystem.particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            ref Vector2 pos = ref particles[i];
            pos = particleSystem.particles.positions[i];


            if (!particleSystem.particles.active[i])
            {
                //Invalidate position if the paritcle is not active
                //They'll have 0 contribution if there this far away from the rectangle
                pos = new Vector2(-9999);
                continue;
            }

            pos += Projectile.Center;
            //Normalize to screen coordinates
            pos = DrawUtilities.WorldToScreenCoordinates(pos, worldRectangle);
        }

        Rectangle screenRectangle = worldRectangle;
        screenRectangle.X -= (int)Main.screenPosition.X;
        screenRectangle.Y -= (int)Main.screenPosition.Y;

        int size = (int)MathHelper.Lerp(768, 32, Charge);
        screenRectangle = DrawUtilities.CenterRectangle(screenRectangle, size, size);

        float particleRadius = MathF.Max(DrawUtilities.TexelSize.X, DrawUtilities.TexelSize.Y);
        particleRadius *= 100;
        bool drawDust = particleSystem == DustParticleSystem;
        if (drawDust)
            particleRadius *= 0.1f;
        StarSuckShader suckShader = ShaderContent.GetInstance<StarSuckShader>();
        suckShader.FarColor = Color.Lerp(Color.SkyBlue, Color.White, 0.35f);
        suckShader.CloseColor = Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 12));
        suckShader.BloomColor = Color.White;
        suckShader.CenterNormalizedCoord = DrawUtilities.WorldToScreenCoordinates(Projectile.Center);
        suckShader.ParticleRadius = particleRadius;
        suckShader.Particles = particles;
        suckShader.Time = Main.GlobalTimeWrappedHourly * 12f;
        suckShader.Swirliness = drawDust ? 1 : 8;
        sb.Restart(effect: suckShader.Effect);
        sb.Draw(TextureAssets.BlackTile.Value, screenRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); ;
        sb.RestartDefaults();
    }

    private void DrawPixelatedEffects(SpriteBatch sb, Vector2 screenPos)
    {
        float inEasing = EasingFunction.InOutSine(Timer / 30f);
        float alpha = MathHelper.Lerp(0f, 1f, inEasing);
        Color glowColor = Color.Lerp(Color.Gold, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 8));
        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, ParticleChargePoint);
        circleDrawer.color = glowColor * 0.16f * alpha;
        circleDrawer.color.A = 0;
        circleDrawer.scale = Vector2.Lerp(Vector2.One * 3f, Vector2.Zero, Charge);
        Main.spriteBatch.Draw(circleDrawer);

        DrawParticles(sb, screenPos, VortexParticleSystem);

        DrawParticles(sb, screenPos, DustParticleSystem);


        //   sb.DrawScreenRectangle();
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawGlowOrb, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedEffects);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRings);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedGlow);
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

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 1;
        fireParams.reloadWindow = 200;
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
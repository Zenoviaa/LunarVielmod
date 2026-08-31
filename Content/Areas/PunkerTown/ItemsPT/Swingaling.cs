using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

//Stamina will charge and then fire multiple bolts, it'll be so cool
//You'll hold it btw

public class Swingaling : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 72;
        Item.shoot = ModContent.ProjectileType<SwingalingSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SwingalingCharge>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<MarshScrap>());
    }
}

public class SwingalingSlash : BaseSwingProjectileV2
{
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSwordSwingStyle(this);
        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;
        outlineColor = Color.White;
        useAfterImage = true;
        swordBeamLength = 180;

        //Bloom
        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.DarkGray;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 42, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.Black, Color.White, ratio) * 0.73f;
    }
    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Chillrend;
    }

    public override void AI()
    {
        base.AI();
        outlineColor = Color.Lerp(Color.White, Color.Black, ExtraMath.Osc(0f, 1f, speed: 12, 0));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (_hit)
            return;


        SoundStyle e = SoundID.DD2_LightningAuraZap with { Pitch = -0.25f };
        SoundEngine.PlaySound(e, target.position);
        FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
        var fx = FXUtil.GlowCircleBoom(target.Center, Color.White * 0.5f, Color.LightGray * 0.5f, Color.DarkGray * 0.5f, duration: 15, baseSize: 0.2f);
        //fx.Scale *= 2;
        for (float n = 0; n < 5; n++)
        {
            DustParticle dp = DustParticle.Spawn(target.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 50));
            dp.innerColor = Color.White;
            dp.outerColor = Color.DarkGray;
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.1f;
            dp.Scale *= 0.85f;
        }
        _hit = true;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
}
public class SwingalingBlast : ModProjectile,
    IDrawToRenderTarget
{
    private float _widthMultiplier;
    private float _zapTime;
    private float _flashTimer;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;

    private Vector2 _controlPoint3;
    private Vector2 _controlPoint4;
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity;
    private Vector2 EndPoint2 => Projectile.Center - Projectile.velocity;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.friendly = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), EndPoint + (EndPoint - Projectile.Center).SafeNormalize(Vector2.Zero) * 32, Projectile.Center, 12, ref collisionPoint))
            return true;

        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            if (Style == 0)
            {
                SoundStyle zapSound;
                int rand = Main.rand.Next(4);
                switch (rand)
                {
                    default:
                    case 0:
                        zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap1 with { PitchVariance = 0.3f };
                        break;
                    case 1:
                        zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap2 with { PitchVariance = 0.3f };
                        break;
                    case 2:
                        zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap3 with { PitchVariance = 0.3f };
                        break;
                    case 3:
                        zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap4 with { PitchVariance = 0.3f };
                        break;
                }
                zapSound.MaxInstances = 3;
                //   zapSound.Volume = 0.3f;
                SoundEngine.PlaySound(zapSound, Projectile.position);
                if (Main.rand.NextBool(4))
                {
                    SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
                    lightningSoundStyle.PitchVariance = 0.4f;
                    SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);
                }
            }


            FXUtil.ShakeCamera(Projectile.Center, 1024, 12);

            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(9, 9);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.DarkBlue;
                var d = DustParticle.Spawn(EndPoint, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 0.5f;
            }
            _controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint2 = Projectile.Center + Main.rand.NextVector2CircularEdge(192, 192);
            _controlPoint3 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _controlPoint4 = Projectile.Center + Main.rand.NextVector2Circular(192, 192);
            _widthMultiplier = Main.rand.NextFloat(0.5f, 1f);
            var fx = FXUtil.GlowCircleBoom(EndPoint, Color.White, Color.SkyBlue, Color.Blue);
            fx.Scale *= 2;
        }

        if (Timer % 5 == 0 && Timer < 30)
        {
            FXUtil.GlowCircleBoom(EndPoint, Color.SkyBlue, Color.DeepSkyBlue, Color.Black);
        }

        if (Timer % 3 == 0)
        {


        }
        if (Timer % 10 == 0)
        {
            _zapTime = Main.rand.NextFloat(0, 100);

        }

        if (Timer % 40 == 0)
        {
            _flashTimer = 28;
        }
        _flashTimer--;
    }


    private Color GetTrailColor(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth(float ratio)
    {
        float ease = EasingFunction.InOutSine(_flashTimer / 30f);
        float w = 20 * _widthMultiplier;
        float outEasing = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.SkyBlue, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DeepSkyBlue, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }

    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        int numPoints = 64;
        List<Vector2> trailPoints = new List<Vector2>(numPoints);
        Vector2 up = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        float outEase = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        Vector2 cp1 = Vector2.Lerp(_controlPoint1, _controlPoint3, outEase);
        Vector2 cp2 = Vector2.Lerp(_controlPoint2, _controlPoint4, outEase);
        for (float f = 0; f < numPoints; f++)
        {
            float ratio = (float)f / numPoints;
            Vector2 startPoint = Projectile.Center;
            Vector2 trailPoint = ExtraMath.CubicBezier(startPoint,
                cp1, cp2, EndPoint2, ratio);
            // Vector2 trailPoint = Vector2.Lerp(startPoint, endPoint, ratio);
            trailPoint += up * MathF.Sin(ratio * 16 + _zapTime) * 32;
            trailPoints.Add(trailPoint);
        }

        for (int i = 0; i < 4; i++)
            trailPoints.Add(trailPoints[trailPoints.Count - 1]);
        Vector2[] lightningPoints = trailPoints.ToArray();
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.8f;

        float time = Main.GlobalTimeWrappedHourly * 16;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = Timer > 15 ? AssetManager.LaserTextures.TexturedLaser : AssetManager.LaserTextures.TexturedLaser2;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);

        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);
        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.SkyBlue;
        bloom.OuterColor = Color.DeepSkyBlue;
        TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
    }
}

public class SwingalingCharge : ModProjectile
{
    private Vector2 HoldOffset => new Vector2(56, 0);
    private float ThrustDistance => 96;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float State => ref Projectile.ai[1];
    private float ChargeTime => 60f;
    private float _charge;
    private bool MaxCharge;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void AI_Blast()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            lightningSoundStyle.PitchVariance = 0.4f;
            SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);
        }
        Vector2 mouseWorld = Main.MouseWorld;
        Vector2 directionToMouseWorld = Owner.Center.DirectionTo(mouseWorld);
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.ChangeDir(Projectile.direction);
            Projectile.velocity = directionToMouseWorld * ThrustDistance;
            Projectile.netUpdate = true;
        }
        float rotation = Projectile.velocity.ToRotation();
        float holdRotation = rotation;
        Vector2 holdOffset = HoldOffset;

        Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
        Vector2 swingCenter = swingStart;

        Projectile.Center = swingCenter;

        if (Owner.direction == -1)
        {
            holdOffset.Y *= -1;
            rotation -= MathHelper.PiOver2;
        }
        Projectile.rotation = rotation + MathHelper.ToRadians(45) * Owner.direction;
        if (Owner.direction == -1)
        {
            Projectile.rotation -= MathHelper.Pi;
        }
        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        if (Main.rand.NextBool(16))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Scale: 0.5f);
        }


        if (Timer % 8 == 0 && this.OwnedByLocalClient())
        {
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                ModContent.ProjectileType<SwingalingBlast>(), Projectile.damage, 1, Projectile.owner);
        }

        float maxTime = _charge * 60;
        if (Timer >= maxTime)
        {
            Projectile.Kill();
        }
    }

    private void AI_Charge()
    {
        Timer++;
        Vector2 mouseWorld = Main.MouseWorld;
        Vector2 directionToMouseWorld = Owner.Center.DirectionTo(mouseWorld);
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.ChangeDir(Projectile.direction);
            Projectile.velocity = directionToMouseWorld * ThrustDistance;
            Projectile.netUpdate = true;
        }

        float progress = Timer / ChargeTime;
        float easedProgress = Easing.OutCubic(progress);
        float rotation = Projectile.velocity.ToRotation();


        float holdRotation = rotation;
        Vector2 holdOffset = HoldOffset;


        Vector2 swingStart = playerCenter + holdOffset.RotatedBy(holdRotation);
        Vector2 swingEnd = playerCenter + Projectile.velocity + holdOffset.RotatedBy(rotation);
        Vector2 swingCenter = Vector2.Lerp(swingEnd, swingStart, easedProgress);

        Projectile.Center = swingCenter;

        if (Owner.direction == -1)
        {
            holdOffset.Y *= -1;
            rotation -= MathHelper.PiOver2;
        }
        Projectile.rotation = rotation + MathHelper.ToRadians(45) * Owner.direction;
        if (Owner.direction == -1)
        {
            Projectile.rotation -= MathHelper.Pi;
        }

        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        if (Main.rand.NextBool(7))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Scale: 0.5f);
        }

        _charge = MathHelper.Clamp(Timer / ChargeTime, 0f, 1f);
        if (Timer == ChargeTime)
        {
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightCyan, Color.DarkBlue, duration: 32);
            for (float f = 0; f < 10; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8));
                dp.outerColor = Color.DarkBlue;
                dp.Scale *= 0.5f;
            }
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Teleport");
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        if (Timer >= ChargeTime && Timer % 8 == 0)
        {
            Vector2 velocity = Main.rand.NextVector2CircularEdge(2, 2);
            var dp = DustParticle.Spawn(Projectile.Center, velocity);
            dp.outerColor = Color.DarkGray;
            dp.Scale *= 0.6f;
        }

        if (this.OwnedByLocalClient() && !Main.mouseRight)
        {
            if (Timer >= ChargeTime)
            {
                MaxCharge = true;
                Timer = 0;
                State = 1;
            }
            else if (Timer > ChargeTime / 2)
            {
                Timer = 0;
                State = 1;

            }
            else
            {
                Projectile.Kill();
            }
            Projectile.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case 0:
                AI_Charge();
                break;
            case 1:
                AI_Blast();
                break;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.VerticalFrame(1, 3);
        sbDrawer.color = Color.Lerp(Color.Transparent, Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 8)), _charge);
        Main.spriteBatch.Draw(sbDrawer);
        sbDrawer.color = Color.Lerp(Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 8)), Color.Transparent, _charge);
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer.VerticalFrame(2, 3);
        sbDrawer.color = Color.Lerp(Color.Transparent, Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 8)), _charge);
        Main.spriteBatch.Draw(sbDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Black, Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 8)), _charge) * 0.24f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.35f;
        Main.spriteBatch.Draw(glowDrawer);



        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
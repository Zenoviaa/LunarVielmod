using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Trailers;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Buffers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;



/// <summary>
/// Sword of the flies - Slashing sends baby ivyn flies to attack enemies, also is a slower sword
/// stamina(1) - Summons a big sword on top of the sword as it charges up and slashes down with a big slam and cool blue and yellow effects
/// </summary>
public class SwordoftheFlies : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 57;
        Item.shoot = ModContent.ProjectileType<SwordoftheFliesSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SwordoftheFliesSuperSword>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 4;
        staminaDamageMultiplier = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MarshScrap>());
    }
}


public class SwordofTheFliesStorm : ModProjectile,
    IDrawToRenderTarget
{
    private ref float HitCount => ref Projectile.ai[1];
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => ModContent.GetInstance<FlyStorm>().Texture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 10;
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.extraUpdates = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 240;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
            sound.PitchVariance = 0.4f;
            sound.Volume = 0.5f;
            SoundEngine.PlaySound(sound, Projectile.position);
        }


        if (Main.rand.NextBool(32))
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
        }

        if (Timer < 30)
            Projectile.velocity *= 0.88f;

        NPC nearest = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
        if(nearest != null && nearest.CanBeChasedBy())
        {
            Vector2 targetVelocity = (nearest.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 13;
            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 0.2f);
        }
        else
        {
            Projectile.velocity *= 0.92f;
        }
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
        Projectile.scale = 0.75f;
        Projectile.scale *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));

        Main.projFrames[Type] = 10;
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 6)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= 5)
            {
                Projectile.frame = 0;
            }
        }
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Black * EasingFunction.QuadraticBump(completionRatio);
    }

    private float GetTrailWidth(float completionRatio)
    {
        float outScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
        return EasingFunction.QuadraticBump(completionRatio) * 10 * outScale;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.VerticalFrame(Projectile.frame + 5, 10);
        sbDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 12, Projectile.identity);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }

    private void RenderFlyTrail(GraphicsDevice graphicsDevice)
    {
        var shader = BasicLaserAlphaShader.Instance;
        shader.BlendState = BlendState.AlphaBlend;
        shader.LaserTexture = TrailRegistry.LightningTrail2Outline;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        HitCount++;
        if (HitCount >= 2)
            Projectile.Kill();

    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderFlyTrail);

    }
}

public class SwordoftheFliesSlash : BaseSwingProjectileV2
{
    private float _oldRot;
    private float _traveledRotation;
    private bool _summonedFly;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SoundStyle swingSound1 = AssetRegistry.Sounds.Melee.NormalSwordSlash1;
        swingSound1.PitchVariance = 0.25f;
        swingSound1.Volume = 0.25f;

        SoundStyle swingSound2 = AssetRegistry.Sounds.Melee.NormalSwordSlash2;
        swingSound2.PitchVariance = 0.25f;

        SoundStyle swingSound3 = AssetRegistry.Sounds.Melee.SwordSpin1;
        swingSound3.PitchVariance = 0.5f;
        swingSound3.Volume = 0.5f;

        SoundStyle swingSoundAlt1 = AssetRegistry.Sounds.Melee.SwordSwing2;
        swingSoundAlt1.PitchVariance = 0.25f;

        SoundStyle swingSoundAlt2 = AssetRegistry.Sounds.Melee.SwordSwing3;
        swingSoundAlt2.PitchVariance = 0.25f;

        int style = 0;
        SoundStyle s1 = style == 0 ? swingSound1 : swingSoundAlt1;
        SoundStyle s2 = style == 0 ? swingSound2 : swingSoundAlt2;

        Add(new OvalSwing
        {
            Duration = 30,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 30,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });
        Add(new OvalSwing
        {
            Duration = 25,
            XSwingRadius = 88,
            YSwingRadius = 48,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = s1,
        });

        Add(new OvalSwing
        {
            Duration = 25,
            XSwingRadius = 88,
            YSwingRadius = 48,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });

        Add(new OvalSwing
        {
            Duration = 19,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 19,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });

        Add(new OvalSwing
        {
            Duration = 16,
            XSwingRadius = 88,
            YSwingRadius = 48,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = s1,
        });

        Add(new OvalSwing
        {
            Duration = 16,
            XSwingRadius = 88,
            YSwingRadius = 48,
            SwingDegrees = 270,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });

        Add(new OvalSwing
        {
            Duration = 13,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s1
        });

        Add(new OvalSwing
        {
            Duration = 13,
            XSwingRadius = 80,
            YSwingRadius = 48,
            SwingDegrees = 135,
            Easing = EasingFunction.InOutExpo,
            Sound = s2
        });

        Add(new OvalSwing
        {
            Duration = 35,
            XSwingRadius = 100,
            YSwingRadius = 40,
            SwingDegrees = 540,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound3
        });

        Add(new OvalSwing
        {
            Duration = 45,
            XSwingRadius = 100,
            YSwingRadius = 40,
            SwingDegrees = 540,
            Easing = EasingFunction.InOutExpo,
            Sound = swingSound3
        });

        swordBeamLength = 180;
        outlineColor = Color.Gold;
        additive = true;
        useAfterImage = true;
    //    hitStopTime = EXTRA_UPDATE_COUNT * 5;

    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.Orange, Color.Black, completionRatio);
    }
    private Color GetTrailColor2(float completionRatio)
    {
        Color trailColor = Color.Orange ;
        trailColor.A = 55;
       // trailColor.A = 0;
        return trailColor;
    }


    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(0, 24, completionRatio);
    }
    private float GetTrailWidth2(float completionRatio)
    {
        return GetTrailWidth(completionRatio) * 3;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlashForward.Asset.Value;
        shader.BloomColor = Color.Purple;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, shader);
    }

    public override void DrawSwingTrail2(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail2(ref lightColor, swingTrailCache);

        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserColor = Color.White;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor2, GetTrailWidth2, laserShader);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Goldenrod;
        b.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor2, GetTrailWidth2, b);
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Sword;
    }

    public override void AI()
    {
        base.AI();
        bloomScale = MathHelper.Lerp(0.08f, 0f, EasingFunction.InExpo(Interpolant));
        if (!_summonedFly && Interpolant > 0.5f && this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, -Projectile.velocity * 5, 
                ModContent.ProjectileType<SwordofTheFliesStorm>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack, Projectile.owner);
            _summonedFly = true;
        }

        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            SparkleParticle dp = SparkleParticle.Spawn(spawnPos, Vector2.Zero, Scale: 0.3f);
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.innerColor = Color.White;
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.fast = true;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;


            if (Main.rand.NextBool(12))
            {


                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.innerColor = Color.LightSkyBlue;
                dp.outerColor = Color.Violet;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        SoundStyle cleaveSound;
        int Sound = Main.rand.Next(2);
        if (Sound == 1)
        {
            cleaveSound = new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver1");
        }
        else
        {
            cleaveSound = new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver2");
        }

        cleaveSound = cleaveSound with { PitchVariance = 0.5f, Volume = 0.14f };
        SoundEngine.PlaySound(cleaveSound, Projectile.position);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
 

        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}

public class FliesSuperBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 444;
        Projectile.height = 444;
        Projectile.timeLeft = 60;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(explosionSound, Projectile.position);

            Color primaryColor = Main.rand.NextBool(2) ? Color.SkyBlue : Color.Goldenrod;
            Color drakColor = Color.Lerp(primaryColor, Color.Black, 0.5f);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, primaryColor, drakColor, duration: 24, baseSize: 0.2f);
            fx.Scale *= 1.8f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, primaryColor, drakColor, duration: 12, baseSize: 0.2f);
            fx2.Scale *= 3.8f;
            for (int i = 0; i < 32; i++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(48, 48));
                Color mainColor = Color.Lerp(Color.Blue, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 0, i));
                dp.innerColor = mainColor;
                dp.outerColor = Color.Lerp(mainColor, Color.Black, 0.5f);
                dp.dampening = 0.03f;
                dp.Scale *= 2;
                dp.superFast = true;
            }
            for (int i = 0; i < 14; i++)
            {
                var dp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(70, 70), Main.rand.NextVector2Circular(6, 6));
                Color mainColor = Color.Lerp(Color.Blue, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 0, i));
                dp.color = Color.Lerp(mainColor, Color.Black, 0.8f);
                dp.fadeToColor = Color.Black;
                dp.Scale *= 1;
                dp.behindLayer = true;
            }
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 12, 512);
            ShakeScreenPosition.Shake = 6;
            FXUtil.CreateRipple(Projectile.Center);

            if (this.OwnedByLocalClient())
            {
                for(int i = 0; i < 4; i++)
                {
                    Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 9);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel,
          ModContent.ProjectileType<SwordofTheFliesStorm>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 2.6f, EasingFunction.OutExpo(outRatio));
        waveDrawer.color = Color.SkyBlue;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
}

public class SwordoftheFliesSuperSword : BaseSwingProjectileV2
{
    private float _flashTimer;
    private float _traveledRotation;
    private float _oldRot;
    private bool _hit;
    public override void DefineCombo()
    {
        base.DefineCombo();

        SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
        swingSound1.PitchVariance = 0.5f;
        Add(new OvalSwing
        {
            Duration = 65,
            XSwingRadius = 64,
            YSwingRadius = 48,
            SwingDegrees = 330,
            Easing = EasingFunction.GreatswordAnticipation,
            Sound = swingSound1,
        });
        Add(new OvalSwing
        {
            Duration = 65,
            XSwingRadius = 64,
            YSwingRadius = 48,
            SwingDegrees = 330,
            Easing = EasingFunction.GreatswordAnticipation,
            Sound = swingSound1,
        });
        Add(new OvalSwing
        {
            Duration = 65,
            XSwingRadius = 64,
            YSwingRadius = 48,
            SwingDegrees = 330,
            Easing = EasingFunction.GreatswordAnticipation,
            Sound = swingSound1,
        });

        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        slashTrailBuilder.baseColor = Color.DarkBlue;
        slashTrailBuilder.windColor = Color.LightCoral;
        slashTrailBuilder.lightColor = Color.LightBlue;
        slashTrailBuilder.colorFunction = GetTrailColor;
        slashTrailBuilder.widthFunction = GetTrailWidth;
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;
        trailVisibilityOffset = 0.45f;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.SkyBlue;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;


        swordBeamLength = 333;
        glowAfterImageColor = Color.Violet * 0.1f;
        hitStopTime = EXTRA_UPDATE_COUNT * 8;
    }

    private Color PrimaryColor => Color.Lerp(Color.SkyBlue, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 32));

    private Color GetTrailColor(float completionRatio)
    {

        Color trailColor = Color.Lerp(PrimaryColor, Color.LightBlue, EasingFunction.InCirc(completionRatio)) * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
        trailColor = Color.Lerp(trailColor, PrimaryColor, _flashTimer / 180);
        return trailColor;
    }

    private Color GetTrailColor3(float completionRatio)
    {
        Color trailColor = Color.Lerp(PrimaryColor, Color.LightBlue, EasingFunction.InCirc(completionRatio));
        trailColor = Color.Lerp(trailColor, PrimaryColor, _flashTimer / 180);
        return trailColor;
    }

    private Color GetBloomTrailColor(float completionRatio)
    {
        return Color.White;
    }

    private float GetTrailWidth(float completionRatio)
    {
        if (Interpolant < 0.3f)
            return 0;
        return MathHelper.SmoothStep(32, 0, completionRatio);
    }

    private float GetBigTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(0, 200, completionRatio) * EasingFunction.QuadraticBump(Interpolant);
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetBigTrailWidth(ratio) * 1.2f;
    }

    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 64, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.DarkBlue, PrimaryColor, ratio) * MathHelper.SmoothStep(0f, 1f, ratio);
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Sword;
    }
    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        if (_hit)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.WhispyTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 1;
        shader.InnerColor = PrimaryColor;
        shader.OuterColor = Color.Lerp(PrimaryColor, Color.Black, 0.5f);
        spriteBatch.Restart(effect: shader.Effect);


        Asset<Texture2D> bladeAsset = TextureRegistry.GlowSword_Sword;
        float scale = 1.2f * EasingFunction.QuadraticBump(Interpolant);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(bladeAsset, Projectile.Center);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.rotation = Projectile.rotation + MathHelper.ToRadians(45);
        sbDrawer.color.A = 0;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);

        sbDrawer.scale *= 1.25f;
        sbDrawer.scale *= scale;
        spriteBatch.Draw(sbDrawer);
        spriteBatch.RestartDefaults();

        Asset<Texture2D> glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.color = Color.DarkBlue * ExtraMath.Osc(0.5f, 1f, speed: 2);
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Projectile.rotation - MathHelper.ToRadians(45);
        glowDrawer.scale.X *= 1;
        glowDrawer.scale.Y *= 0.5f;
        glowDrawer.scale *= scale;
        glowDrawer.worldPosition += (Projectile.rotation - MathHelper.ToRadians(45)).ToRotationVector2() * 192;
        spriteBatch.Draw(glowDrawer);
    }
    public override void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail(ref lightColor, swingTrailCache);
        if (Interpolant < 0.4f)
            return;
        if (_hit)
            return;
        Vector2[] swingPos = new Vector2[swingTrailCache.Length];
        for (int i = 0; i < swingPos.Length; i++)
        {
            ref Vector2 p = ref swingPos[i];
            p = swingTrailCache[i];
            Vector2 diff = (p - Owner.Center).SafeNormalize(Vector2.Zero);
            p += diff * 128;
        }

        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserColor = Color.White;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetTrailColor3, GetBigTrailWidth, laserShader);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = PrimaryColor;
        b.OuterColor = Color.Lerp(PrimaryColor, Color.Black, 0.5f);
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetBloomTrailColor, GetTrailWidth2, b);
    }

    public override void DrawSwingTrail2(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail2(ref lightColor, swingTrailCache);
        if (_hit)
            return;
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserColor = Color.SkyBlue;
        laserShader.InnerColor = Color.DarkBlue;
        laserShader.OuterColor = Color.Black;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor, GetBigTrailWidth, laserShader);
    }


    public override void AI()
    {
        base.AI();
        if(Timer == 1)
        {
            SoundStyle growSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Sun_Start") with { PitchVariance = 0.5f };
            SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Charge_TP_Out") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(chargeSound, Projectile.position);
            SoundEngine.PlaySound(growSound, Projectile.position);
            PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.White * 0.5f, Color.LightSkyBlue * 0.5f, 25, 196);
        }
        outlineColor = PrimaryColor;
        if (_flashTimer > 0)
            _flashTimer--;
 
        if (SwingDirection == 2)
        {

            swordBeamLength = 420;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;


        glowColor = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(Interpolant / 0.2f));
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            Vector2 spawnPos = swingTrailCache[index];
            if (SwingDirection == 2)
            {
                Vector2 diff = (spawnPos - Owner.Center);
                diff = diff.SafeNormalize(Vector2.Zero);
                spawnPos += diff * 64;
            }
            FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkBlue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            sp.Scale *= 2;
            sp.behindLayer = true;

            index = (int)(Interpolant * swingTrailCache.Length) % swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= swingTrailCache.Length;

            spawnPos = swingTrailCache[index];
            Vector2 spawnPos2 = swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f) * 0.25f;
                sp2.Scale *= 0.5f;
            }

            if (Main.rand.NextBool(8))
            {
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.innerColor = Color.LightBlue;
                dp.outerColor = Color.DarkSlateBlue;
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
                dp.Scale *= 0.6f;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle soundStyle = SoundID.DD2_WitherBeastCrystalImpact with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(soundStyle, target.position);
        if (!_hit)
        {
            _flashTimer = 180;
            ShakeScreenPosition.Shake = 4;
            FXUtil.GlowCircleBoom(target.Center, Color.Blue, Color.DarkBlue, Color.DarkBlue, duration: 30, baseSize: 0.2f);
            for (float n = 0; n < 8; n++)
            {
                DustParticle dp = DustParticle.Spawn(target.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 5f));
                dp.innerColor = Color.LightBlue;
                dp.outerColor = Color.DarkBlue;
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.1f;
                dp.Scale *= 0.85f;
            }


            if (ComboIndex < ComboCount && this.OwnedByLocalClient())
            {
                SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
                int combo = ComboIndex + 1;
                int dir = comboPlayer.ComboDirection;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Main.MouseWorld - Owner.Center, Projectile.type, Projectile.damage, Projectile.knockBack,
                            Projectile.owner, ai2: combo, ai1: dir);
                comboPlayer.IncreaseCombo();
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<FliesSuperBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;

        }
    }
}

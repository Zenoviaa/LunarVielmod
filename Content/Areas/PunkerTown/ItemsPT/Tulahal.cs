using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class Tulahal : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 60;
        Item.shoot = ModContent.ProjectileType<TulahalSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<TulahalThrow>();
        meleeWeaponType = MeleeWeaponType.Greatsword;
        staminaCost = 3;
        staminaDamageMultiplier = 5;
    }
    public override void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        //base.ShootSwing(player, source, position, velocity, type, damage, knockback);

        SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();

        int combo = comboPlayer.ComboCounter;
        if (combo == 1 || combo == 2 || combo == 3)
        {
            int style = 1;
            if (combo == 1)
                style = 2;
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(4)), staminaProjectileShoot, damage, knockback, player.whoAmI, ai1: style);
            comboPlayer.IncreaseCombo();
        }
        else
        {
            int dir = comboPlayer.ComboDirection;
            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            if (p.ModProjectile is BaseSwingProjectileV2 swingV2)
            {
                comboPlayer.IncreaseCombo();
            }
        }


    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankSword>();
    }
}

public class TulahalSlash : BaseSwingProjectileV2
{
    private float _flashTimer;
    private float _traveledRotation;
    private float _oldRot;
    private bool _hit;
    private NPCSucker _npcSucker;
    public override void DefineCombo()
    {
        base.DefineCombo();

        SwingV2Helper.AddGreatswordSwingStyle2(this);
        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        slashTrailBuilder.baseColor = Color.DarkRed;
        slashTrailBuilder.windColor = Color.LightCoral;
        slashTrailBuilder.lightColor = Color.Goldenrod;
        slashTrailBuilder.colorFunction = GetTrailColor;
        slashTrailBuilder.widthFunction = GetTrailWidth;
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        trailVisibilityOffset = 0.45f;

        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.Violet;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;

        swordBeamLength = 180;

        glowAfterImageColor = Color.Violet * 0.1f;
        //   useAfterImage = true;
        hitStopTime = EXTRA_UPDATE_COUNT * 8;
        //bigSwingTrailOffset = 132;
    }
    private Color GetTrailColor(float completionRatio)
    {
        Color trailColor = Color.Lerp(Color.Purple, Color.Red, EasingFunction.InCirc(completionRatio)) * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
        trailColor = Color.Lerp(trailColor, Color.Gold, _flashTimer / 180);
        return trailColor;
    }
    private Color GetTrailColor3(float completionRatio)
    {
        Color trailColor = Color.Lerp(Color.Purple, Color.Red, EasingFunction.InCirc(completionRatio));
        trailColor = Color.Lerp(trailColor, Color.Gold, _flashTimer / 180);
        return trailColor;
    }
    private Color GetTrailColor2(float completionRatio)
    {
        return Color.Lerp(Color.Purple, Color.Red, EasingFunction.InCirc(completionRatio)) * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
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
    private float GetBigTrailWidth2(float completionRatio)
    {
        return MathHelper.SmoothStep(0, 152, completionRatio) * EasingFunction.QuadraticBump(Interpolant);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetBigTrailWidth(ratio) * 1.2f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetBigTrailWidth(ratio) * 1f;
    }
    private float GetTrailWidth4(float ratio)
    {
        return GetBigTrailWidth(ratio) * 1.05f;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 64, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }
    private Color GetBloomColor(float ratio)
    {
        return Color.Lerp(Color.DarkRed, Color.Purple, ratio) * MathHelper.SmoothStep(0f, 1f, ratio);
    }

    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_TulaSword;
    }
    public override void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {
        base.PostDrawSword(position, srcRect, drawColor, rotation, origin, drawScale, spriteEffect, layerDepth);
        if (IsFinishingSwing())
        {
            SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Owner.Center);
            spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
            spiralVortexDrawer.color = Color.DarkViolet * 0.3f * EasingFunction.QuadraticBump(Interpolant);
            spiralVortexDrawer.color.A = 0;
            spiralVortexDrawer.scale *= 1.5f;
            Main.spriteBatch.Draw(spiralVortexDrawer);
        }
        if (SwingDirection != 2)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.WhispyTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.8f;
        shader.Tiling = Vector2.One * 1;
        shader.InnerColor = Color.IndianRed;
        shader.OuterColor = Color.DarkViolet;
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
        glowDrawer.color = Color.DarkViolet * ExtraMath.Osc(0.5f, 1f, speed: 2);
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
        /*
        FixedRichLaserShader laserShade2r = FixedRichLaserShader.Instance;
        laserShade2r.LaserColor = Color.White;
        laserShade2r.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShade2r.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor, GetBigTrailWidth, laserShader);
        */
        if (SwingDirection != 2)
            return;
        if (Interpolant < 0.4f)
            return;

        if (isAfterImageProjectile)
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
        b.InnerColor = Color.Violet;
        b.OuterColor = Color.DarkViolet;
        TrailDrawer.Draw(Main.spriteBatch, swingPos, GetBloomTrailColor, GetTrailWidth2, b);
    }

    public override void DrawSwingTrail2(ref Color lightColor, Vector2[] swingTrailCache)
    {
        base.DrawSwingTrail2(ref lightColor, swingTrailCache);
        if (ComboIndex == 6)
            return;
        if (isAfterImageProjectile)
            return;
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserColor = Color.IndianRed;
        laserShader.InnerColor = Color.DarkViolet;
        laserShader.OuterColor = Color.Black;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor, GetBigTrailWidth, laserShader);
    }


    public override void AI()
    {
        base.AI();
        if (_flashTimer > 0)
            _flashTimer--;
        if (Timer == 1 && SwingDirection == 2)
        {
            SoundStyle growSound = new SoundStyle("Stellamod/Assets/Sounds/FenixSummonGrav") with { Pitch = -0.3f };
            SoundEngine.PlaySound(growSound, Projectile.position);
        }
        if (SwingDirection == 2)
        {

            swordBeamLength = 420;
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;

        outlineColor = Color.Lerp(Color.Violet, Color.IndianRed, ExtraMath.Osc(0f, 1f, speed: 10));
        glowColor = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(Interpolant / 0.2f));
        growScale = MathHelper.Lerp(0f, 0.15f, EasingFunction.QuadraticBump(Interpolant));
        _npcSucker ??= new NPCSucker();


        if (SwingDirection == 2)
        {
            Owner.SetImmuneTimeForAllTypes(10);
            if (Owner.velocity.Y > 0)
                Owner.velocity.Y *= 0.98f;
        }
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
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.DarkViolet, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f)) * 0.25f;
            sp.Scale *= 0.48f;
            if (SwingDirection == 2)
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
                dp.innerColor = Color.IndianRed;
                dp.outerColor = Color.DarkRed;
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
                dp.Scale *= 0.6f;
            }

        }
        if (Interpolant > 0.5f)
        {
            _npcSucker.AI(Projectile.Center, strength: 0.8f);
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
            FXUtil.GlowCircleBoom(target.Center, Color.IndianRed, Color.DarkRed, Color.DarkViolet, duration: 30, baseSize: 0.2f);


            for (float n = 0; n < 8; n++)
            {
                DustParticle dp = DustParticle.Spawn(target.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 5f));
                dp.innerColor = Color.IndianRed;
                dp.outerColor = Color.DarkRed;
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.1f;
                dp.Scale *= 0.85f;
            }

            if (SwingDirection == 2)
            {
                SoundStyle e = new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity1") with { Pitch = -0.25f };
                SoundEngine.PlaySound(e, target.position);
                ShakeScreenPosition.Shake = 10;
                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                var fx = FXUtil.GlowCircleBoom(target.Center, Color.IndianRed, Color.DarkRed, Color.DarkViolet, duration: 30, baseSize: 0.2f);
                fx.Scale *= 2;
                for (float n = 0; n < 24; n++)
                {
                    DustParticle dp = DustParticle.Spawn(target.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 50));
                    dp.innerColor = Color.IndianRed;
                    dp.outerColor = Color.DarkRed;
                    dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                    dp.gravity = 0;
                    dp.noTileCollide = true;
                    dp.dampening = 0.1f;
                    dp.Scale *= 0.85f;
                }
            }
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

public class TulahalThrow : ModProjectile
{
    private bool _hit;
    private bool _playedSound;
    private float _flashTimer;
    private int _hitNPC;
    private Vector2 _stickOffset;
    private Vector2 _initialVelocity;
    private Asset<Texture2D> _chainTextureAsset;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private float ThrowDistance => 444;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 1;
    }

    private void AI_GrappleSwing()
    {
        if (_hit)
        {
            NPC npc = Main.npc[_hitNPC];
            if (npc.active)
            {
                Projectile.Center = (npc.Center + _stickOffset);
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                _stickOffset = Vector2.Zero;
                _hit = false;
            }
        }
        if (_flashTimer > 0)
            _flashTimer--;
        Timer++;
        if (Timer == 1)
        {
            _initialVelocity = Projectile.velocity;
            Projectile.velocity *= 6;
        }
        if (Projectile.velocity.Length() < 1)
        {
            if (_hit)
            {
                Owner.SetImmuneTimeForAllTypes(10);
            }
            if (!_playedSound)
            {
                SoundStyle pullSound = new SoundStyle("Stellamod/Assets/Sounds/CrossbowPull") with { PitchVariance = 0.4f };
                SoundEngine.PlaySound(pullSound, Projectile.position);
                _playedSound = true;
            }
            Owner.velocity = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0f, 36, EasingFunction.InExpo((Timer - 60f) / 20f));
            if (Timer >= 130 || (Vector2.Distance(Owner.Center, Projectile.Center) < 32))
            {
                Owner.velocity.Y = 0;
                Owner.velocity = -Owner.oldVelocity + Vector2.UnitY * -15;
                Projectile.Kill();
            }
        }
        if (Main.rand.NextBool(12))
        {
            DustParticle dp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 1f));
            dp.innerColor = Color.IndianRed;
            dp.outerColor = Color.DarkRed;
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.1f;
            dp.Scale *= 0.85f;
        }

        if (_hit)
            Projectile.velocity *= 0.7f;
        Projectile.velocity *= 0.92f;
        Projectile.rotation = _initialVelocity.ToRotation() + MathHelper.PiOver4;
    }

    private void AI_GrappleSlash()
    {
        if (_flashTimer > 0)
            _flashTimer--;
        Timer++;
        if (Timer == 1)
        {
            _initialVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            //            Projectile.velocity *= 6;
        }

        if (Main.rand.NextBool(12))
        {
            DustParticle dp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 1f));
            dp.innerColor = Color.IndianRed;
            dp.outerColor = Color.DarkRed;
            dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.1f;
            dp.Scale *= 0.85f;
        }

        float time = 90;
        if (Style == 2)
            time = 120;
        float ratio = Timer / time;
        float ease1 = EasingFunction.QuickOutSlowIn(ratio);
        float startup = 1f;
        if (Style == 2)
        {
            startup = MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(Timer / 45f));
        }
        Vector2 targetPosition = Vector2.Lerp(Owner.Center, Owner.Center + _initialVelocity * ThrowDistance, startup * ease1);
        Vector2 targetVelocity = (targetPosition - Projectile.Center);
        Projectile.velocity = targetVelocity;
        Projectile.rotation = _initialVelocity.ToRotation() + MathHelper.PiOver4;
        //  Owner.itemTime = 2;
        if (Timer >= time)
        {
            Projectile.Kill();
        }
    }

    public override void AI()
    {
        base.AI();
        switch (Style)
        {
            case 0:
                AI_GrappleSwing();
                break;
            case 1:
            case 2:
                AI_GrappleSlash();
                break;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            SoundStyle soundStyle = SoundID.DD2_WitherBeastCrystalImpact with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(soundStyle, target.position);

            SoundStyle hitSound = new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver1") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(hitSound, target.position);

            _stickOffset = (Projectile.Center - target.Center);
            _hitNPC = target.whoAmI;
            _flashTimer = 60f;
            ShakeScreenPosition.Shake = 4;
            FXUtil.GlowCircleBoom(target.Center, Color.IndianRed, Color.DarkRed, Color.DarkViolet, duration: 30, baseSize: 0.2f);
            for (float n = 0; n < 8; n++)
            {
                DustParticle dp = DustParticle.Spawn(target.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(0.5f, 5f));
                dp.innerColor = Color.IndianRed;
                dp.outerColor = Color.DarkRed;
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.1f;
                dp.Scale *= 0.85f;
            }

            var glowStretch = FXUtil.GlowStretch(target.Center, Projectile.velocity);
            glowStretch.OuterGlowColor = Color.DarkRed;
            _hit = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _chainTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Chain");
        float w = _chainTextureAsset.Width();
        if (w == 0)
            w = 1;
        float distanceToPlayer = Vector2.Distance(Projectile.Center, Owner.Center);
        float steps = distanceToPlayer / w;
        for (float s = 0; s < steps; s++)
        {
            Vector2 pos = Vector2.Lerp(Projectile.Center, Owner.Center, s / steps);
            SpritebatchDrawer chainDrawer = SpritebatchDrawer.FromTextureAsset(_chainTextureAsset, pos);
            Main.spriteBatch.Draw(chainDrawer);
        }


        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            SpritebatchDrawer afterImageDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afterImageDrawer.worldPosition = pos + Projectile.Size * 0.5f;
            afterImageDrawer.color = Color.Lerp(Color.Red, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.1f;
            afterImageDrawer.VerticalFrame(1, 3);
            Main.spriteBatch.Draw(afterImageDrawer);
        }

        SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(swordDrawer);



        swordDrawer.VerticalFrame(1, 3);
        swordDrawer.color = Color.Lerp(Color.Transparent, Color.Red, _flashTimer / 60f);
        Main.spriteBatch.Draw(swordDrawer);
        return false;
    }

    public override void Unload()
    {
        base.Unload();
        _chainTextureAsset = null;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient() && _hit)
        {
            SwingPlayerV2 comboPlayer = Owner.GetModPlayer<SwingPlayerV2>();
            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            int bigDir = 2;
            int ai2 = 5;
            if (Style == 1 || Style == 2)
            {
                return;
            }



            Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.UnitY.RotatedBy(Owner.direction * MathHelper.ToRadians(-30)), ModContent.ProjectileType<TulahalSlash>(), Projectile.damage, Projectile.knockBack,
                Owner.whoAmI, ai1: bigDir, ai2: ai2);
        }
    }
}
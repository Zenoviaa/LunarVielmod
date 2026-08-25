using Mono.Cecil;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Explosions;
using Stellamod.Visual.Particles;
using System;
using System.Buffers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceGreatBlade : ModProjectile
{
    private enum AIState
    {
        ChargeUp,
        HoldBack,
        Swing
    }
    public override string Texture => TextureRegistry.EmptyTexture;

    private ref float Timer => ref Projectile.ai[0];
    private ref float SwingDirection => ref Projectile.ai[1];
    private AIState State
    {
        get => (AIState)Projectile.ai[2];
        set => Projectile.ai[2] = (float)value;
    }
    private Player Owner => Main.player[Projectile.owner];
    private Vector2[] _swingTrailCache;
    private Vector2 _rotationalVelocity;
    private Vector2 _startProjectileCenter;
    private Vector2 _initialVelocity;
    private Vector2 _initialOffset;
    private float _startProjectileRotation;

    private OvalSwing _ovalSwing;

    private float _ratio;
    private float _bladeRatio;
    private float _rotationChargeOffset;
    public float _swordBeamLength;
    private bool _swordBeamedSound;
    private float _oldRot;
    private float _traveledRotation;
    private float _trailWidthLerp;
    private int _stage;
    private float _growTimer;
    private float _nextGrowPoint;


    public float chargeUpTime => 60  * fixer;
    public float holdBackTime => 60 * fixer;
    public float swingTime => 64 * fixer;
    public float holdOffset => _initialOffset.Length();
    public float swordOffset => _initialOffset.Length();
    public float fixer => Projectile.extraUpdates + 1;
    public float growUpTime => 30 * fixer;
    private SlashTrailer _bladeSlashes;
    private SlashTrailer _wideTrailer;
    private SlashTrailer _wideTrailer2;
    private SlashTrailer _auraTrailer;
    private float _flashTimer;
    private bool _invert;

    public float flashTime => 45 * fixer;

    public float flashRatio => _flashTimer / flashTime;
    public SlashTrailer BuildBladeSlashesTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutSine(interpolant) * 180 * _trailWidthLerp;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / flashTime;
            ratio = 1f - ratio;
            Color lerp1 = Color.Lerp(Color.White, Color.DarkGray, interpolant);
            Color lerp2 = Color.Lerp(Color.Transparent, lerp1, interpolant);
            return Color.Lerp(lerp2, Color.Black, 0.75f * ratio);
        }
        SlashEffect slashEffect = new SlashEffect();
        slashEffect.BaseColor = Color.White;
        slashEffect.HighlightColor = Color.White;
        slashEffect.RimHighlightColor = Color.DarkRed;
        slashEffect.WindColor = Color.SkyBlue;
        slashEffect.BlendState = BlendState.Additive;
        slashEffect.WindTexture = TrailRegistry.CausticTrail.Value;
        slashEffect.TrailTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin1.Value;
        slashEffect.HighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin2.Value;
        slashEffect.WindTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin3.Value;
        slashEffect.RimHighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin4.Value;

        SlashTrailer bladeSlashes = new SlashTrailer();
        bladeSlashes.Shader = slashEffect;
        bladeSlashes.TrailWidthFunction = GetTrailWidth;
        bladeSlashes.TrailColorFunction = GetTrailColor;
    
        bladeSlashes.invert = true;
        return bladeSlashes;
    }

    /// <summary>
    /// The large faint trail on this sword
    /// </summary>
    /// <returns></returns>
    public SlashTrailer BuildBladeSlashesWideTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutSine(interpolant) * 232 * _trailWidthLerp;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / flashTime;
            Color lerp1 = Color.Lerp(Color.White, Color.DarkGoldenrod, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant) * 0.7f * ratio;
        }
        SlashEffect slashEffect = new SlashEffect();
        slashEffect.BaseColor = Color.White;
        slashEffect.HighlightColor = Color.White;
        slashEffect.RimHighlightColor = Color.DarkGoldenrod;
        slashEffect.WindColor = Color.SkyBlue;
        slashEffect.BlendState = BlendState.Additive;
        slashEffect.WindTexture = TrailRegistry.CausticTrail.Value;
        slashEffect.TrailTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin1.Value;
        slashEffect.HighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin2.Value;
        slashEffect.WindTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin3.Value;
        slashEffect.RimHighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Thin4.Value;

        SlashTrailer bladeSlashes = new SlashTrailer();
        bladeSlashes.Shader = slashEffect;
        bladeSlashes.TrailWidthFunction = GetTrailWidth;
        bladeSlashes.TrailColorFunction = GetTrailColor;
        bladeSlashes.invert = true;
        if (_invert)
            bladeSlashes.invert = false;
        return bladeSlashes;
    }

    public SlashTrailer BuildAuraTrailer()
    {
        float GetTrailWidth(float interpolant)
        {
            return EasingFunction.QuadraticBump(interpolant) * 128;
        }
        Color GetTrailColor(float interpolant)
        {
            float ratio = _flashTimer / 120f;
            Color lerp1 = Color.Lerp(Color.White, Color.Goldenrod, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant) * ratio;
        }
        BlackFireShader blackFireShader = new BlackFireShader();
        blackFireShader.SetDefaults();
        blackFireShader.InnerColor = Color.Black;
        blackFireShader.OuterEmiteColor = Color.Black;
        blackFireShader.OuterColor = Color.Goldenrod;

        SlashTrailer slashTrailer = new SlashTrailer();
        slashTrailer.Shader = blackFireShader;
        slashTrailer.TrailWidthFunction = GetTrailWidth;
        slashTrailer.TrailColorFunction = GetTrailColor;
        return slashTrailer;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.
        float length = _swordBeamLength;
        float rotation = Projectile.rotation;
        rotation -= MathHelper.PiOver4;
        Vector2 start = Projectile.Center - rotation.ToRotationVector2() * length;
        Vector2 end = Projectile.Center + rotation.ToRotationVector2() * length;
        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 16, ref collisionPoint);
        return check;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 6000;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 8;
    }

    public override void AI()
    {
        base.AI();
        _bladeSlashes ??= BuildBladeSlashesTrailer();
        _wideTrailer ??= BuildBladeSlashesWideTrailer();
        _auraTrailer ??= BuildAuraTrailer();

        _invert = true;
        _wideTrailer2 ??= BuildBladeSlashesWideTrailer();
        
        _swordBeamLength = 256;
        _flashTimer--;
        if(_growTimer < growUpTime)
        {
            _growTimer++;
        }

        int denom = 8 * (Projectile.extraUpdates + 1);
        if (Timer % denom == 0)
        {
            Vector2 startPos = Projectile.Center;
            Vector2 endPos = startPos + _rotationalVelocity * 300;
            Vector2 spawnPos = Vector2.Lerp(startPos, endPos, Main.rand.NextFloat(0f, 1f));
        
            var sp = SirestiasSparkleParticle.Spawn(spawnPos + Main.rand.NextVector2Circular(80, 80), Vector2.Zero);
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.outerColor = Color.Yellow;
        }

        if(Timer % denom == 0)
        {
            Vector2 spawnPos = Projectile.Center;
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.8f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);


            Vector2 spawnPos2 = Projectile.Center + _rotationalVelocity * 300f;
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2 * (Projectile.extraUpdates + 1)))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f);
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1;
                sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = 0;
                sp2.noRot = true;
            }

        }


        switch (State)
        {
            case AIState.ChargeUp:
                AI_ChargeUp();
                break;
            case AIState.HoldBack:
                AI_HoldBack();
                break;
            case AIState.Swing:
                AI_Swing();
                break;
        }

        AI_OrientPlayer();
    }

    private void AI_OrientPlayer()
    {
        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));
    }

    private OvalSwing OvalSwing
    {
        get
        {
            if(_ovalSwing == null)
            {
                _swingTrailCache = ArrayPool<Vector2>.Shared.Rent(200);
                _ovalSwing = new OvalSwing
                {
                    Duration = swingTime,
                    XSwingRadius = 140,
                    YSwingRadius = 115,
                    SwingDegrees = 355,
                    Easing = EasingFunction.InOutExpo
                };
            }
            return _ovalSwing;
        }
    }

    private void AI_Swing()
    {
        if(Timer > swingTime * 0.35f)
        {
            Projectile.friendly = true;
        }
       


        if(_growTimer < growUpTime)
        {
            _growTimer++;
        }
        Timer++;
        if(Timer == 1)
        {
            _startProjectileCenter = Projectile.Center;
            _startProjectileRotation = Projectile.rotation;

        }
        float Interpolant = Timer / swingTime;
        Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
        _trailWidthLerp = EasingFunction.QuadraticBump(Interpolant);
        OvalSwing.SetDirection((int)SwingDirection);
        OvalSwing.UpdateSwing(Interpolant, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), out Vector2 swingOffset);

        Vector2 targetProjCenter = Owner.Center + swingOffset;
        float targetProjRot = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;


        float easeInStart = EasingFunction.InOutSine(Interpolant / 0.2f);
        //Vector2 adjustedProjCenter = Vector2.Lerp(_startProjectileCenter, targetProjCenter, easeInStart);
       // float adjustedProjRot = Utils.AngleLerp(_startProjectileRotation, targetProjRot, easeInStart);
        Projectile.Center = targetProjCenter;
        Projectile.rotation = targetProjRot;


        OvalSwing.CalculateTrailingPointsExtended(Interpolant, Projectile.velocity.SafeNormalize(Vector2.Zero), ref _swingTrailCache, 
            trailOffset: 2.4f);
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(Owner.Center.X, Owner.Center.Y, 0));
        //Now we transform the points
        //Calculating points locally and then translating it is a bit simpler.

        for (int t = 0; t < _swingTrailCache.Length; t++)
        {
            ref Vector2 point = ref _swingTrailCache[t];
            point = Vector2.Transform(point, translationMatrix);
        }


        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;
        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            int index = (int)(Interpolant * _swingTrailCache.Length) % _swingTrailCache.Length;
            Vector2 spawnPos = _swingTrailCache[index];
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 1.6f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            sp.behindLayer = false;

            index = (int)(Interpolant * _swingTrailCache.Length) % _swingTrailCache.Length;
            int nextIndex = index + 4;
            nextIndex %= _swingTrailCache.Length;

            spawnPos = _swingTrailCache[index];
            Vector2 spawnPos2 = _swingTrailCache[nextIndex];
            Vector2 spawnVelocity = spawnPos2 - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 24;

            if (Main.rand.NextBool(2))
            {
                Color color = new Color(41, 43, 66);
                var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
                sp2.color = Color.Lerp(color, Color.White, 0.25f);
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1.4f;
                sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = 0;
                sp2.noRot = true;
                sp2.behindLayer = false;
            }


            int denom = (int)MathHelper.Lerp(12, 4, flashRatio);
            if (Main.rand.NextBool(denom))
            {


                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
            }

        }

        if (_flashTimer > 0)
        {
            _flashTimer--;
        }
        if (Timer % 16 == 0)
        {
            int index = (int)(Interpolant * _swingTrailCache.Length) % _swingTrailCache.Length;
            Vector2 spawnPos = _swingTrailCache[index];
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.25f;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }
        if (Timer % 8 == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = (int)(Interpolant * _swingTrailCache.Length) % _swingTrailCache.Length;
                Vector2 spawnPos = _swingTrailCache[index];
                spawnPos += Main.rand.NextVector2Circular(32, 32);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }
            Vector2 dustPos = Projectile.Center + (Projectile.rotation+MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(1f, 256f);
            SirestiasSparkleAlphaParticle s2 = SirestiasSparkleAlphaParticle.SpawnInAlphaLayer(dustPos, Vector2.Zero);
            s2.gravity = 0;
            s2.noTileCollide = true;
            s2.Scale *= 0.25f;
            s2.fast = true;
            s2.color = Color.Lerp(Color.Black, Color.Blue, 0.15f);
        }


        if (Timer >= swingTime)
        {
            Projectile.Kill();
        }
    }

    private void AI_HoldBack()
    {
   
        Timer++;

        float time = holdBackTime;
        float ratio = Timer / time;

        if (ratio >= 0.4f && _stage == 1)
        {
            GrowUp();
        }
        ShakeScreenPosition.Shake = MathHelper.Lerp(1f, 4f, ratio);
        float dir = -Owner.direction;
        float inc = dir * 0.02f * MathHelper.Lerp(1f, 0f, ratio) * 1f / fixer;
        _rotationChargeOffset += inc;


        Vector2 vel = _rotationalVelocity;
        float mult = MathHelper.Lerp(1f, 1.2f, EasingFunction.QuadraticBump(_growTimer / growUpTime));



       Vector2 holdBackPos = Owner.Center + vel.SafeNormalize(Vector2.Zero).RotatedBy(_rotationChargeOffset) * mult * holdOffset;
        OvalSwing.SetDirection((int)SwingDirection);
        OvalSwing.UpdateSwing(0f, Projectile.Center, Projectile.velocity, out Vector2 swingStartOffset);
        Vector2 swingStartPos = Owner.Center + swingStartOffset;

        Vector2 swordHoldCenter = Vector2.Lerp(holdBackPos, swingStartPos, ratio);
        Projectile.Center = swordHoldCenter;
        Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;


        float denom = 54 * fixer;
        if(Timer >= holdBackTime -denom && !_swordBeamedSound)
        {
            _swordBeamedSound = true;
            SoundStyle sound = AssetRegistry.Sounds.Melee.ExcaliburHeavenlyStrike;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);
        }
        if(Timer >= holdBackTime)
        {
            SwitchState(AIState.Swing);
        }
    }


    private void AI_ChargeUp()
    {
        Timer++;
        if(Timer == 1)
        {
            _initialOffset = Projectile.Center - Owner.Center;
            SoundStyle sound = AssetRegistry.Sounds.Melee.WeaponSwordbigger;
            SoundEngine.PlaySound(sound, Projectile.position);
        }
        Vector2 initialVelocity = Projectile.Center - Owner.Center;
        initialVelocity = initialVelocity.SafeNormalize(Vector2.Zero);
        Vector2 targetVelocity = new Vector2(-1 * SwingDirection, -1);
        float time = chargeUpTime;
        _ratio = Timer / time;
        _bladeRatio = Timer / time;

        _ratio = MathHelper.Clamp(_ratio, 0f, 1f);
        _bladeRatio = MathHelper.Clamp(_bladeRatio, 0f, 1f);


        if (_ratio >= 0.8f && _stage == 0)
        {
            GrowUp();
        }
        _bladeRatio = EasingFunction.InOutSine(_bladeRatio);
        float ease = EasingFunction.InOutExpo(_ratio);
        _rotationalVelocity = initialVelocity;

        float dir = -Owner.direction;
        _initialOffset = _initialOffset.RotatedBy(dir * MathHelper.ToRadians(0.5f * 0.1f));

        Vector2 originalCenter = Owner.Center + _initialOffset ;
        //    Projectile.Center = Owner.Center + _rotationalVelocity.SafeNormalize(Vector2.Zero) * holdOffset * EasingFunction.InOutSine(_bladeRatio);
        Projectile.Center = originalCenter;
        Projectile.rotation = _rotationalVelocity.ToRotation() + MathHelper.PiOver4;
        if(Timer >= chargeUpTime + (30 * fixer))
        {
            SwitchState(AIState.HoldBack);
        }
    }
    private void SwitchState(AIState state)
    {
        State = state;
        Timer = 0;
        Projectile.netUpdate = true;
    }

    private void GrowUp()
    {
        _stage++;
        _growTimer = 0;
        SoundStyle sound = AssetRegistry.Sounds.Melee.WeaponSwordbigger;
        SoundEngine.PlaySound(sound, Projectile.position);

        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
        //   donut.Scale *= Projectile.scale;
        donut.Scale *= 3;
        donut.fadeToColor = Color.Goldenrod;
        donut.shrink = true;
        donut.noStretch = true;

        for(float f =0; f < 8f; f++)
        {
            Vector2 offset = Main.rand.NextVector2Circular(64, 64);
            Vector2 spawnPos = Projectile.Center + offset;
            Vector2 velocity = (Projectile.Center - spawnPos) * 0.2f;
            var fx = FXUtil.GlowStretch(spawnPos, velocity);
            fx.OuterGlowColor = Color.Goldenrod;
        }
        if (Main.netMode == NetmodeID.Server)
            return;

        ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.Goldenrod, 0.25f, 15);
    }
    private Asset<Texture2D> GetGlowSwordTexture()
    {
        switch (_stage)
        {
            default:
            case 0:
                return AssetManager.GlowMask.RomanceGlowSwordSmall;
            case 1:
                return AssetManager.GlowMask.RomanceGlowSwordMedium;
            case 2:
                return AssetManager.GlowMask.RomanceGlowSword;
        }
    }
    private void DrawPixelatedGlowSword(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float rotation = Projectile.rotation;
        if (SwingDirection == 1)
        {
            rotation -= MathHelper.PiOver2;
        }

        float ease = EasingFunction.InOutSine(_bladeRatio);
        Vector2 growScale = Vector2.Lerp(new Vector2(0f, 1f), Vector2.One, ease);
        float ease2 = EasingFunction.QuadraticBump(_growTimer / growUpTime);
        growScale *= Vector2.Lerp(Vector2.One, Vector2.One * 1.2f, ease2);
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BeamTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.DirnTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.3f;
        shader.Tiling = Vector2.One * 2;
        spriteBatch.Restart(effect: shader.Effect);

        Asset<Texture2D> glowSwordTextureAsset = GetGlowSwordTexture();
        SpritebatchDrawer glowSwordSprite = SpritebatchDrawer.FromTextureAsset(glowSwordTextureAsset, Projectile.Center);
        glowSwordSprite.rotation = rotation - MathHelper.PiOver4;
        glowSwordSprite.blackIsTransparency = true;
        glowSwordSprite.color = Color.White;
        glowSwordSprite.scale = growScale;
        glowSwordSprite.worldPosition += (Projectile.rotation-MathHelper.PiOver4).ToRotationVector2() * swordOffset * _bladeRatio;
       
        /*
        if(_swingTrailCache != null)
        {
            if (State == AIState.Swing)
            {
                for (int i = 0; i < _swingTrailCache.Length; i += 20)
                {
                    //   float oldRot = Projectile.oldRot[i];
                    Vector2 pos = _swingTrailCache[i];

                    glowSwordSprite.worldPosition = pos;
                    float r = (pos - Owner.Center).ToRotation();
                    glowSwordSprite.rotation = r;
                    float ratio = (float)i / (float)_swingTrailCache.Length;
                    ratio = 1f - ratio;
                    glowSwordSprite.color =
                        Color.Lerp(Color.White, Color.Goldenrod, ratio) * MathHelper.SmoothStep(1f, 0f, ratio) * 0.05f;
                    glowSwordSprite.scale = Vector2.Lerp(Vector2.One, Vector2.Zero, ratio) * new Vector2(1, 1f) * growScale;
                    spriteBatch.Draw(glowSwordSprite);
                }
            }
    
        }*/

        glowSwordSprite.scale =  growScale;
        glowSwordSprite.rotation = rotation - MathHelper.PiOver4;
        glowSwordSprite.worldPosition = Projectile.Center;
        glowSwordSprite.worldPosition += (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * swordOffset * _bladeRatio;
        spriteBatch.Draw(glowSwordSprite);

//        glowSwordSprite.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 4) * 12;
        glowSwordSprite.color = Color.Goldenrod;
        glowSwordSprite.scale *= 1.2f;
        glowSwordSprite.color *= 0.5f;
        spriteBatch.Draw(glowSwordSprite);
        spriteBatch.RestartDefaults();

    }
    private void DrawGlowSwordSprite(ref Color lightColor)
    {
        SpriteBatch spriteBath = Main.spriteBatch;

        float ease2 = EasingFunction.QuadraticBump(_growTimer / growUpTime);
        Vector2 scale = Vector2.Lerp(Vector2.One, Vector2.One * 1.2f, ease2);
        /*
        SpritebatchDrawer glowBallDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        glowBallDrawer.scale = new Vector2(0.75f, 0.2f) * _bladeRatio * scale; ;
        glowBallDrawer.rotation = Projectile.rotation + MathHelper.PiOver4;
        glowBallDrawer.color = Color.Goldenrod;
        glowBallDrawer.color *= 0.5f;
        glowBallDrawer.color.A = 0;
        glowBallDrawer.worldPosition += (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2()  * _bladeRatio; ;
        spriteBath.Draw(glowBallDrawer);




        glowBallDrawer.scale = new Vector2(0.66f, 0.2f) * _bladeRatio * scale; ;
        //glowBallDrawer.rotation = Projectile.rotation;
        glowBallDrawer.color = Color.White;
        glowBallDrawer.color *= 0.5f;
        glowBallDrawer.color.A = 0;
        spriteBath.Draw(glowBallDrawer);
        */


        /*
        glowBallDrawer.LeftCenterOrigin();
        glowBallDrawer.scale = new Vector2(2f, 0.2f);
        glowBallDrawer.worldPosition -= _rotationalVelocity * 249;
        glowBallDrawer.rotation -= MathHelper.PiOver2;
        glowBallDrawer.color = Color.White;
        glowBallDrawer.color.A = 0;
        spriteBath.Draw(glowBallDrawer);*/
    }

    private void DrawSwordSprite(ref Color lightColor)
    {
        float rotation = Projectile.rotation;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipVertically;
            rotation -= MathHelper.PiOver2;
        }

        SpriteBatch spriteBatch = Main.spriteBatch;

        Texture2D texture2 = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
        Texture2D texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture + "_Ascended").Value;
        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        int startY = frameHeight * Projectile.frame;

        Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
        Vector2 origin = sourceRectangle.Size() / 2f;
        Color drawColor = Color.White;


        Vector2 drawScale = Vector2.One;
        float swordRotation = rotation;

        Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

        spriteBatch.Draw(texture2, drawPosition,
            sourceRectangle, drawColor, rotation, origin, drawScale, spriteEffects, 0);


        float flashAlpha = _flashTimer / 120f;
        spriteBatch.Draw(texture, drawPosition,
            sourceRectangle, drawColor * flashAlpha, rotation, origin, drawScale, spriteEffects, 0);

        
        SpritebatchDrawer bloomSprite = 
            SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        bloomSprite.rotation = Projectile.rotation;
        bloomSprite.worldPosition += (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 90 * _bladeRatio; ;
        bloomSprite.blackIsTransparency = true;
        bloomSprite.color = Color.Goldenrod;
        bloomSprite.scale = new Vector2(2f, 0.5f) * _bladeRatio; ;
        bloomSprite.rotation -= MathHelper.PiOver4;
        spriteBatch.Draw(bloomSprite);
    }

    private void DrawSlashTrail(GraphicsDevice gDevice)
    {
        Color lightColor = Color.White;
        if (_auraTrailer == null || _wideTrailer == null)
            return;

        //   _auraTrailer.DrawTrail(ref lightColor, _swingTrailCache);
        //    _wideTrailer.DrawTrail(ref lightColor, _swingTrailCache);
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.OuterColor = Color.Lerp(Color.Black, Color.Goldenrod, flashRatio);
        laserShader.InnerColor = Color.Lerp(Color.Black, Color.LightGoldenrodYellow, flashRatio);
        //laserShader.LaserColor = Color.Lerp(Color.Black, Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 8) * 0.5f), flashRatio);
        laserShader.LaserTexture = TrailRegistry.BeamTrail;
        //laserShader.BloomTexture = TrailRegistry.BeamTrail;
        laserShader.Time = Main.GlobalTimeWrappedHourly * -64;
        TrailDrawer.Draw(Main.spriteBatch, _swingTrailCache, GetSlashTrailColor, GetSlashTrailWidth, laserShader);

        _bladeSlashes.DrawTrail(ref lightColor, _swingTrailCache);
        _wideTrailer.DrawTrail(ref lightColor, _swingTrailCache);
        _wideTrailer2.DrawTrail(ref lightColor, _swingTrailCache);
    }

    private void DrawSlashTrailBlack(GraphicsDevice gDevice)
    {

        BasicLaserAlphaShader smokeShader = BasicLaserAlphaShader.Instance;
        smokeShader.InnerColor = Color.Black;
        smokeShader.LaserTexture = AssetManager.LaserTextures.HeavenlySlashTrail;
        smokeShader.OuterColor = Color.Black;
        smokeShader.BlendState = BlendState.AlphaBlend;
        smokeShader.Tiling = new Vector2(1f, -1f);
        smokeShader.Time = 0;
        TrailDrawer.Draw(Main.spriteBatch, _swingTrailCache, GetBlackSlashTrailColor, GetBlackSlashTrailWidth, smokeShader);
    }


    private Color GetSlashTrailColor(float w)
    {
        Color slashColor = Color.Lerp(Color.White, Color.Black, w);
        slashColor = Color.Lerp(Color.Black,  Color.Goldenrod, flashRatio);
        return slashColor;
    }
    
    private float GetSlashTrailWidth(float w)
    {
        float Interpolant = Timer / swingTime;
        Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
        return  252 * MathHelper.Lerp(0, 1, EasingFunction.InOutSine(w)) * _trailWidthLerp;
    }
    private Color GetBlackSlashTrailColor(float w)
    {
        Color bColor = Color.Lerp(Color.Black, Color.Blue, 0.15f);
        return Color.Lerp(bColor, bColor, w);
    }

    private float GetBlackSlashTrailWidth(float w)
    {
        float Interpolant = Timer / swingTime;
        Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
        return 128 * MathHelper.Lerp(0, 1, EasingFunction.InOutSine(w)) * _trailWidthLerp;
    }


    public override bool PreDraw(ref Color lightColor)
    {
        if (State == AIState.Swing)
        {
            //   PixelationManager.QueuePrimitivesDrawAction(DrawGlowSwordPixelPrims);
            if (_swingTrailCache != null && Timer > 180)
            {
                PixelationManager.QueuePrimitivesDrawAction(DrawSlashTrail);
                PixelationManager.QueuePrimitivesDrawAction(DrawSlashTrailBlack);
            }

        }

        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedGlowSword);
        DrawGlowSwordSprite(ref lightColor);
        DrawSwordSprite(ref lightColor);
        return false;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (target.HasBuff<HeavenlyMark>())
        {
            _flashTimer = flashTime;
            target.DelBuff(target.FindBuffIndex(ModContent.BuffType<HeavenlyMark>()));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Smite>(), 
                Projectile.damage / 5, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);

            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(target.Center, Vector2.Zero, Color.Red);
            donut.Scale *= 2f;
            donut.fadeToColor = Color.Goldenrod;
            donut.noStretch = true;

            donut = LegacyParticle.NewParticle<GlowDonutParticle>(target.Center, Vector2.Zero, Color.Red);
            donut.Scale *= 0.5f;
            donut.fadeToColor = Color.Goldenrod;
            donut.noStretch = true;

            for(float f = 0; f < 12; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                Vector2 pos = target.Center;
                SparkleParticle sp = SparkleParticle.Spawn(pos, vel, Scale: 1f);
                sp.outerColor = Color.Goldenrod;
                sp.noTileCollide = true;
            }
            for (float f = 0; f < 12; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                Vector2 pos = target.Center;
                DustParticle sp = DustParticle.Spawn(pos, vel, Scale: 1f);
                sp.outerColor = Color.Goldenrod;
                sp.noTileCollide = true;
            }

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.ExcaliburHeavenlyExplosions;
            hitSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hitSound, target.Center);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<HeavenlyCrashBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<HolyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            var boom = FXUtil.GlowCircleBoom(target.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
            boom.Scale *= 2;
            for (float f = 0f; f < 8f; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                Vector2 pos = target.Center;
                var ds = DustParticle.Spawn(pos, vel);
                ds.noTileCollide = true;
                ds.outerColor = Color.Yellow;
            }
            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = target.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 velocity = (pos - target.Center).SafeNormalize(Vector2.Zero) * 32;
                var fx = FXUtil.GlowStretch(pos, velocity);
                fx.OuterGlowColor = Color.Goldenrod;
            }

        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if(_swingTrailCache != null)
        {
            ArrayPool<Vector2>.Shared.Return(_swingTrailCache);
        }
    }
}

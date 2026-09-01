using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class BouncingRazorSuns : ModProjectile,
    IDrawToRenderTarget
{
    private LazyAsset<Texture2D> _altTexture;
    private LazyAsset<Texture2D> _auraTexture;

    private enum AIState
    {
        Come_In,
        Bounce,
        Chase
    }

    private float _inTimer;
    private float _squishTimer;
    private Vector2 _offset;
    private Vector2 _startOffset;
    private Vector2 _startPoint;
    private AnimationFramer _discAnimationFrame;
    private AnimationFramer _auraAnimationFrame;
    private float InScale
    {
        get
        {
            return EasingFunction.InOutSine(_inTimer / 60f);
        }
    }

    private Vector2 DrawScale
    {
        get
        {
            return Vector2.One * 1.4f * MathHelper.Lerp(1f, 1.4f, EasingFunction.OutExpo(_squishTimer / 60f)) * InScale;
        }
    }

    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get
        {
            return (AIState)Projectile.ai[1];
        }
        set
        {
            Projectile.ai[1] = (float)value;
        }
    }

    private ref float Variant => ref Projectile.ai[2];
    private float _deadAlpha;
    private float InTime => 100;
    private float BounceTime => 190;
    private float AnticipationTime => 190;

    public float orbitSpeed;
    public float fastAnimateTimer;
    public override void Unload()
    {
        base.Unload();
        _altTexture?.Unload();
        _auraTexture?.Unload();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startPoint);
        writer.Write(_squishTimer);
        writer.Write(orbitSpeed);
        writer.Write(fastAnimateTimer);
        writer.WriteVector2(_startOffset);
        writer.WriteVector2(_offset);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startPoint = reader.ReadVector2();
        _squishTimer = reader.ReadSingle();
        orbitSpeed = reader.ReadSingle();
        fastAnimateTimer = reader.ReadSingle();
        _startOffset = reader.ReadVector2();
        _offset = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //Circular hitbox
        float collisionRadius = 124;
        Vector2 centerPoint = targetHitbox.Center();
        Vector2 myPoint = projHitbox.Center();
        return Vector2.Distance(myPoint, centerPoint) <= collisionRadius;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.hostile = false;
        Projectile.timeLeft = 930;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    private void MakeParticles()
    {
        if (Main.rand.NextBool(2))
        {
            DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(90, 90), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
            sp.gravity = 0f;
            sp.fast = true;
            sp.dampening = 0.1f;
            sp.Scale *= 1.5f;
            sp.innerColor = Main.rand.NextBool(2) ? Color.White : GetDiscAuraColor();
            sp.outerColor = Color.Lerp(sp.innerColor, Color.Black, 0.5f);
        }
    }
    public override void AI()
    {
        base.AI();
        _inTimer++;
        if (_squishTimer > 0)
            _squishTimer--;
        if (fastAnimateTimer > 0)
            fastAnimateTimer--;
        _discAnimationFrame.frameSpeed = 2;
        if (fastAnimateTimer > 0)
            _discAnimationFrame.frameSpeed = 1;
        _discAnimationFrame.maxFrame = 60;
        _discAnimationFrame.UpdateTick();

        _auraAnimationFrame.frameSpeed = 1;
        _auraAnimationFrame.maxFrame = 90;
        _auraAnimationFrame.UpdateTick();

        if (!NPC.AnyNPCs(ModContent.NPCType<Gothivia>()))
            Projectile.Kill();
        MakeParticles();
        switch (State)
        {
            case AIState.Come_In:
                AI_ComeIn();
                break;
            case AIState.Bounce:
                AI_Bounce();
                break;
            case AIState.Chase:
                // Projectile.hostile = true;
                AI_Chase();
                break;
        }
        _offset = _offset.RotatedBy(orbitSpeed);
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void AI_ComeIn()
    {
        Timer++;
        if (Timer == 1)
        {
            _offset = Vector2.UnitY * 512;
            if (Variant == 1)
            {
                _offset *= -1;
            }
            _startOffset = _offset;
            _startPoint = Projectile.Center;
        }

        float ratio = Timer / InTime;
        orbitSpeed = MathHelper.Lerp(0, 0.1f, EasingFunction.QuadraticBump(ratio));
        orbitSpeed *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(ratio));
        Vector2 offset = _offset;
        Vector2 o = Vector2.Lerp(offset * 0.1f, offset * 1f, EasingFunction.InOutSine(ratio));


        Vector2 targetPoint = _startPoint + Projectile.velocity;

        Vector2 pos = targetPoint + o;
        Projectile.Center = pos;
        if (Timer >= InTime)
        {
            Projectile.velocity = Vector2.Zero;
            //  Projectile.velocity = (_startPoint - targetPoint).SafeNormalize(Vector2.Zero) * 16;
            SwitchState(AIState.Chase);
        }
    }

    private void AI_Bounce()
    {
        Timer++;
        if (Timer == 1)
        {
            fastAnimateTimer = 30;
            _squishTimer = 60;
            ShakeScreenPosition.Shake = 4;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") { PitchVariance = 0.5f }, Projectile.Center);

            if (MultiplayerHelper.IsHost && Variant == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RazorFireBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        Timer++;

        float ratio = Timer / BounceTime;
        float ease = EasingFunction.InOutSine(ratio);

        float maxDetectDistance = 4000;
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
        if (player != null)
        {
            _startPoint = player.Center;
        }

        orbitSpeed = MathHelper.Lerp(0.05f, 0.01f, EasingFunction.InOutExpo(ratio));
        Vector2 offset = _offset;
        Vector2 o = Vector2.Lerp(offset * 0.15f, offset, EasingFunction.OutExpo(ratio));
        Projectile.velocity = Vector2.Zero;
        Projectile.Center = _startPoint + o;
        _deadAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        if (Timer >= BounceTime)
        {
            //   Main.NewText("Chase");
            //_startPoint = Projectile.Center;
            //_startOffset = _startOffset.RotatedBy(radians);
            SwitchState(AIState.Chase);
        }
    }

    private void AI_Chase()
    {
        float maxDetectDistance = 4000;
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
        if (player == null)
            return;


        Timer++;
        float maxRadians = MathHelper.TwoPi;
        float ratio = Timer / AnticipationTime;
        float ease = EasingFunction.InOutSine(ratio);
        float radians = ease * maxRadians;
        _deadAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InSine(ratio));
        orbitSpeed = MathHelper.Lerp(0.05f, 0f, EasingFunction.InExpo(ratio));

        Vector2 offset = _offset;
        Vector2 o = Vector2.Lerp(offset, offset * 0.05f, EasingFunction.InExpo(ratio * ratio * ratio * ratio));
        Projectile.velocity = Vector2.Zero;

        Vector2 targetPoint = player.Center + o;
        Vector2 proposedPoint = Vector2.Lerp(Projectile.Center, targetPoint, MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(ratio)));
        Projectile.Center = proposedPoint;

        if (Timer >= AnticipationTime)
        {
            SwitchState(AIState.Bounce);
        }
    }

    private Asset<Texture2D> GetDiscTextureAsset()
    {
        switch (Variant)
        {
            default:
            case 0:
                return TextureAssets.Projectile[Type];
            case 1:
                return _altTexture;
        }
    }

    private Color GetDiscAuraColor()
    {
        switch (Variant)
        {
            default:
            case 0:
                return Color.Aqua;
            case 1:
                return Color.Orange;
        }
    }
    private Color GetDiscAuraColor2()
    {
        switch (Variant)
        {
            default:
            case 0:
                return Color.DarkGreen;
            case 1:
                return Color.DarkGoldenrod;
        }
    }
    private Color GetDiscAuraColor3()
    {
        switch (Variant)
        {
            default:
            case 0:
                return Color.LightGreen;
            case 1:
                return Color.Red;
        }
    }

    private void LoadTextures()
    {
        _altTexture ??= new LazyAsset<Texture2D>($"{Texture}_Alt");
        _auraTexture ??= new LazyAsset<Texture2D>($"{Texture}_Aura");
    }
    public override bool PreDraw(ref Color lightColor)
    {

        DrawAura();
        DrawDisc();
        return false;
    }

    private void DrawAura()
    {
        LoadTextures();
        SpriteBatch spriteBatch = Main.spriteBatch;
        Asset<Texture2D> textureAsset = _auraTexture;
        SpritebatchDrawer discDrawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Projectile.Center);
        Rectangle frame = textureAsset.Value.GetFrame(_auraAnimationFrame.frame, horizontalFrameCount: 5, verticalFrameCount: 18);
        discDrawer.sourceRect = frame;
        discDrawer.color = GetDiscAuraColor() * ExtraMath.Osc(0.3f, 1f, speed: 24) * (1f - _deadAlpha); ;
        discDrawer.color.A = 0;
        discDrawer.rotation = Main.GlobalTimeWrappedHourly * 2;
        discDrawer.scale = DrawScale * 1.5f;
        discDrawer.CenterOrigin();
        spriteBatch.Draw(discDrawer);
    }

    private void DrawDisc(Color? overrideColor = null)
    {
        LoadTextures();
        SpriteBatch spriteBatch = Main.spriteBatch;
        Asset<Texture2D> textureAsset = GetDiscTextureAsset();
        SpritebatchDrawer discDrawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Projectile.Center);
        Rectangle frame = textureAsset.Value.GetFrame(_discAnimationFrame.frame, horizontalFrameCount: 5, verticalFrameCount: 12);
        discDrawer.sourceRect = frame;
        discDrawer.scale = DrawScale;
        discDrawer.color = overrideColor != null ? overrideColor.Value : discDrawer.color;
        discDrawer.CenterOrigin();

        if (overrideColor == null)
        {
            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowDrawer.color = GetDiscAuraColor() * 0.5f;
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 1.5f;
            spriteBatch.Draw(glowDrawer);
            SpriteWhiteShader whiteShader = ShaderContent.GetInstance<SpriteWhiteShader>();
            spriteBatch.Restart(effect: whiteShader.Effect);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = i / (float)Projectile.oldPos.Length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                var drawer = discDrawer;
                drawer.worldPosition = pos;
                drawer.color = Color.Lerp(GetDiscAuraColor(), Color.Black, ratio) * 0.04f;
                drawer.color.A = 0;
                spriteBatch.Draw(drawer);
            }

            spriteBatch.RestartDefaults();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = i / (float)Projectile.oldPos.Length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                var drawer = discDrawer;
                drawer.worldPosition = pos;
                drawer.color = Color.Lerp(Color.White, Color.Transparent, EasingFunction.OutExpo(ratio)) * 0.3f;
                spriteBatch.Draw(drawer);
            }
        }

        discDrawer.color = Color.Lerp(discDrawer.color, Color.Lerp(Color.White, Color.Black, 0.9f), _deadAlpha);
        spriteBatch.Draw(discDrawer);
        discDrawer.color = Color.White * ExtraMath.Osc(0.1f, 0.5f, speed: 24) * (1f - _deadAlpha);
        discDrawer.color.A = 0;
        spriteBatch.Draw(discDrawer);

        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            float e = f;
            e += Main.GlobalTimeWrappedHourly * 5;
            // f +=
            Vector2 offset = e.ToRotationVector2();
            discDrawer.worldPosition = Projectile.Center + offset * 32;
            discDrawer.color = Color.White * ExtraMath.Osc(0.1f, 0.5f, speed: 24) * 0.12f * (1f - _deadAlpha);
            discDrawer.color.A = 0;
            spriteBatch.Draw(discDrawer);
        }
    }

    private void DrawOutline(SpriteBatch spriteBatch)
    {
        LoadTextures();
        Color outlineColor = Projectile.hostile ? Color.Red : Color.Yellow;
        DrawDisc(outlineColor);
    }

    private void DrawRippingTrail(GraphicsDevice gDevice)
    {
        Color primaryColor = GetDiscAuraColor();
        Color darkerColor = Color.Lerp(GetDiscAuraColor2(), Color.Black, 0f);
        BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
        bloomShader.Time = Main.GlobalTimeWrappedHourly * 50;
        bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
        bloomShader.InnerColor = primaryColor;
        bloomShader.OuterColor = darkerColor;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = GetDiscAuraColor3();
        basicLaserShader.OuterColor = GetDiscAuraColor3();
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor2, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, basicLaserShader, Projectile.Size * 0.5f);
    }

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio));
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(172, 172, completionRatio) * InScale;
    }

    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = ShaderContent.GetInstance<BasicLaserShader>();
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        Color primaryColor = Color.Lerp(GetDiscAuraColor(), Color.White, 0.25f);
        laserShader.InnerColor = primaryColor;
        laserShader.OuterColor = Color.Lerp(primaryColor, Color.Black, 0.25f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
    }

    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(120, 96, completionRatio);
    }
    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f * InScale;
    }
    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        Color secondaryLerp = Color.Lerp(GetDiscAuraColor2(), Color.Black, completionRatio);
        return Color.Lerp(GetDiscAuraColor(), secondaryLerp, completionRatio);
    }

    private Color GetSpiralDashTrailColor2(float completionRatio)
    {
        Color secondaryLerp = Color.Lerp(GetDiscAuraColor3(), Color.Black, completionRatio);
        return Color.Lerp(GetDiscAuraColor2(), secondaryLerp, completionRatio);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        Color primaryColor = GetDiscAuraColor();
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, primaryColor, Color.Lerp(primaryColor, Color.Black, 0.5f), duration: 25, baseSize: 0.23f);
        fx.Scale *= 1.8f;
        for (float f = 0; f < 10; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
            dp.Scale *= 1.5f;
            dp.gravity = 0.05f;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.outerColor = primaryColor;
        }
        ShakeScreenPosition.Shake = 6;
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutline);
        PixelationManager.QueuePrimitivesDrawAction(DrawRippingTrail);
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
    }

}


public class RazorFireBoom : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private float Time => 45;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Time;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }
    public override void AI()
    {
        base.AI();


        if (Timer > 27)
        {
            Projectile.hostile = false;
        }
        Timer++;
        if (Timer == 1)
        {
            FXUtil.CreateRipple(Projectile.Center);
            ShakeScreenPosition.Shake = 6;
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.LightGoldenrodYellow, Color.OrangeRed, 55, 450);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Red, Color.Black, duration: 25, baseSize: 0.2f);
            fx.Scale *= 3f;


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.VectorScale *= 4;
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (float f = 0; f < 14; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(32, 32));
                dp.dampening = 0.05f;
                dp.gravity *= 0.05f;
                dp.Scale *= 2;
            }
        }
        var dp2 = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(384, 384), Vector2.Zero);
        dp2.dampening = 0.05f;
        dp2.gravity *= 0.05f;
        dp2.fast = true;
    }

    private void DrawPixelatedBoom(SpriteBatch sb, Vector2 screenPos)
    {
        Asset<Texture2D> noiseTextureAsset = AssetManager.Noise.FlamethrowerNoise;
        FlameyBoomShader boomShader = ShaderContent.GetInstance<FlameyBoomShader>();
        float t = Timer / Time;
        boomShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        boomShader.Time = EasingFunction.OutSine(t);
        boomShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, t);
        boomShader.BloomColor = Color.Lerp(Color.Red, Color.DarkRed, t);

        sb.Restart(effect: boomShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.InvertedVoronoi.Asset.Value, Projectile.Center);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 1, EasingFunction.OutQuad(t)) * 1.5f;
        sb.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.FlamethrowerNoise, Projectile.Center);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 1, EasingFunction.OutQuad(t)) * 12;
        sb.Draw(drawer);
        sb.RestartDefaults();
    }
    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBoom);
    }
}
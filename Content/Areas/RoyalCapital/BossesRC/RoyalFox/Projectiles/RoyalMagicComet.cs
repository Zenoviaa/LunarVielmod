using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;


[Autoload(Side = ModSide.Client)]
public class RoyalMagicCometStarsRenderer : ModSystem
{
    private Asset<Texture2D> _royalSmokeMaskTextureAsset;
    private RenderTargetProvider _starsRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _maskRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private readonly RoyalMagicRenderer.Particles _particles = new(252);
    private bool _activeParticles;
    public override void Load()
    {
        base.Load();
        _royalSmokeMaskTextureAsset = ModContent.Request<Texture2D>("Stellamod/Effects/RoyalMagic/RoyalSmokeMask");
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderStars;
    }
    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderStars;
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        SimulateParticles();
    }


    public void SpawnParticle(Vector2 position, Vector2 velocity, float timeLeft)
    {
        int freeIndex = -1;
        for (int i = 0; i < _particles.Length; i++)
        {
            ref float t = ref _particles.timeleft[i];
            if (t <= 0)
            {
                freeIndex = i;
                break;
            }
            //velocity *= 0.98f;
        }

        if (freeIndex == -1)
            return;

        _particles.timeleft[freeIndex] = timeLeft;
        _particles.position[freeIndex] = position;
        _particles.velocity[freeIndex] = velocity;
    }

    private void SimulateParticles()
    {
        _activeParticles = false;
        for (int i = 0; i < _particles.Length; i++)
        {
            ref float timeLeft = ref _particles.timeleft[i];
            if (timeLeft <= 0)
                continue;
            timeLeft--;

            ref Vector2 position = ref _particles.position[i];
            ref Vector2 velocity = ref _particles.velocity[i];
            position += velocity;
            velocity *= 0.96f;
            _activeParticles = true;
        }
    }

    private void DrawMaskParticles(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_royalSmokeMaskTextureAsset, Vector2.Zero);
        for (int i = 0; i < _particles.Length; i++)
        {
            ref float timeLeft = ref _particles.timeleft[i];
            if (timeLeft <= 0)
                continue;
            ref Vector2 position = ref _particles.position[i];
            maskDrawer.worldPosition = position;
            maskDrawer.VerticalFrame(i % 4, 4);
            maskDrawer.CenterOrigin();
            maskDrawer.rotation = Main.GlobalTimeWrappedHourly + i * 3;
            float progress = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(timeLeft / 60f));
            Color brighterColor = Color.White;
            maskDrawer.color = Color.White;
            float offset = MathHelper.Lerp(0.8f, 1f, MathHelper.Lerp(0f, 1f, (i % 8f) / 8f));
            maskDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * offset, progress);
            spriteBatch.Draw(maskDrawer);
        }
    }

    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }

    private void RenderStars()
    {

        if (!_activeParticles)
            return;

        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        SpriteBatch sb = Main.spriteBatch;
        gDevice.SetRenderTarget(_maskRT);
        gDevice.Clear(Color.Transparent);

        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null);
        DrawMaskParticles(sb);
        sb.End();

        gDevice.SetRenderTarget(_starsRT);
        gDevice.Clear(Color.Transparent);


        RoyalMagicStarsShader starsShader = ShaderContent.GetInstance<RoyalMagicStarsShader>();
        starsShader.Time = Main.GlobalTimeWrappedHourly * 4;
        starsShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        starsShader.ScreenOffset = GetScreenOffset(scale: 1);

        sb.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointWrap,
            DepthStencilState.None,
            RasterizerState.CullNone,
            starsShader.Effect);

        sb.Draw(AssetManager.Noise.CometStars.Value, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Blue);

        sb.End();
        PixelationManager.QueueSpritebatchDrawAction(DrawToScreen, DrawLayer.BehindNPCsWithOutline);
    }

    private void DrawToScreen(SpriteBatch sb, Vector2 screenPos)
    {

        MaskCombineShader starMix = ShaderContent.GetInstance<MaskCombineShader>();
        starMix.MixTexture = _starsRT;
        sb.Restart(effect: starMix.Effect);
        sb.Draw(_maskRT, Vector2.Zero, Color.White);
        sb.RestartDefaults();


    }
}


public class RoyalMagicComet : ModProjectile,
    IDrawToRenderTarget
{
    private float _alpha;

    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];

    private ref float Size => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 360;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                Size = Main.rand.NextFloat(0.66f, 1.2f);
                Style = Main.rand.Next(3);
                Projectile.netUpdate = true;
            }
        }

        Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        if (screenRect.Contains(Projectile.position.ToPoint()))
        {
            if (Timer % 4 == 0)
            {
                RoyalFox.SpawnCometStarParticle(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f), 60);
            }

            if (Timer % 8 == 0)
            {
                var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                sp.outerColor = Color.SkyBlue;
                sp.gravity = 0.05f;
                sp.noTileCollide = true;
            }

            if (Main.rand.NextBool(5))
            {
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 7));
                dp.outerColor = Color.Blue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.1f;
                dp.superFast = true;
            }

            if (Main.rand.NextBool(15))
            {
                LightningSparkParticle dp = Particle<LightningSparkParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8),
                    color: Color.Blue,
                    Scale: Main.rand.NextFloat(0.2f, 0.35f));
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                dp.fast = true;
            }
        }

        float targetAlpha = Projectile.hostile ? 1f : 0.35f;
        _alpha = MathHelper.Lerp(_alpha, targetAlpha, 0.2f);
        float ratio = (Size - 0.66f) / 0.66f;
        float gravity = MathHelper.SmoothStep(1f, 0.1f, ratio);
        if (Projectile.velocity.Y < 20)
            Projectile.velocity.Y += gravity;
        Projectile.velocity.X *= 0.98f;
        if (Projectile.velocity.Y > 0 && Timer > 55)
            Projectile.hostile = true;
        Projectile.scale = ExtraMath.Osc(0.5f, 0.75f, speed: 3, Projectile.whoAmI);
    }


    private Color GetCometColor()
    {
        switch (Style)
        {
            default:
            case 0:
                return Color.Blue;
            case 1:
                return Color.SkyBlue;
            case 2:
                return Color.Pink;
        }
    }
    private Color StarryTrailColorFunction(float completionRatio)
    {
        Color trailColorFunction = Color.Lerp(Color.White, GetCometColor(), completionRatio) * 0.3f * MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(completionRatio));
        trailColorFunction *= Alpha;
        trailColorFunction.A = 0;
        return trailColorFunction;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(36, 24, EasingFunction.InOutSine(completionRatio)) * Size;
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        CometTrailShader cometTrail = ShaderContent.GetInstance<CometTrailShader>();
        cometTrail.BloomColor = GetCometColor() * Alpha;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);


        cometTrail.LaserTexture = TrailRegistry.VortexTrail;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);
    }

    private float Alpha => _alpha;
    public override bool PreDraw(ref Color lightColor)
    {

        float alpha = Alpha;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        drawer.scale *= 0.4f;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)Projectile.oldPos.Length;
            drawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            drawer.color = Color.Lerp(Color.White, Color.Black, ratio) * 0.025f * alpha;
            drawer.color.A = 0;


            drawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio) * 0.4f * Size;
            Main.spriteBatch.Draw(drawer);
        }
        drawer.scale = Vector2.One * 0.4f * Size;
        drawer.color = Color.Blue * 0.35f * ExtraMath.Osc(0.75f, 1f, speed: 18, offset: Projectile.whoAmI) * alpha;
        drawer.color.A = 0;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Color.White * alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.6f;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);
        //    Main.spriteBatch.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        drawer.color = Color.White * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 9, Projectile.whoAmI) * alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.4f * Size;
        Main.spriteBatch.Draw(drawer);

        SpritebatchDrawer blackStar = SpritebatchDrawer.FromProjectile(Projectile);
        blackStar.color = Color.Black * alpha;
        blackStar.scale = Vector2.One * ExtraMath.Osc(0.75f, 1f, speed: 9, Projectile.whoAmI) * 0.6f * Size;
        Main.spriteBatch.Draw(blackStar);


        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Violet, 30, Main.rand.NextFloat(100, 200));
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.DarkGray, Color.DarkViolet, duration: 45, baseSize: Main.rand.NextFloat(0.06f, 0.24f));
        for (float n = 0; n < 8; n++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
            dp.outerColor = Color.DarkGray;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.noTileCollide = true;
        }
        ImpactEffect();
        if (Main.netMode == NetmodeID.Server)
            return;


        for (float f = 0; f < 4; f++)
        {
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
            Vector2 vel = Main.rand.NextVector2Circular(4, 4);
            royalMagicRenderer.SpawnParticle(Projectile.Center + Main.rand.NextVector2Circular(64, 64), vel, 90);

        }
    }

    private void ImpactEffect()
    {
        int[] gores = AutoGoreLoader.FindGores("GrayRock");
        foreach (int g in gores)
        {
            Gore.NewGore(Projectile.GetSource_FromThis(),
                Projectile.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }


        var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
        sear.innerColor = Color.Gray;
        sear.outerColor = GetCometColor();
        sear.fadeToColor = Color.Black;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
        ShakeScreenPosition.Shake = 2;


        for (float f = 0; f < 4f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(80, 80);
            var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
            zap.innerColor = Color.Gray;
            zap.outerColor = GetCometColor();
            zap.fadeToColor = Color.Black;
            zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
            zap.Rotation = Main.rand.NextFloat(0f, 3f);
        }

        SoundStyle smashSound;
        int sound = Main.rand.Next(3);
        switch (sound)
        {
            default:
            case 0:
                smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
                break;
            case 1:
                smashSound = AssetRegistry.Sounds.Bishinine.Comet1;
                break;
            case 2:
                smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
                foreach (int g in gores)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
                }
                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
                   innerColor: Color.Gray,
                   glowColor: GetCometColor(),
                   outerGlowColor: GetCometColor(), duration: 15, baseSize: .09f);
                p3.Scale *= 4;
                break;
        }


        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);


        var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
        part.fadeToColor = Color.Black;
        part.outerColor = Color.Gray;
        part.noStretch = true;
        part.shrink = true;

        var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
        part2.fadeToColor = Color.Black;
        part2.outerColor = Color.Gray;
        part2.noStretch = true;
        part2.color *= 0.5f;
        for (float f = 0; f < 5; f++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
            vel.Y -= 10;
            var d = Dust.NewDustPerfect(Projectile.Center,
                ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);
        }

        float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
        FXUtil.GlowCircleBoom(Projectile.Center,
           innerColor: Color.Gray,
           glowColor: GetCometColor(),
           outerGlowColor: GetCometColor(), duration: 15, baseSize: boomSize * 2);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
        // throw new NotImplementedException();
    }
}

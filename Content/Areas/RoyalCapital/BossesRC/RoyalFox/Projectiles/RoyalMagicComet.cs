using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;


[Autoload(Side = ModSide.Client)]
public class RoyalMagicCometStarsRenderer : ModSystem
{
    private Asset<Texture2D> _royalSmokeMaskTextureAsset;
    private ManagedRenderTarget _starsRT;
    private ManagedRenderTarget _maskRT;
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

    public override void OnModLoad()
    {
        base.OnModLoad();
        _starsRT = ManagedRenderTarget.New();
        _maskRT = ManagedRenderTarget.New();
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

    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
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
        Projectile.tileCollide = false;
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
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        Timer++;

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

        Projectile.velocity.Y += 0.15f;
        if (Timer >= 45)
            Projectile.hostile = true;
        Projectile.scale = ExtraMath.Osc(0.5f, 0.75f, speed: 3, Projectile.whoAmI);
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {
        Color trailColorFunction = Color.Lerp(Color.White, Color.Blue, completionRatio) * 0.3f * MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(completionRatio));
        trailColorFunction.A = 0;
        return trailColorFunction;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(36, 24, EasingFunction.InOutSine(completionRatio));
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        CometTrailShader cometTrail = ShaderContent.GetInstance<CometTrailShader>();
        cometTrail.BloomColor = Color.Blue;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);


        cometTrail.LaserTexture = TrailRegistry.VortexTrail;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        drawer.scale *= 0.4f;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)Projectile.oldPos.Length;
            drawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            drawer.color = Color.Lerp(Color.White, Color.Black, ratio) * 0.025f;
            drawer.color.A = 0;


            drawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio) * 0.4f;
            Main.spriteBatch.Draw(drawer);
        }
        drawer.scale =  Vector2.One * 0.4f;
        drawer.color = Color.Blue * 0.35f * ExtraMath.Osc(0.75f, 1f, speed: 6, offset: Projectile.whoAmI);
        drawer.color.A = 0;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.6f;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);
    //    Main.spriteBatch.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        drawer.color = Color.White * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 3, Projectile.whoAmI);
        drawer.color.A = 0;
        drawer.scale *= 0.4f;
        Main.spriteBatch.Draw(drawer);

        SpritebatchDrawer blackStar = SpritebatchDrawer.FromProjectile(Projectile);
        blackStar.color = Color.Black;
        blackStar.scale = Vector2.One * ExtraMath.Osc(0.75f, 1f, speed: 3, Projectile.whoAmI) * 0.6f;
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
        if (Main.netMode == NetmodeID.Server)
            return;


        for (float f = 0; f < 4; f++)
        {
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
            Vector2 vel = Main.rand.NextVector2Circular(4, 4);
            royalMagicRenderer.SpawnParticle(Projectile.Center + Main.rand.NextVector2Circular(64, 64), vel, 90);

        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
        // throw new NotImplementedException();
    }
}

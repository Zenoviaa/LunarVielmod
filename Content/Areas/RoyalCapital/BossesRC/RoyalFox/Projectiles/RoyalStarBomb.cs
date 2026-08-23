using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class RoyalStarBombBoom : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 60f;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 512;
        Projectile.height = 512;
        Projectile.hostile = true;
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            SoundStyle explosionSound = AssetRegistry.Sounds.AlcaricFox.FenixExplosion;
            SoundEngine.PlaySound(explosionSound);
            ShockwavePlayer shockwavePlayer = Main.LocalPlayer.GetModPlayer<ShockwavePlayer>();
            shockwavePlayer.Bee = 220;
            shockwavePlayer.shockwavePosition = Projectile.Center;
            shockwavePlayer.rippleSize = 5;
        }
        if (Timer == 2)
        {
            for (float f = 0; f < 32; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(252, 252);
                Vector2 vel = (pos - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 80f);
                var dp = DustParticle.Spawn(pos, vel);
                dp.outerColor = Color.Gray;
                dp.dampening = 0.1f;
                dp.noTileCollide = true;
            }

            for (float f = 0; f < 32f; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(384, 384);
                RoyalMagicStarParticle.Spawn(pos, Vector2.Zero, Scale: Main.rand.NextFloat(0.4f, 0.7f));
            }

            if (Main.netMode != NetmodeID.Server)
            {
                RoyalMagicRenderer renderer = ModContent.GetInstance<RoyalMagicRenderer>();
                for (float f = 0; f < 64; f++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(252, 252);
                    Vector2 vel = (pos - Projectile.Center);
                    vel = vel.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 15f);
                    renderer.SpawnParticle(pos, vel, 180);
                }
            }
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue, 45, baseSize: 0.24f);
            fx.Scale *= 3;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.CreateRipple(Projectile.Center);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.Transparent, Color.White, 35, 1768);
        }

        if (Timer >= 27)
            Projectile.hostile = false;

        ShakeScreenPosition.Shake = MathHelper.Lerp(6, 0, EasingFunction.InExpo(Timer / Time));
        if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
        {
            SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
            effectsPlayer.darknessCurve = MathHelper.Lerp(1.2f, 0f, EasingFunction.InExpo(Timer / (Time / 3f)));
        }
    }

    private void DrawImpactFrames(SpriteBatch sb, Vector2 screenPos)
    {

        if (Timer < 3)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Impact, Projectile.Center);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 1.5f;
            drawer.rotation = Main.GlobalTimeWrappedHourly * 24;
            Main.spriteBatch.Draw(drawer);
        }
    }

    private void DrawWaveBoom(SpriteBatch sb, Vector2 screenPos)
    {
        RoyalShockwaveCircleShader shockwaevShader = RoyalShockwaveCircleShader.Instance;
        shockwaevShader.Time = -Timer * 0.02f + 0.8f;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        sbDrawer.CenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 3.8f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(8f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sb.Draw(sbDrawer);


        sbDrawer.CenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1.9f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(4f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sb.Draw(sbDrawer);

        sb.RestartDefaults();
    }

    public override bool PreDraw(ref Color lightColor)
    {

        if (Timer < 3)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Impact, Projectile.Center);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 1.5f;
            drawer.rotation = Main.GlobalTimeWrappedHourly * 24;
            Main.spriteBatch.Draw(drawer);
        }
        else if (Timer < 6)
        {

        }
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawImpactFrames, DrawLayer.OverPlayers);
        PixelationManager.QueueSpritebatchDrawAction(DrawWaveBoom, DrawLayer.OverPlayers);
    }
}

[Autoload(Side = ModSide.Client)]
public class RoyalStarBombRenderer : ModSystem
{
    public delegate void SpritebatchDrawAction(SpriteBatch sb);

    private RenderTargetProvider _bombRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private Queue<SpritebatchDrawAction> _drawQueue;
    public override void Load()
    {
        base.Load();
        _drawQueue = new Queue<SpritebatchDrawAction>();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderBombs;
    }
    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderBombs;
    }
    private void RenderBombs()
    {
        if (_drawQueue.Count <= 0)
            return;
        GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
        gDevice.SetRenderTarget(_bombRT);
        gDevice.Clear(Color.Transparent);
        var sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
        while (_drawQueue.Count > 0)
        {
            _drawQueue.Dequeue()(sb);
        }
        sb.End();
        PixelationManager.QueueSpritebatchDrawAction(DrawToScreen);
    }
    private void DrawToScreen(SpriteBatch sb, Vector2 screenPos)
    {
        Color outlineColor = new Color(150, 150, 235) * 0.85f;
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;

        RoyalOutlineShader outlineShader = ShaderContent.GetInstance<RoyalOutlineShader>();
        outlineShader.TexelSize = texelSize;
        outlineShader.OutlineColor = outlineColor;
        outlineShader.Levels = 2;
        // sb.Restart(effect: outlineShader.Effect);
        // sb.Draw(_bombRT, Vector2.Zero, Color.White);
        // sb.RestartDefaults();

        sb.Draw(_bombRT, Vector2.Zero, Color.White);
    }


    public static void Queue(SpritebatchDrawAction drawAction)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        RoyalStarBombRenderer renderer = ModContent.GetInstance<RoyalStarBombRenderer>();
        renderer._drawQueue.Enqueue(drawAction);
    }
}
public class RoyalStarBomb : ModProjectile,
    IDrawToRenderTarget
{
    private bool _holding;
    private Vector2 _originalPosition;
    private Vector2 _bounceOffset;
    private float _scale;
    private float _shaker;
    private float _shockTimer;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float State => ref Projectile.ai[2];
    private float Size;
    private float MaxScale = 0.8f;
    private float NumPulses => 3;
    private float Scale => MathHelper.Lerp(0.35f, MaxScale, EasingFunction.InExpo(Size / NumPulses));


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_originalPosition);
        writer.WriteVector2(_bounceOffset);
        writer.Write(Size);
        writer.Write(_holding);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _originalPosition = reader.ReadVector2();
        _bounceOffset = reader.ReadVector2();
        Size = reader.ReadSingle();
        _holding = reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 80;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        MaxScale = 1.5f;
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = true;
        Projectile.timeLeft = 1800;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (!NPC.AnyNPCs(ModContent.NPCType<RoyalFox>()))
            Projectile.Kill();
        if (Size < NumPulses)
        {
            RoyalFox.ChargeParticlesBig(Projectile.Center, in Timer);
            if (Timer == 1 || Timer % 65 == 0)
            {
                FXUtil.CreateRipple(Projectile.Center);
                PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.Transparent, Color.White, 35, 768);

                if (Main.netMode != NetmodeID.Server)
                {
                    ScreenShaderSystem system = ModContent.GetInstance<ScreenShaderSystem>();
                    system.TintScreen(Color.Pink, 0.4f, 15);
                }
                SoundStyle sound;
                switch (Size)
                {
                    default:
                    case 0:
                        sound = AssetRegistry.Sounds.AlcaricFox.FenixStarballgrow1;
                        break;
                    case 1:
                        sound = AssetRegistry.Sounds.AlcaricFox.FenixStarballgrow2;
                        break;
                    case 2:
                        sound = AssetRegistry.Sounds.AlcaricFox.FenixStarballgrow3;
                        break;
                }
                SoundEngine.PlaySound(sound);
                _shockTimer = 0;
                _bounceOffset = Main.rand.NextFloat(-2f, 2f).ToRotationVector2();
                _bounceOffset *= 115;
                Size++;
            }
        }

        _shockTimer++;
        _shaker *= 0.98f;
        if (Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
            RoyalMagicStarParticle.Spawn(pos, Vector2.Zero, Scale: Main.rand.NextFloat(0.4f, 0.7f));
            if (Main.netMode != NetmodeID.Server)
            {
                pos = Projectile.Center + Main.rand.NextVector2Circular(128, 129);
                RoyalMagicRenderer renderer = ModContent.GetInstance<RoyalMagicRenderer>();
                Vector2 vel = (pos - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                renderer.SpawnParticle(pos, vel * Main.rand.NextFloat(3f, 8f) * _scale * 0.8f, 180 * _scale * 0.8f);
            }
        }


        if (_holding)
        {
            Projectile.hostile = false;
        }
        if (!_holding)
        {
            if (State != 0)
            {
                if (State == 10)
                {
                    _bounceOffset = State.ToRotationVector2();
                    _bounceOffset *= 115;
                    _holding = true;
                    Projectile.netUpdate = true;
                }
                else
                {
                    _bounceOffset = State.ToRotationVector2();
                    _bounceOffset *= 115;
                    State = 0;

                    Projectile.netUpdate = true;
                }
            }
        }

        if (State == 11)
        {
            Projectile.netUpdate = true;
            Projectile.Kill();
        }

        if (_holding)
        {
            Projectile.Center = Parent.Center;
            if (Timer % 3 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
                RoyalMagicStarParticle.Spawn(pos, Vector2.Zero, Scale: Main.rand.NextFloat(0.4f, 0.7f));
            }
        }

        if (_bounceOffset.Length() > 0)
        {
            _bounceOffset *= 0.96f;
        }
        _scale = MathHelper.Lerp(_scale, Scale, 0.1f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<RoyalStarBombBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    private void DrawStarBomb(SpriteBatch sb)
    {
        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.WhiteCircle, Projectile.Center + _bounceOffset);
        Color color = Color.Lerp(Color.Blue, Color.Pink, _shockTimer / 30);
        circleDrawer.color = color * 0.75f * MathHelper.Lerp(1f, 0f, EasingFunction.Clamp(_shockTimer / 30));
        circleDrawer.color.A = 0;
        circleDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 12, _shockTimer / 30f);
        Main.spriteBatch.Draw(circleDrawer);

        SpritebatchDrawer glowBall2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center + _bounceOffset);
        glowBall2.color = Color.White * 0.9f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowBall2.color.A = 0;
        glowBall2.scale *= 2 * _scale;
        glowBall2.scale.Y *= 1.3f;
        sb.Draw(glowBall2);


        RoyalMagicBallShader ballShader = ShaderContent.GetInstance<RoyalMagicBallShader>();
        ballShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        ballShader.BloomColor = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3));
        ballShader.Distortion = MathHelper.Lerp(9f, 1f, EasingFunction.InOutExpo(Size / NumPulses));
        ballShader.Time = Main.GlobalTimeWrappedHourly * -24;
        ballShader.Resolution = TextureAssets.Projectile[Type].Value.Size();
        ballShader.StarTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Stars").Value;
        sb.Restart(SpriteSortMode.Immediate, effect: ballShader.Effect);

        SpritebatchDrawer ballDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        ballDrawer.color = Color.White;
        ballDrawer.scale = Vector2.One * _scale;
        ballDrawer.worldPosition += _bounceOffset;
        sb.Draw(ballDrawer);

        sb.RestartDefaults();

        SpritebatchDrawer glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3)) * 0.1f;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.Blue * 0.75f * (_bounceOffset.Length() / 115f);
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);



        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center + _bounceOffset);
        glowBall.color = Color.White * 0.75f * MathHelper.Lerp(0f, 1f, EasingFunction.InExpo((_bounceOffset.Length() / 115f)));
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SolarRing, Projectile.Center + _bounceOffset);
        glowBall.color = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3)) * 0.08f;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale * MathHelper.Lerp(0f, 2f, EasingFunction.InExpo((_bounceOffset.Length() / 115f)));
        sb.Draw(glowBall);


        glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center + _bounceOffset);
        glowBall.color = Color.White * 0.92f;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale * MathHelper.Lerp(0, 6.4f, EasingFunction.InExpo((_bounceOffset.Length() / 115f)));
        sb.Draw(glowBall);
    }
    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 0, completionRatio);
    }
    private float StarryTrailWidthFunction2(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 2.6f;
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {

        return Color.White;
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserAlphaShader alphaShader = ShaderContent.GetInstance<BasicLaserAlphaShader>();
        alphaShader.LaserTexture = TrailRegistry.LightningTrail3;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction2, alphaShader, Projectile.Size * 0.5f);

    }
    private Color SpiralColorFunction(float completionRatio)
    {
        return Color.White;
    }
    private void RenderSpiralTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader basicShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        basicShader.InnerColor = Color.SkyBlue;
        basicShader.OuterColor = Color.DarkBlue;
        basicShader.Tiling = new Vector2(4f, 1f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, SpiralColorFunction, StarryTrailWidthFunction2, basicShader, Projectile.Size * 0.5f);





    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderSpiralTrail, DrawLayer.OverNPCsWithOutline);
        RoyalStarBombRenderer.Queue(DrawStarBomb);
        RoyalMagicRenderer.Queue(RenderStarryDashTrail);
        //   OutlineRenderer.Queue(DrawOutlines);
    }
}

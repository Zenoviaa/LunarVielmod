using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class RoyalStarBomb : ModProjectile,
    IDrawToRenderTarget
{
    private float _scale;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Size => ref Projectile.ai[2];
    private float MaxScale = 0.8f;
    private float NumPulses => 3;
    private float Scale => MathHelper.Lerp(0.25f, MaxScale, EasingFunction.InExpo(Size / NumPulses));
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
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

        if(Size < NumPulses)
        {
            RoyalFox.ChargeParticles(Projectile.Center, in Timer);
            if (Timer % 65 == 0)
            {
                FXUtil.CreateRipple(Projectile.Center);
                PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.Transparent, Color.White, 35, 768);
                if(Main.netMode != NetmodeID.Server)
                {
                    ScreenShaderSystem system = ModContent.GetInstance<ScreenShaderSystem>();
                    system.TintScreen(Color.Pink, 0.2f, 15);
                }
                Size++;
            }
        }

        if(Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(256, 256);
            RoyalMagicStarParticle.Spawn(pos, Vector2.Zero, Scale: Main.rand.NextFloat(0.4f, 0.7f));
            if(Main.netMode != NetmodeID.Server)
            {
                pos = Projectile.Center + Main.rand.NextVector2Circular(128, 129);
                RoyalMagicRenderer renderer = ModContent.GetInstance<RoyalMagicRenderer>();
                Vector2 vel = (pos - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                renderer.SpawnParticle(pos, vel * Main.rand.NextFloat(3f, 8f) * _scale * 0.8f, 180 * _scale * 0.8f);
            }
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
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    private void DrawStarBomb(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer glowBall2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
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
        sb.Draw(ballDrawer);

        sb.RestartDefaults();

        SpritebatchDrawer glowBall = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowBall.color = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3)) * 0.1f;
        glowBall.color.A = 0;
        glowBall.scale *= 2 * _scale;
        sb.Draw(glowBall);

        /*
        SpritebatchDrawer magicCircle = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MagicCircle2, Projectile.Center);
        magicCircle.color = Color.Blue * 0.35f * ExtraMath.Osc(0f, 1f, speed: 6);
        magicCircle.color.A = 0;
        magicCircle.scale *= 3 * _scale;
        magicCircle.rotation = Main.GlobalTimeWrappedHourly;
        sb.Draw(magicCircle);*/
    }

    private void DrawOutlines(SpriteBatch sb)
    {


        RoyalMagicBallShader ballShader = ShaderContent.GetInstance<RoyalMagicBallShader>();
        ballShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        ballShader.BloomColor = Color.Lerp(Color.Blue, Color.Magenta, ExtraMath.Osc(0f, 1f, speed: 3));
        ballShader.Distortion = MathHelper.Lerp(9f, 1f, EasingFunction.InOutExpo(Size / NumPulses));
        ballShader.Time = Main.GlobalTimeWrappedHourly * -24;
        ballShader.Resolution = TextureAssets.Projectile[Type].Value.Size();
        ballShader.StarTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Stars").Value;
        sb.Restart(SpriteSortMode.Immediate, effect: ballShader.Effect);

        SpritebatchDrawer ballDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        ballDrawer.color = Color.Red;
        ballDrawer.scale = Vector2.One * _scale;
        sb.Draw(ballDrawer);

        sb.RestartDefaults();
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawStarBomb, DrawLayer.OverNPCsWithOutline);
     //   OutlineRenderer.Queue(DrawOutlines);
    }
}

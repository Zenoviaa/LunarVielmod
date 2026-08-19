using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Dusts;
using Stellamod.Effects.Generic;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class PacmanBoom : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 60;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.5f;
        Projectile.timeLeft = 44;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var smokeParitcle = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                smokeParitcle.dampening = 0.09f;
                smokeParitcle.fadeToColor = Color.Black * 0.5f;
                smokeParitcle.initialColor = Color.DarkRed * 0.5f;
                smokeParitcle.Scale *= 2f;
                smokeParitcle.behindLayer = true;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var dp = DustParticle.Spawn(pos, vel);
                dp.dampening = 0.05f;
                dp.innerColor = Color.OrangeRed;
                dp.fast = true;
            }

            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red, duration: 12, baseSize: 0.24f);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, Projectile.position);
        }
    }

    private void DrawPixelatedFlameBoom(SpriteBatch sb, Vector2 sp)
    {
        NoisyBoomShader boomShader = ShaderContent.GetInstance<NoisyBoomShader>();
        boomShader.Time = Main.GlobalTimeWrappedHourly * 8;
        boomShader.NoiseColor = Color.Red;
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = boomShader };

        float time = Timer / Time;
        float ease = EasingFunction.OutExpo(time);
        float ease2 = EasingFunction.InOutSine(time);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.4f * ExtraMath.Osc(0.6f, 1f, speed: 6) * MathHelper.Lerp(1f, 0f, ease2);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.2f * MathHelper.Lerp(0f, 1f, ease);
        sb.Draw(glowDrawer);
        using (SpritebatchStarter.Begin(sb, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.FlameVortexNoise.Asset, Projectile.Center);
            drawer.scale = Vector2.One * MathHelper.Lerp(0.2f, 1.56f, ease);
            drawer.color = Color.Lerp(Color.Gold, Color.Transparent, ease2);
            sb.Draw(drawer);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlameBoom);
    }
}

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss.Projectiles;

public class CariyaThrust : ModProjectile
{
    private Vector2 _mirageOffset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 240;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 4;
    }
    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            SoundStyle thrustSound = AssetRegistry.Sounds.Cariya.Carianpokie with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(thrustSound, Projectile.position);
            ThrustParticle.Spawn(Projectile.Center, Projectile.velocity);
            LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.1f);
        }
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }
        Projectile.velocity.X *= 0.94f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Timer % 8 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            var dp = DustParticle.Spawn(pos, Vector2.Zero, DustParticleSpawnParams.Default);
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.outerColor = Color.Blue;
        }

        if (Timer % 7 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(96, 96);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer % 6 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 vel = -Projectile.velocity * 0.3f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Blue;
            fx.VectorScale *= 0.5f;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float outAlpha = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        SpritebatchDrawer afterDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            float ratio = i / (float)Projectile.oldPos.Length;
            afterDrawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ratio) * 0.15f * outAlpha;
            afterDrawer.color.A = 0;
            afterDrawer.worldPosition = pos;
            afterDrawer.scale.X *= 0.4f;
            Main.spriteBatch.Draw(afterDrawer);
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0.8f, EasingFunction.InOutSine(Timer / 60f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f * outAlpha;
        shader.OuterColor = Color.DarkBlue * 0.5f * outAlpha;
        Main.spriteBatch.Restart(effect: shader.Effect);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= 1.5f;
        sbDrawer.scale.X *= 0.3f;
        sbDrawer.color = Color.LightBlue * 0.5f * outAlpha;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition += _mirageOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale.X *= 0.3f;
        sbDrawer.color = Color.White * outAlpha * 0.5f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        Main.spriteBatch.RestartDefaults();
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 16f; f++)
        {
            Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 3;
            vel *= Main.rand.NextFloat(0.5f, 6);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            DustParticle dp = DustParticle.Spawn(pos, vel);
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.innerColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
        }
    }
}

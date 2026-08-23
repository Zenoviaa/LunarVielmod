using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss.Projectiles;

public class CariyaTallFlyingSlash : ModProjectile
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
        Projectile.width = 100;
        Projectile.height = 384;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            SoundStyle overheadSlash = AssetRegistry.Sounds.Cariya.CarianSlash2 with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(overheadSlash, Projectile.position);
            var fx = FXUtil.GlowStretch(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver4));
            fx.VectorScale.X *= 6;
            fx.VectorScale.Y *= 0.75f;
        }
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }
        if (Projectile.velocity.X < 0)
        {
            Projectile.spriteDirection = -1;
        }
        else
        {
            Projectile.spriteDirection = 1;
        }

        if (Projectile.velocity.Length() > 1)
            Projectile.velocity.X *= 0.94f;
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
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(96, 144);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer % 6 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 144);
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
            Main.spriteBatch.Draw(afterDrawer);
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0.8f, EasingFunction.InOutSine(Timer / 60f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f;
        shader.OuterColor = Color.DarkBlue * 0.5f;
        Main.spriteBatch.Restart(effect: shader.Effect);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= 1.5f;
        sbDrawer.color = Color.LightBlue * 0.5f * outAlpha;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition += _mirageOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * outAlpha * 0.5f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        Main.spriteBatch.RestartDefaults();
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.DarkBlue, 35);
        fx.Scale *= 2;
        for (float f = 0; f < 32f; f++)
        {
            Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 3;
            vel *= Main.rand.NextFloat(0.5f, 6);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(127, 127);
            DustParticle dp = DustParticle.Spawn(pos, vel);
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 1.5f;
            dp.innerColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
        }
    }
}
public class CariyaFlyingSlash : ModProjectile
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
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            var fx = FXUtil.GlowStretch(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.PiOver4));
            fx.VectorScale.X *= 6;
            fx.VectorScale.Y *= 0.75f;
        }
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }
        if (Projectile.velocity.X < 0)
        {
            Projectile.spriteDirection = -1;
        }
        else
        {
            Projectile.spriteDirection = 1;
        }

        if (Projectile.velocity.Length() > 1)
            Projectile.velocity.X *= 0.94f;
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
            Main.spriteBatch.Draw(afterDrawer);
        }

        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0.8f, EasingFunction.InOutSine(Timer / 60f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f;
        shader.OuterColor = Color.DarkBlue * 0.5f;
        Main.spriteBatch.Restart(effect: shader.Effect);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= 1.5f;
        sbDrawer.color = Color.LightBlue * 0.5f * outAlpha;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition += _mirageOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * outAlpha * 0.5f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        Main.spriteBatch.RestartDefaults();
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.DarkBlue, 35);
        fx.Scale *= 2;
        for (float f = 0; f < 32f; f++)
        {
            Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 3;
            vel *= Main.rand.NextFloat(0.5f, 6);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(127, 127);
            DustParticle dp = DustParticle.Spawn(pos, vel);
            dp.outerColor = Color.Blue;
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.Scale *= 1.5f;
            dp.innerColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
        }
    }
}

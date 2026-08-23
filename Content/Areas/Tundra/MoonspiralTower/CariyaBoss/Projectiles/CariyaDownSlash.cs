using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss.Projectiles;

public class CariyaDownSlash : ModProjectile
{
    private Vector2 _mirageOffset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.timeLeft = 30;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle slashSound = AssetRegistry.Sounds.Cariya.CarianSlash2 with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(slashSound, Projectile.position);


            var fx = FXUtil.GlowStretch(Projectile.Center, Projectile.velocity.RotatedBy(-MathHelper.PiOver4));
            fx.VectorScale.X *= 6;
            fx.VectorScale.Y *= 0.75f;
        }
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }
        if (Timer % 6 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 vel = -Projectile.velocity * 0.3f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Blue;
            fx.VectorScale *= 0.5f;
        }
        float frameSpeed = 4;
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
        }
        Projectile.velocity.Y *= 0.99f;
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
        Projectile.rotation += Projectile.spriteDirection * 0.025f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float outAlpha = EasingFunction.OutExpo((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer afterDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        afterDrawer.scale *= 2f;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            afterDrawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ratio) * 0.15f * outAlpha;
            afterDrawer.color *= MathHelper.Lerp(1f, 0f, ratio);
            afterDrawer.color.A = 0;
            afterDrawer.worldPosition = pos;
  
            Main.spriteBatch.Draw(afterDrawer);
        }
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= 2f;
        sbDrawer.color = Color.LightBlue * 0.5f * outAlpha;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition += _mirageOffset;
        Main.spriteBatch.Draw(sbDrawer);
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0.8f, EasingFunction.OutExpo(Timer / 60f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.75f * outAlpha;
        shader.OuterColor = Color.DarkBlue * 0.75f * outAlpha;
        Main.spriteBatch.Restart(effect: shader.Effect);



        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * outAlpha * 0.85f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        Main.spriteBatch.RestartDefaults();
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

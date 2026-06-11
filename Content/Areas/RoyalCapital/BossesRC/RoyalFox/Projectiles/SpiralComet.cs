using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class SpiralComet : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        SpawnParticles();
        if (Timer < 45)
            Projectile.velocity *= 0.96f;
        else
            Projectile.velocity *= 1.15f;
        if(Projectile.velocity.Length() > 25)
        {
            if (Timer % 4 == 0)
            {
                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero));
                donut.Scale *= 0.6f;
            }
        }
    }

    private void SpawnParticles()
    {
        Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        if (screenRect.Contains(Projectile.position.ToPoint()))
        {
            if (Timer % 7 == 0)
            {
                RoyalFox.SpawnCometStarParticle(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f), 60);
            }


            if (Main.rand.NextBool(9))
            {
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 7));
                dp.outerColor = Color.Blue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.1f;
                dp.superFast = true;
            }
        }
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 0, ratio);
    }
    private Color GetTrailColor(float ratio)
    {
        Color c = Color.Lerp(Color.Black, Color.LightSkyBlue, EasingFunction.QuadraticBump(ratio));
        c.A = 0;
        return c;
    }

    private void RenderCometTrail(GraphicsDevice gDevice)
    {
        CometTrailShader cometTrail = ShaderContent.GetInstance<CometTrailShader>();
        cometTrail.BloomColor = Color.Lerp(Color.Blue, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 6, Projectile.whoAmI)) * 0.23f;
     //   TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, cometTrail, Projectile.Size * 0.5f);


        cometTrail.LaserTexture = AssetManager.LaserTextures.Aura;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, cometTrail, Projectile.Size * 0.5f);
    }

    private void DrawCometHead(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer cometDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        cometDrawer.color = Color.White * 0.4f;
        cometDrawer.color.A = 0;
        cometDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.48f, ExtraMath.Osc(0f, 1f, speed: 16, Projectile.whoAmI)) * 0.2f;
        cometDrawer.scale *= Vector2.Lerp(Vector2.One * 2f, Vector2.One, EasingFunction.OutExpo(Timer / 60f));
        sb.Draw(cometDrawer);

        cometDrawer.color = Color.Blue * 0.04f;
        cometDrawer.color.A = 0;
        cometDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.48f, ExtraMath.Osc(0f, 1f, speed: 16, Projectile.whoAmI)) * 1.5f;
        sb.Draw(cometDrawer);

        cometDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        cometDrawer.color = Color.White * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 64, Projectile.whoAmI);
        cometDrawer.color.A = 0;
        cometDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.48f, ExtraMath.Osc(0f, 1f, speed: 16, Projectile.whoAmI)) * 0.4f;
        cometDrawer.scale *= MathHelper.Lerp(7, 1f, EasingFunction.OutExpo(Timer / 60f));
        cometDrawer.rotation = MathHelper.Lerp(0.5f, 0f, EasingFunction.OutExpo(Timer / 60f));
        sb.Draw(cometDrawer);


        cometDrawer.scale *= 0.6f;
        sb.Draw(cometDrawer);
        sb.Draw(cometDrawer);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderCometTrail);
        PixelationManager.QueueSpritebatchDrawAction(DrawCometHead);
    }
}

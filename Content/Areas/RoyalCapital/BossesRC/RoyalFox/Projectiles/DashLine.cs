using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class DashLine : ModProjectile,
    IDrawToRenderTarget
{
    private float DeathTime => 25;
    private ref float Timer => ref Projectile.ai[0];
    private ref float IsUsed => ref Projectile.ai[1];
    private ref float DeathTimer => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(IsUsed == 1)
        {
            IsUsed = 2;
            Projectile.netUpdate = true;
        }
        if (IsUsed == 2)
        {
            DeathTimer++;
            if(DeathTimer >= 25f)
            {
                Projectile.Kill();
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void DrawLine(SpriteBatch sb, Vector2 screenPos)
    {
        float alpha = EasingFunction.OutSine(Timer / 60) * MathHelper.Lerp(1f, 0f, DeathTimer / DeathTime);
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4"), Projectile.Center - Projectile.velocity * 1200);
        lineDrawer.color = Color.Lerp(Color.Black, Color.White, alpha);
        lineDrawer.color.A = 0;
        lineDrawer.rotation = Projectile.velocity.ToRotation();
        lineDrawer.LeftCenterOrigin();

        Vector2 scale = Vector2.Lerp(new Vector2(0f, 1f), new Vector2(2f, 1f), alpha);
        scale.Y = 0.75f;
        scale.X *= 12;
        lineDrawer.scale = scale;
        sb.Draw(lineDrawer);
    }
    private Color StarryTrailColorFunction(float completionRatio)
    {

        return Color.Lerp(Color.White, Color.Transparent, completionRatio) *
            MathHelper.Lerp(0f, 1f, EasingFunction.Clamp((float)Projectile.timeLeft / 30f)) * EasingFunction.QuadraticBump(DeathTimer / DeathTime);
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(180, 0, completionRatio);
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        List<Vector2> points = new List<Vector2>();
        float numPoints = 24;
        Vector2 endPoint = Vector2.Lerp(Projectile.Center - Projectile.velocity * 1000, Projectile.Center + Projectile.velocity * 1200, DeathTimer / DeathTime);
        Vector2 startPoint = endPoint - Projectile.velocity * 3500;
        for(float f = 0; f < numPoints; f++)
        {
            Vector2 p = Vector2.Lerp(endPoint, startPoint, f / numPoints);
            points.Add(p);
        }
        Vector2[] trailPoints = points.ToArray();
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawLine);
        if (DeathTimer <= 0)
            return;

        PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
        //  throw new NotImplementedException();
    }
}

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
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

    private Vector2 StartPoint => EndPoint - Projectile.velocity * 9000;
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity * 3000;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), EndPoint, StartPoint, 10, ref collisionPoint))
            return true;

        return false;
    }
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
        if (IsUsed == 1)
        {
            IsUsed = 2;
            Projectile.netUpdate = true;
        }
        if (IsUsed == 2)
        {

            Projectile.hostile = true;
            if(DeathTimer >= 16)
            {
                Projectile.hostile = false;
            }

            DeathTimer++;
            if (DeathTimer == 1)
            {
                Vector2 endPoint = Projectile.Center + Projectile.velocity * 3000;
                Vector2 startPoint = endPoint - Projectile.velocity * 9000;
                Rectangle screenREct = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
                float numPoints = 48;
                for (float f = 0; f < numPoints; f++)
                {
                    float ratio = f / numPoints;
                    Vector2 pos = Vector2.Lerp(startPoint, endPoint, ratio);
                    if (screenREct.Contains(pos.ToPoint()))
                    {
                        float numParticles = 4;
                        for (float n = 0; n < numParticles; n++)
                        {
                            RoyalFox.CreateRoyalStarSmallSmoke(pos + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2));
                        }
                    }
                }
                //    ShakeScreenPosition.Shake = 10;
            }
            if (DeathTimer >= 25f)
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
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4"), Projectile.Center - Projectile.velocity * 3000);
        lineDrawer.color = Color.Lerp(Color.Black, Color.White, alpha);
        lineDrawer.color.A = 0;
        lineDrawer.rotation = Projectile.velocity.ToRotation();
        lineDrawer.LeftCenterOrigin();

        Vector2 scale = Vector2.Lerp(new Vector2(0f, 1f), new Vector2(2f, 1f), alpha);
        scale.Y = 0.75f;
        scale.X *= 18;
        lineDrawer.scale = scale;
        sb.Draw(lineDrawer);
    }
    private Color StarryTrailColorFunction(float completionRatio)
    {

        return Color.Lerp(Color.White, Color.Transparent, completionRatio) *
            MathHelper.Lerp(0f, 1f, EasingFunction.Clamp(Projectile.timeLeft / 30f)) * EasingFunction.QuadraticBump(DeathTimer / DeathTime);
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(354, 0, completionRatio) * EasingFunction.QuadraticBump(DeathTimer / DeathTime);
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        List<Vector2> points = new List<Vector2>();
        float numPoints = 64;
        Vector2 endPoint = Projectile.Center + Projectile.velocity * 3000;
        Vector2 startPoint = endPoint - Projectile.velocity * 9000;
        for (float f = 0; f < numPoints; f++)
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

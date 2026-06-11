using Stellamod.Common.Shaders;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class RoyalMagicStarryDashTrail : ModProjectile,
    IDrawToRenderTarget
{
    private float Time => 30f;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;

    private Vector2 StartPoint => Projectile.Center - Projectile.velocity;
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity * 4;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), EndPoint, StartPoint, 48, ref collisionPoint))
            return true;

        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(2) && Timer < 30)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 startPos = Projectile.Center - Projectile.velocity;
                Vector2 endPos = Projectile.Center + Projectile.velocity * 4;
                Vector2 pos = Vector2.Lerp(startPos, endPos, Main.rand.NextFloat(0f, 1f));
                pos += Main.rand.NextVector2Circular(64, 64);
                var p = RoyalMagicSwordParticle.Spawn(
                    pos, Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 45), Scale: Main.rand.NextFloat(0.15f, 0.25f));
                p.color = Color.Blue;
            }
        }

        if (Timer >= 33)
            Projectile.hostile = false;
        if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
        {
            SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
            effectsPlayer.darknessCurve = MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / 60f));
        }
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {

        return Color.White;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(240, 284, completionRatio) * MathHelper.SmoothStep(1f, 0f, Timer / Time);
    }
    private float StarryTrailWidthFunction2(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 2.6f;
    }
    private float StarryTrailWidthFunction3(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 1.6f;
    }
    private float StarryTrailWidthFunction4(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 3.5f;
    }


    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        Vector2 startPos = Projectile.Center - Projectile.velocity;
        Vector2 endPos = Projectile.Center + Projectile.velocity * 4;
        List<Vector2> points = new List<Vector2>();

        float ratio = Timer / Time;
        Vector2 startTrailPoint = startPos;
        Vector2 endTrailPoint = endPos;
        float numPoints = 32;
        for (float f = 0; f < numPoints; f++)
        {
            float ratio2 = f / numPoints;
            points.Add(Vector2.Lerp(endTrailPoint, startTrailPoint, ratio2));
        }
        Vector2[] trailPoints = points.ToArray();
        BasicLaserAlphaShader alphaShader = ShaderContent.GetInstance<BasicLaserAlphaShader>();
        alphaShader.LaserTexture = TrailRegistry.LightningTrail3;
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction2, alphaShader);
    }

    private void RenderNormalStarryTrail(GraphicsDevice gDevice)
    {
        Vector2 startPos = Projectile.Center - Projectile.velocity; ;
        Vector2 endPos = Projectile.Center + Projectile.velocity * 4;
        List<Vector2> points = new List<Vector2>();

        float ratio = Timer / Time;
        float numPoints = 32;
        for (float f = 0; f < numPoints; f++)
        {
            float ratio2 = f / numPoints;
            points.Add(Vector2.Lerp(endPos, startPos, ratio2));
        }
        Vector2[] trailPoints = points.ToArray();

        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserTexture = TrailRegistry.SimpleTrail;
        laserShader.InnerColor = Color.Lerp(Color.Pink, Color.Blue, ratio);
        laserShader.OuterColor = Color.Lerp(Color.DarkBlue, Color.Black, ratio);
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction4, laserShader);

        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = Color.SkyBlue;
        laserShader.OuterColor = Color.Lerp(Color.SkyBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction3, laserShader);

        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, trailPoints, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader);
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
        RoyalMagicRenderer.Queue(RenderStarryDashTrail);
        PixelationManager.QueuePrimitivesDrawAction(RenderNormalStarryTrail, DrawLayer.OverPlayers);
        // throw new NotImplementedException();
        //    PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
    }
}

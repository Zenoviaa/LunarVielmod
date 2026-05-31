using Stellamod.Common.Shaders;
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
        Projectile.timeLeft = (int)Time;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {

        return Color.White;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 152, completionRatio) * MathHelper.SmoothStep(1f, 0f,Timer/ Time);
    }
    private float StarryTrailWidthFunction2(float completionRatio)
    {
        return StarryTrailWidthFunction(completionRatio) * 2.6f;
    }


    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        Vector2 startPos = Projectile.Center;
        Vector2 endPos = Projectile.Center + Projectile.velocity * 2;
        List<Vector2> points = new List<Vector2>();

        float ratio = Timer / Time;
        Vector2 startTrailPoint = Vector2.Lerp(startPos, endPos, ratio);
        Vector2 endTrailPoint = startTrailPoint - Projectile.velocity.SafeNormalize(Vector2.Zero) * 4500;
        float numPoints = 32;
        for(float f = 0; f < numPoints; f++)
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
        Vector2 startPos = Projectile.Center;
        Vector2 endPos = Projectile.Center + Projectile.velocity;
        List<Vector2> points = new List<Vector2>();

        float ratio = Timer / Time;
        Vector2 startTrailPoint = Vector2.Lerp(startPos, endPos, ratio);
        Vector2 endTrailPoint = startTrailPoint - Projectile.velocity.SafeNormalize(Vector2.Zero) * 1500;
        float numPoints = 32;
        for (float f = 0; f < numPoints; f++)
        {
            float ratio2 = f / numPoints;
            points.Add(Vector2.Lerp(endTrailPoint, startTrailPoint, ratio2));
        }
        Vector2[] trailPoints = points.ToArray();
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
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

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;

public class CelestialBowSpin : ModProjectile
{
    private Vector2 _mirageOffset;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.timeLeft = 180;
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
        if (Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }

        if(Timer % 6 == 0)
        {
            
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Projectile.velocity.RotatedBy(MathHelper.ToRadians(30) * 0.1f));
                sp.Scale *= 0.5f;
                sp.flickering = true;
                sp.outerColor = Color.Turquoise;
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.dampening = 0.05f;
            
        }

        if(Timer > 45)
        {
            Projectile.hostile = true;
        }
        else
        {
            Projectile.hostile = false;
        }

        float rotIncrease = MathHelper.Lerp(0.01f, 0.12f, EasingFunction.InSine(Timer / 60f));
        Projectile.rotation += rotIncrease;
        Projectile.Center = Parent.Center;
    }

    private void DrawCelestialTrail(GraphicsDevice gDevice)
    {
        BlackFireShader laserShader =BlackFireShader.Instance;
        laserShader.Tiling = new Vector2(1f, 2f);
        //   laserShader.LaserTexture = TrailRegistry.TwistingTrail;
        laserShader.PrimaryTexture = TrailRegistry.BeamTrail;
        laserShader.BloomTexture = TrailRegistry.GlowTrail;
   //     laserShader.LaserColor = Color.LightGreen;
        laserShader.InnerColor = Color.Turquoise;
        laserShader.OuterColor = Color.Lerp(Color.Turquoise, Color.Black, 0.85f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BasicLaserShader splittingShader = BasicLaserShader.Instance;
        splittingShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        splittingShader.InnerColor = Color.Turquoise;
        splittingShader.OuterColor = Color.Lerp(Color.White, Color.DarkTurquoise, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, DashTrailColorFunction, DashTrailWidthFunction, splittingShader, Projectile.Size * 0.5f);
       // TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, DashTrailColorFunction, DashTrailWidthFunction, splittingShader, Projectile.Size * 0.5f);
    }
    private float DashTrailWidthFunction(float completionRatio)
    {
        float outAlpha = EasingFunction.Clamp((float)Projectile.timeLeft / 60f);
        return MathHelper.Lerp(58, 0, completionRatio) * outAlpha;
    }

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }
    private float GetTrailWidth(float completionRatio)
    {
        float outAlpha = EasingFunction.Clamp((float)Projectile.timeLeft / 60f);
        return MathHelper.Lerp(64, 0, completionRatio) * outAlpha;
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }
    private void DrawPixelatedBows(SpriteBatch sb, Vector2 screenPos)
    {
        float alpha = EasingFunction.InSine(Timer / 30f);
        alpha *= (float)(EasingFunction.Clamp(Projectile.timeLeft / 30f));
        Vector2 pullScale = Vector2.One;
        pullScale *= MathHelper.Lerp(1.45f, 1f, EasingFunction.InSine(Timer / 30f));


        float come = EasingFunction.QuadraticBump(Timer / 60f);
        Vector2 inOffset = Vector2.Lerp(Vector2.Zero, Projectile.rotation.ToRotationVector2() * 128,  come);

        SpritebatchDrawer backGlowDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), Projectile.Center); ;
        backGlowDrawer.scale *= pullScale * 2;
        backGlowDrawer.color = Color.Black * 0.5f * alpha;
        //  glowDrawer.color.A = 0;
        backGlowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(backGlowDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center); ;
        glowDrawer.scale *= pullScale * 0.5f;
        glowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        glowDrawer.color.A = 0;
        glowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center); ;
        spiralVortexDrawer.scale *= pullScale * 0.5f;
        spiralVortexDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.1f * alpha;
        spiralVortexDrawer.color.A = 0;
        spiralVortexDrawer.worldPosition += inOffset;
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(spiralVortexDrawer);

        SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        bowDrawer.scale *= pullScale;
        bowDrawer.color = Color.Lerp(Color.Teal, Color.LightGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.5f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.worldPosition += inOffset;
        Main.spriteBatch.Draw(bowDrawer);


        bowDrawer.worldPosition -= Projectile.rotation.ToRotationVector2() * 8;
        bowDrawer.worldPosition += _mirageOffset;
        bowDrawer.color =
            Color.Lerp(Color.DarkTurquoise, Color.DarkGreen, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI)) * 0.2f * alpha;
        bowDrawer.color.A = 0;
        bowDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(bowDrawer);

        float lineOut = Timer / 60f;
        lineOut = EasingFunction.InOutSine(lineOut);
        float lineOutAlpha = MathHelper.Lerp(1f, 0f, lineOut);
        SpritebatchDrawer bloomlineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Parent.Center);
        bloomlineDrawer.color = Color.LightGreen * alpha * lineOutAlpha;
        bloomlineDrawer.color.A = 0;

        float dist = Vector2.Distance(Projectile.Center, Main.player[Parent.target].Center);
        float bloomLineSize = dist / (float)bloomlineDrawer.texture.Width;
        bloomlineDrawer.scale.X *= bloomLineSize;
        bloomlineDrawer.scale.Y *= 0.025f;
        bloomlineDrawer.LeftCenterOrigin();
        bloomlineDrawer.drawOrigin.X += 64;
        bloomlineDrawer.rotation = Projectile.velocity.ToRotation();
        Main.spriteBatch.Draw(bloomlineDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawCelestialTrail, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBows);
        return false;
    }
}
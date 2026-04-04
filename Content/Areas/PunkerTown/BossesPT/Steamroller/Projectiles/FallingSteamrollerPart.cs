using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

public class FallingSteamrollerPart : ModProjectile
{
    private Vector2 _startPosition;
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
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 240;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity.Y = 13;
            _startPosition = Projectile.Center;
            if (this.OwnedByLocalClient())
            {
                Projectile.velocity.X = Main.rand.NextFloat(-3f, 3f);
                Projectile.netUpdate = true;
            }
        }
        Projectile.velocity.Y += 0.5f;
        Projectile.rotation += 0.05f;
        Projectile.rotation += Projectile.velocity.Length() * 0.01f;
    }
    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 64, completionRatio);
    }

    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.Yellow;
        laserShader.OuterColor = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.GradientPillar, _startPosition);
        lineDrawer.scale = new Vector2(0.025f, 6f);
        lineDrawer.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        lineDrawer.BottomCenterOrigin();
        lineDrawer.color = Color.Lerp(Color.Black, Color.Yellow, EasingFunction.QuadraticBump(Timer / 40f));
        lineDrawer.color.A = 0;

        Main.spriteBatch.Draw(lineDrawer);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<SteamrollerBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: 1);
        }
    }
}

using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class RoyalMagicMiniStar : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 _targetPosition;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Mode => ref Projectile.ai[1];
    private ref float FlashTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 360;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(FlashTimer > 0)
        {
            FlashTimer--;
        }


        if(Mode == 1)
        {
            Timer = 0;
            Mode = 2;
            FlashTimer = 30;
            Projectile.netUpdate = true;
        }

        if(Mode == 2)
        {
            if (Main.rand.NextBool(4))
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(4, 4));
                dp.outerColor = Color.DarkViolet;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
            }
            Projectile.hostile = true;
            if(Timer < 30)
            {
                Player closest = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
                if(closest != null)
                {
                    _targetPosition = closest.Center;
                }
              
            }

            Vector2 vel = (_targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * 35;
            Projectile.velocity = Projectile.velocity.MoveTowards(vel, MathHelper.Lerp(0f, 1f, Timer / 30f));
            if(Timer >= 70)
            {
                Projectile.Kill();
            }
        }
        else
        {
            Projectile.velocity *= 0.96f;
        }
        Projectile.scale = ExtraMath.Osc(0.5f, 0.75f, speed: 3, Projectile.whoAmI);
    }

    private Color StarryTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(32, 0, completionRatio);
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserTexture = TrailRegistry.Beamlight;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);

        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            drawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            drawer.color = Color.Lerp(Color.White, Color.Black, (float)i / (float)Projectile.oldPos.Length) * 0.05f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
        drawer.color = Color.White * ExtraMath.Osc(0.75f, 1f, speed: 6);
        drawer.color.A = 0;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);

        drawer.rotation = MathHelper.Lerp(0, MathHelper.TwoPi * 1, EasingFunction.InOutSine(FlashTimer / 30f));
        drawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 2, EasingFunction.InOutSine(FlashTimer / 30f));
        drawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(FlashTimer / 30f)) * 0.7f;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        Color targetColor = Projectile.hostile ? Color.Red : Color.Yellow;
        drawer.color = targetColor * ExtraMath.Osc(0.5f, 1f, speed: 12);
        drawer.color.A = 0;
        drawer.scale = Vector2.One;
        drawer.rotation = 0;
        drawer.VerticalFrame(1, 2);
        Main.spriteBatch.Draw(drawer);


        float alpha = MathHelper.Lerp(0f, 1f, FlashTimer / 30f);
        var ta = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/RayLight4");
        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(ta, Projectile.Center) ;
        lineDrawer.color = Color.Lerp(Color.Black, Color.White, alpha);
        lineDrawer.color.A = 0;
        lineDrawer.rotation = (_targetPosition - Projectile.Center).ToRotation();
        lineDrawer.LeftCenterOrigin();

        Vector2 scale = Vector2.Lerp(new Vector2(0f, 1f), new Vector2(2f, 1f), alpha);
        scale.Y = 0.5f;

        float xScale = (Vector2.Distance(Projectile.Center, _targetPosition))/ ta.Width() ;
        scale.X *= xScale;
        lineDrawer.scale = scale;
        Main.spriteBatch.Draw(lineDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Violet, 30, Main.rand.NextFloat(100, 200));
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.DarkGray, Color.DarkViolet, duration: 45, baseSize: Main.rand.NextFloat(0.06f, 0.24f));
        for(float n = 0; n < 8; n++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
            dp.outerColor = Color.DarkGray;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.noTileCollide = true;
        }
        if (Main.netMode == NetmodeID.Server)
            return;


        for(float f = 0; f < 4; f++)
        {
            RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
            Vector2 vel = Main.rand.NextVector2Circular(4, 4);
            royalMagicRenderer.SpawnParticle(Projectile.Center + Main.rand.NextVector2Circular(64, 64), vel, 90);

        }
    }

    public void DrawToRenderTargets()
    {
        if (Mode != 2)
            return;

        PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);
       // throw new NotImplementedException();
    }
}

using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
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
        Projectile.width = 16;
        Projectile.height = 16;
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
            Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            if (screenRect.Contains(Projectile.position.ToPoint()))
            {
                if (Timer % 4 == 0)
                {
                    RoyalFox.SpawnCometStarParticle(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f), 60);
                }

                if (Timer % 16 == 0)
                {
                    var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                    sp.outerColor = Color.SkyBlue;
                    sp.gravity = 0.05f;
                    sp.noTileCollide = true;
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

                if (Main.rand.NextBool(21))
                {
                    LightningSparkParticle dp = Particle<LightningSparkParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8),
                        color: Color.Blue,
                        Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    dp.parent = Projectile;
                    dp.gravity = 0f;
                    dp.dampening = 0.05f;
                    dp.fast = true;
                }
            }
            if (Timer == 1)
            {
                SoundStyle activate = AssetRegistry.Sounds.AlcaricFox.FenixStarsactivate;
                SoundEngine.PlaySound(activate, Projectile.position);
            }
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

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        drawer.scale *= 0.2f;

        if(Mode != 2)
        {
            drawer.scale *= 0.5f;
        }
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = i / (float)Projectile.oldPos.Length;
            drawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            drawer.color = Color.Lerp(Color.White, Color.Black, ratio) * 0.0025f;
            drawer.color.A = 0;


            drawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio) * 0.4f;
            Main.spriteBatch.Draw(drawer);
        }
        drawer.scale = Vector2.One * 0.2f;

        float s = MathHelper.Lerp(0.5f, 1.2f, EasingFunction.InOutSine(Timer / 30f));
        if (Mode != 2)
        {
            drawer.scale *= 0.5f;
 
        }
        else
        {
            drawer.scale *= s;
        }

        drawer.color = Color.Blue * 0.35f * ExtraMath.Osc(0.75f, 1f, speed: 6, offset: Projectile.whoAmI);
        if(Mode != 2)
        {
            drawer.color *= ExtraMath.Osc(0.3f, 1f, speed: 6, Projectile.whoAmI);
        }
        drawer.color *= 0.5f;
        drawer.color.A = 0;
        drawer.worldPosition = Projectile.Center;
        //Main.spriteBatch.Draw(drawer);


        drawer.color = Color.White;
        if (Mode != 2)
        {
            drawer.color *= ExtraMath.Osc(0.3f, 1f, speed: 6, Projectile.whoAmI);
        }
        drawer.color *= 0.05f;
        drawer.color.A = 0;
        drawer.scale *= 0.6f;

        drawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(drawer);
        //    Main.spriteBatch.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        drawer.color = Color.White * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 3, Projectile.whoAmI);
        if (Mode != 2)
        {
            drawer.color *= ExtraMath.Osc(0.3f, 1f, speed: 6, Projectile.whoAmI);
        }
        drawer.color *= 0.5f;
        drawer.color.A = 0;
        drawer.scale *= 0.4f;
        if (Mode != 2)
        {
            drawer.scale *= 0.5f;
        }
        else
        {
            drawer.scale *= s;
        }
       // Main.spriteBatch.Draw(drawer);



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


        drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.White;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);


        drawer.color = Projectile.hostile ? Color.Red : Color.Yellow;
        drawer.color.A = 0;
        drawer.VerticalFrame(1, 2);
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundStyle activate = AssetRegistry.Sounds.AlcaricFox.FenixSmallStarExplode;
        SoundEngine.PlaySound(activate, Projectile.position);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.Blue, 30, Main.rand.NextFloat(100, 200));
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue, duration: 45, baseSize: Main.rand.NextFloat(0.06f, 0.24f));
        fx.Scale *= 1.5f;
        for(float n = 0; n < 8; n++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
            dp.outerColor = Color.Blue;
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

    private Color StarryTrailColorFunction(float completionRatio)
    {
        Color trailColorFunction = Color.Lerp(Color.White, Color.Blue, completionRatio) * 0.3f * MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(completionRatio));
        trailColorFunction.A = 0;
        return trailColorFunction;
    }

    private float StarryTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(24, 18, EasingFunction.InOutSine(completionRatio));
    }

    private void RenderStarryDashTrail(GraphicsDevice gDevice)
    {
        CometTrailShader cometTrail = ShaderContent.GetInstance<CometTrailShader>();
        cometTrail.BloomColor = Color.Blue;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);


        cometTrail.LaserTexture = TrailRegistry.VortexTrail;
        TrailDrawer.Draw(Projectile.oldPos, StarryTrailColorFunction, StarryTrailWidthFunction, cometTrail, Projectile.Size * 0.5f);
    }

    public void DrawToRenderTargets()
    {
        if (Mode != 2)
            return;
        PixelationManager.QueuePrimitivesDrawAction(RenderStarryDashTrail);

        // throw new NotImplementedException();
    }
}

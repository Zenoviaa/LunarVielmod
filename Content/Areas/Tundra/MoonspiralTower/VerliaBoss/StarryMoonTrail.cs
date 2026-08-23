using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;

public class StarryMoonTrail : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float ShouldKill => ref Projectile.ai[1];
    private ref float KillTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2064;
        ProjectileID.Sets.TrailCacheLength[Type] = 64;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();


        float distanceToParent = Vector2.Distance(Parent.Center, Projectile.Center);
        if(!Parent.active || distanceToParent > 64)
        {
            ShouldKill = 1;
        }

        if (Main.rand.NextBool(6))
        {
           var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
            sp.innerColor = Color.White;
            sp.outerColor = Color.Blue;
            sp.behindLayer = true;
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.fast = true;
        }
        if (ShouldKill == 1)
        {
            KillTimer++;
            if (KillTimer >= 30f)
                Projectile.Kill();
        }
        if (ShouldKill == 0)
        {
            Projectile.velocity = (Parent.Center - Projectile.Center);
        }
    }

    private void DrawPixelatedStarTrail(GraphicsDevice gDevice)
    {
        StarMixShader laserShader = StarMixShader.Instance;
        laserShader.MaskTexture = TrailRegistry.Beamlight;
        laserShader.InnerTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/StarNoise");
       // laserShader.t
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.DarkBlue;
        laserShader.Tiling = Vector2.One ;
        laserShader.Time = Main.GlobalTimeWrappedHourly * -0.5f;
       
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.SkyBlue;
        bloomTrailShader.OuterColor = Color.Blue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor2, GetTrailWidth2, bloomTrailShader, Projectile.Size * 0.5f);
    }


    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.2f;
    }
    private float GetTrailWidth(float ratio)
    {
        float width = MathHelper.Lerp(1f, 0f, KillTimer / 30f);
        return MathHelper.SmoothStep(80, 0, ratio) * width;
    }
    private Color GetTrailColor2(float ratio)
    {
        return Color.White;
    }

    private Color GetTrailColor(float ratio)
    {
        Color trailColor = Color.SkyBlue;
        trailColor = Color.Lerp(trailColor, Color.Black, ratio);
        trailColor.A = 0;
        return trailColor;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedStarTrail, DrawLayer.BehindTiles); 
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

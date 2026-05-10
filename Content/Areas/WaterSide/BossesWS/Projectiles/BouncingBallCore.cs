using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class BouncingBall : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Parent => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 7 == 0)
        {
            var d = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero);
            d.fadeToColor = Color.Black * 0.1f;
            d.color = Color.DarkBlue * 0.1f;
        }

        if (Timer % 12 == 0)
        {
            var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pearlsand);
            Main.dust[d].noGravity = true;
        }


        Projectile.rotation += 0.1f;
        Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3() * 0.4f);
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(1f, 0f, ratio) * 128;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio) * 0.66f;
    }
    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        var shader = ShaderContent.GetInstance<BasicLaserAlphaShader>();
        shader.InnerColor = Color.DarkGray;
        shader.OuterColor = Color.Black;
        shader.LaserTexture = TrailRegistry.FlamingTrailNoBlack;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
        _outlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer e = SpritebatchDrawer.FromProjectile(Projectile);
            e.worldPosition = pos;
            e.color = Color.Lerp(Color.White, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.05f;
            Main.spriteBatch.Draw(e);
        }
        SpritebatchDrawer ballDraer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(ballDraer);

        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(_outlineTextureAsset, Projectile.Center);
        outlineDrawer.color = Color.Red;
        outlineDrawer.rotation = ballDraer.rotation;
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class BouncingBallCore : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Slavery => ref Projectile.ai[1];
    private ref float BounceTimer => ref Projectile.ai[2];
    private Vector2[] _offsets;
    private Projectile[] _children;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Slavery = 2;
            if (this.OwnedByLocalClient())
            {
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BouncingBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Slavery);
            }
        }

        float bounceTime = 90f;
        BounceTimer++;
        if (BounceTimer >= bounceTime)
        {
            BounceTimer = 0f;
        }
        _children ??= new Projectile[2];
        _offsets ??= new Vector2[2];
        for (int i = 0; i < _offsets.Length; i++)
        {
            ref Vector2 offset = ref _offsets[i];
            Vector2 maxOffset = -Vector2.UnitY * 196;
            maxOffset = maxOffset.RotatedBy(Timer * 0.05f);

            float halfTime = bounceTime / 2f;
            float inEasing = BounceTimer / halfTime;
            inEasing = EasingFunction.OutExpo(inEasing);
            float outEasing = (BounceTimer - halfTime) / halfTime;
            outEasing = EasingFunction.InExpo(outEasing);
            float mixedEasing = inEasing * MathHelper.Lerp(1f, 0f, outEasing);
            if (i == 1)
                maxOffset *= -1;
            offset = Vector2.Lerp(Vector2.Zero, maxOffset, mixedEasing);

        }

        Player nearestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
        if (nearestPlayer != null)
        {
            Vector2 velToPalyer = (nearestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity += velToPalyer * 0.2f;
        }

        Projectile.rotation += 0.05f;
        Projectile.rotation += Projectile.velocity.Length() * 0.02f;

        int index = 0;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (index >= _offsets.Length)
                break;
            if (proj.type != ModContent.ProjectileType<BouncingBall>())
                continue;
            if (proj.ai[1] != Slavery)
                continue;

            Vector2 targetPos = Projectile.Center + _offsets[index];
            Vector2 vel = targetPos - proj.Center;
            proj.velocity = vel;
            _children[index] = proj;
            index++;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = oldVelocity.X * -1;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = oldVelocity.Y * -1;
        return false;
    }

    private float GetTrailWidth(float ratio)
    {
        return 16;
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.DarkGreen;
    }

    private void DrawPixelatedThornTrail(GraphicsDevice gDevice)
    {
        if (_children == null)
            return;

        for (int i = 0; i < _children.Length; i++)
        {
            float numPoints = 32;
            Vector2 start = Projectile.Center;
            Vector2 end = _children[i].Center;
            Vector2[] points = CommonDrawing.InterpolateBetweenPoints(start, end, numPoints);

            HairShader shader = ShaderContent.GetInstance<HairShader>();
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
            shader.WaveFrequency = 8;
            shader.XOffset = 12;
            TrailDrawer.Draw(Main.spriteBatch, points, GetTrailColor, GetTrailWidth, shader);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedThornTrail);
        SpritebatchDrawer ballCoreDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        ballCoreDrawer.scale = Vector2.One * EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        Main.spriteBatch.Draw(ballCoreDrawer);
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

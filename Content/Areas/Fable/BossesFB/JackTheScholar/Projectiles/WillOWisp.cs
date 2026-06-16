using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.JackTheScholar.Projectiles;

public class WillOWisp : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];

    private float _scale;
    private Vector2 InitialVelocity;
    private Vector2 TargetVelocity;
    private Player _target;
    private float Scale => 1.2f;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.light = 0.278f;
        Projectile.timeLeft = 180;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            InitialVelocity = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }
        if (Timer % 12 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer % 6 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer < 30 && _target == null || _target != null && !_target.active)
        {
            _target = PlayerHelper.FindClosestPlayer(Projectile.Center, maxDetectDistance: 1024);
        }
        if (Timer < 30)
        {
            _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.25f, 1.2f), EasingFunction.InCubic(Timer / 30f));
            Projectile.velocity *= 0.5f;
        }

        if (Timer == 30)
        {
            //Ping Sound
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        if (Timer == 90)
        {
            if (_target != null && _target.active)
            {
                TargetVelocity = Projectile.velocity = (_target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * InitialVelocity.Length();
            }
        }

        if (Timer > 90)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, TargetVelocity, 0.02f);
        }

        Projectile.scale = _scale;
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, alpha: 0.12f);
        SpritebatchDrawer flameDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        flameDrawer.scale *= Scale;
        Main.spriteBatch.Draw(flameDrawer);

        SpritebatchDrawer glintDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarGlint, Projectile.Center);
        glintDrawer.color = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
        glintDrawer.color.A = 0;
        glintDrawer.rotation = MathHelper.Lerp(1.54f, 0f, EasingFunction.InOutCirc(Timer / 30f));
        glintDrawer.scale *= MathHelper.Lerp(6, 0f, EasingFunction.InOutSine(Timer / 60f));
        Main.spriteBatch.Draw(glintDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.2f * ExtraMath.Osc(0.8f, 1f, speed: 12);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.25f;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 12; i++)
        {
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default, 1.5f);
            Dust dust = Main.dust[num];
            dust.noGravity = true;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
        }
        var part = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red);
        part.Scale *= 0.5f;
        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
    }


    public void DrawOutlines(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer flameDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        flameDrawer.scale *= Scale;
        flameDrawer.color = Color.Red;
        spriteBatch.Draw(flameDrawer);
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutlines);
    }
}

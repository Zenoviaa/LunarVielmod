using Stellamod.Content.Areas.Jungle.SporedomBoss.Gores;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class ThornyBounceBall : ModProjectile,
    IDrawToRenderTarget
{
    private float XSpeed => 5.5f;
    private float Tracking => 0.03f;
    private float Gravity => 0.3f;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Squish => ref Projectile.ai[1];
    private Vector2 Scale => Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(Squish));
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 500;
        Projectile.penetrate = -1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        foreach(OldPosition oldPosition in Projectile.IterateOldPosBackwards())
        {
            var pos = oldPosition.position + Projectile.Size * 0.5f;
            var afDrawer = Projectile.Drawer;
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.DarkGreen, Color.Transparent, oldPosition.progress) * 0.1f;
            afDrawer.rotation = Projectile.oldRot[oldPosition.index];
            Main.spriteBatch.Draw(afDrawer);
        }

        var drawer = Projectile.Drawer;
        drawer.scale *= Scale;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var shootSound = AssetReferences.Assets.Sounds.Jack_Land.Asset with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(shootSound, Projectile.position);
            var goreType = ModContent.GoreType<GreenFallenLeaf>();
            for (var f = 0; f < 4; f++)
            {
                var vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.6f, 1f);
                var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
            }
        }

        var nearestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if (nearestPlayer != null)
        {
            var direction = nearestPlayer.Center.X < Projectile.Center.X ? -1 : 1;
            var targetX = direction * XSpeed;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetX, Tracking);
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.08f;
        Projectile.velocity.Y += Gravity;
        Squish = MathHelper.Lerp(Squish, 0f, 0.1f);
    }

    private void BounceEffects()
    {
        Squish = 1;
        var goreType = ModContent.GoreType<GreenFallenLeaf>();
        for (var f = 0; f < 1; f++)
        {
            var vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.6f, 1f);
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
        }

        for (float f = 0; f < 5f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.25f;
            p.color = Color.Orange * 0.4f;
        }

        var shootSound = AssetReferences.Assets.Sounds.Jack_Land.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(shootSound, Projectile.position);

        var p3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
        p3.Scale *= 0.4f;
        p3.fadeToColor = Color.DarkGreen;
        p3.outerColor = Color.Goldenrod;
        p3.innerColor = Color.LightGreen;
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (ProjectileHelper.Bounce(Projectile, oldVelocity))
        {
            BounceEffects();
        }
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var shootSound = AssetReferences.Assets.Sounds.NaturalCast.Asset with { PitchVariance = 0.4f };
        SoundEngine.PlaySound(shootSound, Projectile.position);
        var goreType = ModContent.GoreType<GreenFallenLeaf>();
        for(var f = 0;f < 18; f++)
        {
            var vel = Main.rand.NextVector2Circular(8, 8);
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1.4f));
        }

        for (float f = 0; f < 7f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.25f;
            p.color = Color.Orange * 0.4f;
        }
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue((SpriteBatch sb) => Main.spriteBatch.Draw(Projectile.Drawer with { color = Color.Red, scale = Scale }));
    }
}

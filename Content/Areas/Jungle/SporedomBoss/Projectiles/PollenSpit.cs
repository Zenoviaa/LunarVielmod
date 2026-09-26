using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;


public class CorePellet : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    private ref float Explode => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 450;
        Projectile.light = 0.78f;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity *= 0.96f;
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        if(Explode >= 7)
        {
            Projectile.Kill();
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            for(float f = 0; f < 14; f++)
            {
                var firer = ProjFirer.From<FallingPollen>(Projectile);
                firer.velocity.X = Main.rand.NextFloat(-16, 16);
                firer.velocity.Y -= 12;
                firer.New();
            }
        }
    }
}

public class SmallPellet : ModProjectile
{
    private Vector2 FindTargetToMoveTo
    {
        get
        {
            var type = ModContent.ProjectileType<CorePellet>();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != type)
                    continue;
                if (proj.ai[0] == Parent.whoAmI)
                {
                    return proj.Center;
                }
            }
            return Projectile.Center;
        }
    }
    private ref float InitialSpeed => ref Projectile.ai[2];
    private ref float Timer => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 240;
    }

    public override void AI()
    {
        base.AI();
        if(Timer == 1)
        {
            InitialSpeed = Projectile.velocity.Length();
        }
        var targetToMoveTo = FindTargetToMoveTo;
        var targetDirection = targetToMoveTo - Projectile.Center;
        targetDirection = targetDirection.SafeNormalize(Vector2.Zero);
        var targetVelocity = targetDirection * InitialSpeed;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.03f);
        var distanceSquaredToTarget = Vector2.DistanceSquared(Projectile.Center, targetToMoveTo);
        var isCloseEnough = distanceSquaredToTarget < 24 * 24;
        if (isCloseEnough)
        {
            Eat();
        }
    }

    private void Eat()
    {
        var type = ModContent.ProjectileType<CorePellet>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type != type)
                continue;
            if (proj.ai[0] == Parent.whoAmI)
            {
                proj.ai[2]++;
                proj.netUpdate = true;
                break;
            }
        }
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class FallingPollen : ModProjectile
{
    private float Lifetime => 300;
    private float LifeRatio => Timer / Lifetime;
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandScale => ref Projectile.ai[1];
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        RandScale = Main.rand.NextFloat(0.6f, 1f);
    }
    
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Lifetime;
        Projectile.penetrate = -1;
        Projectile.light = 0.78f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(16))
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.125f;
            p.color = Color.Orange * 0.4f;
        }

        if (MathF.Abs(Projectile.velocity.X) > 1)
            Projectile.velocity *= 0.96f;
        Projectile.velocity.X += MathF.Sin(Timer * 0.03f) * 0.06f;
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 0.1f, 0.1f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InSine(LifeRatio));
        drawer.scale *= RandScale;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class ThornyBounceBall : ModProjectile
{
    private float XSpeed => 4;
    private float Tracking => 0.03f;
    private float Gravity => 0.3f;
    private ref float Timer => ref Projectile.ai[0];
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
        Projectile.timeLeft = 240;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();
        var nearestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 1024);
        if (nearestPlayer != null)
        {
            var direction = nearestPlayer.Center.X < Projectile.Center.X ? -1 : 1;
            var targetX = direction * XSpeed;
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetX, Tracking);
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.velocity.Y += Gravity;
    }

    private void BounceEffects()
    {
        for (float f = 0; f < 5f; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 3);
            var p = Particle<ThickSmokeParticle>.Spawn(pos, velocity, Color.DarkGray);
            p.Scale *= 0.25f;
            p.color = Color.Orange * 0.4f;
        }

        var p3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
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
}

[Autoload(Side = ModSide.Client)]
public class PollenSpitRenderer : ModSystem
{
    public static readonly Queue<Action<SpriteBatch>> DrawActionQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderPollenSpit;
    }

    private void RenderPollenSpit(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (DrawActionQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPollenSpit);
        }
    }

    private void DrawPollenSpit(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        spriteBatch.EndOut(out var oldParameters);
        using var maskTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(maskTarget, Color.Transparent))
        {
            using (spriteBatch.Ctx(oldParameters with { matrix = Matrix.identity }))
            {
                while (DrawActionQueue.Count > 0)
                {
                    DrawActionQueue.Dequeue()(spriteBatch);
                }
            }
        }


        var pollenMixPass = AssetReferences.Effects.Foresty.PollenMix.CreateBlackPass();
        pollenMixPass.Parameters.time = Main.GlobalTimeWrappedHourly;

        var noiseTexture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset.Value;
        pollenMixPass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointClamp,
            Texture = noiseTexture
        };

        pollenMixPass.Parameters.darkColor = new Color(242, 92, 0).ToVector4();
        pollenMixPass.Parameters.lightColor = new Color(255, 189, 162).ToVector4();
        pollenMixPass.Parameters.spriteSize = maskTarget.Target.Size();
        pollenMixPass.Parameters.noiseTexelSize = noiseTexture.GetTexelSize();
        pollenMixPass.Apply();
        using (spriteBatch.Ctx(oldParameters with { effect = pollenMixPass.Shader }))
        {
            spriteBatch.Draw(maskTarget, Vector2.Zero, Color.White);
        }
        spriteBatch.Begin(oldParameters);

    }
}
public class PollenSpit : ModProjectile,
    IDrawToRenderTarget
{
    private float XSlowing => 0.99f;
    private float MaxFallSpeed => 15;
    private float Gravity => 0.3f;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity.X *= XSlowing;
        if (Projectile.velocity.Y < MaxFallSpeed)
            Projectile.velocity.Y += Gravity;
        if (Timer % 16 == 0)
        {
            var offset = Main.rand.NextVector2Circular(16, 16);
            var pos = Projectile.Center + offset;
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, DustID.GemTopaz, vel, Scale: Main.rand.NextFloat(0.2f, 0.6f));
        }

        if (Timer % 8 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 384f, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Orange, Color.Gold, Main.rand.NextFloat(0f, 1f));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (var f = 0; f < 4; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = Color.Gold.ToVector4(),
                outerColor = Color.DarkOrange.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }

        for (var f = 0; f < 8; f++)
        {
            DustParticle.Spawn(Projectile.Center,
                -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = Color.Gold,
                    outerColor = Color.DarkOrange,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 0.7f)
                });
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    private void PollenDraw(SpriteBatch spriteBatch)
    {
        var drawer = Projectile.Drawer;
        var startScale = new Vector2(1.1f, 0.9f);
        var endScale = new Vector2(0.9f, 1.1f);
        var scale = Vector2.One;
        scale.X = MathHelper.Lerp(startScale.X, endScale.X, ExtraMath.Osc(0f, 1f, speed: 2));
        scale.Y = MathHelper.Lerp(startScale.Y, endScale.Y, ExtraMath.Osc(0f, 1f, speed: 2, offset: 3.14f));
        drawer.scale *= scale;
        spriteBatch.Draw(drawer);
    }

    public void DrawToRenderTargets()
    {
        PollenSpitRenderer.DrawActionQueue.Enqueue(PollenDraw);
        OutlineRenderer.Queue(PollenDraw);
    }
}

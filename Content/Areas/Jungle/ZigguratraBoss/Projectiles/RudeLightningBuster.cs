using Stellamod.Common.Particles;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;

public class RudeLightningBuster : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeLightningSpikeySpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Orange;
        Particles.LightningSpikeySpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            scale = Main.rand.NextFloat(0.4f, 0.8f),
            color = color,
            timeLeft = 100
        });
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(32);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(8))
        {
            MakeLightningSpikeySpark(Projectile.Center + Main.rand.NextVector2Circular(32, 32));
        }

        if (Main.rand.NextBool(16))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, DustID.AmberBolt, vel, Scale: Main.rand.NextFloat(0.6f, 1.2f));
        }

        if (Main.rand.NextBool(4))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(3, 3);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                innerColor = Color.LightGoldenrodYellow.ToVector4(),
                outerColor = Color.DarkOrange.ToVector4(),
                position = pos,
                velocity = vel,
                timeLeft = Main.rand.NextFloat(40, 120),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),
            });
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSlash, DrawLayer.OverPlayers);
        return false;
    }

    void DrawPixelatedSlash(SpriteBatch sb, Vector2 sp)
    {
        var drawer = Projectile.Drawer;
        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            var afDrawer = drawer;
            afDrawer.Apply(Projectile, oldPos);
            afDrawer.color = Color.Lerp(Color.Gold, Color.Transparent, oldPos.progress) * 0.3f;
            afDrawer.color.A = 0;
            sb.Draw(afDrawer);
        }

        var noiseTexture = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset;
        var pass = AssetReferences.Effects.Electric.RudeLightningSlash.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSize = TextureAssets.Projectile[Type].Size();
        pass.Parameters.noiseTexelSize = noiseTexture.Value.GetTexelSize();
        pass.Parameters.distortionStrength = 0.03f;
        pass.Parameters.noiseSampler = new HlslSampler
        {
            Texture = noiseTexture.Value,
            Sampler = SamplerState.PointWrap
        };
        pass.Apply();

        //No point to batch this, bro doesn't spam this projectile
        //Just drawing like this should be fine.
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader }))
        {
            drawer.color *= ExtraMath.Osc(0.9f, 1f, speed: 16);
            sb.Draw(drawer);
        }

    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<RudeLightningCrash>(Projectile);
            firer.velocity = Vector2.Zero;
            firer.New();
        }
    }
}

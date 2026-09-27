using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class CorePellet : ModProjectile
{
    private Color PollenLightColor => Color.Lerp(Color.Gold, Color.Black, 0.8f);
    private Color PollenDarkColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.8f);
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    private ref float Explode => ref Projectile.ai[2];
    private Vector2 WigglyScale => Vector2.Lerp(new Vector2(1.1f, 0.9f), new Vector2(0.9f, 1.1f), ExtraMath.Osc(0f, 1f, speed: 2, offset: Projectile.whoAmI));
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
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
        if (Timer % 16 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2() * MathHelper.Lerp(0, 32, Main.rand.NextFloat(0f, 1f));
            var sp = FaintSmokeParticle.Spawn(Projectile.Center + offset, Main.rand.NextVector2Circular(15, 15));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = PollenDarkColor;

            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.Scale *= 0.3f;
            sp.dampening = 0.05f;
        }

        Projectile.velocity *= 0.96f;
        if(Explode >= 5)
        {
            Projectile.frame = 2;
        } else if (Explode >= 2)
        {
            Projectile.frame = 1;
        }
        else
        {
            Projectile.frame = 0;
        }
        if(Explode >= 7)
        {
            Projectile.Kill();
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.scale *= WigglyScale;
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

        FXUtil.GlowCircleBoom(Projectile.Center, Color.Gold, Color.Gold, Color.DarkOrange, duration: 0.23f, baseSize: 0.20f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = PollenLightColor.ToVector4(),
                outerColor = PollenDarkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }

        for (var f = 0; f < 16; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center,
                -Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = PollenLightColor,
                    outerColor = PollenDarkColor,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 0.7f)
                });
            dp.dampening = 0.05f;
        }
        var explodeSound = AssetReferences.Assets.Sounds.WetDeath.Asset with { PitchVariance = 0.8f };
        SoundEngine.PlaySound(explodeSound, Projectile.position);
    }
}

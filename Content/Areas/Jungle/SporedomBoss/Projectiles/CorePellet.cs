using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Jungle.SporedomBoss.Gores;
using Stellamod.Core;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class CorePellet : ModProjectile
{
    private Vector2 _pulseShakeOffset;
    private float _pulseAlpha;
    private float _lastExplode;
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


    private void WiggleEffect()
    {
        for (var i = 0; i < 16; i++)
        {
            var up = -Vector2.UnitY * 12;
            up = up.RotatedByRandom(6.28f);
            var dp = DustParticle.Spawn(Projectile.Center,
                up * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = PollenLightColor,
                    outerColor = PollenDarkColor,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 0.7f)
                });
            dp.dampening = 0.04f;
        }
        _pulseAlpha = 1f;
    }

    public override void AI()
    {
        base.AI();
        _pulseAlpha = MathHelper.Lerp(_pulseAlpha, 0f, 0.08f);
        if(_pulseAlpha > 0.3f)
        {
            if(Timer % 4 == 0)
            {
                _pulseShakeOffset = Main.rand.NextVector2Circular(4, 4);
            }
        }
        else
        {
            _pulseShakeOffset = Vector2.Zero;
        }
        if(_lastExplode != Explode) 
        {
            WiggleEffect();
            _lastExplode = Explode;
        }
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
        drawer.scale *= MathHelper.Lerp(1f, 1.2f, EasingFunction.OutExpo(_pulseAlpha));
        drawer.worldPosition += _pulseShakeOffset;
        drawer.rotation += MathHelper.Lerp(-0.05f, 0.05f, _pulseAlpha);
        drawer.worldPosition.Y += ExtraMath.Osc(0f, 16f, 2);
        Main.spriteBatch.Draw(drawer);

        var pusleDrwaer = drawer;
        pusleDrwaer.color = Color.Lerp(Color.Transparent, Color.Yellow, _pulseAlpha);
        pusleDrwaer.color.A = 0;
        Main.spriteBatch.Draw(pusleDrwaer);
        return false;
    }
    
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            for(float f = 0; f < 20; f++)
            {
                var firer = ProjFirer.From<FallingPollen>(Projectile);
                firer.velocity.X = Main.rand.NextFloat(-54, 54);
                firer.velocity.Y -= 12;
                firer.New();
            }
        }

        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.Gold, Color.DarkOrange, 25, 256);
        FXUtil.GlowCircleBoom(Projectile.Center, Color.Gold, Color.Gold, Color.DarkOrange, duration: 0.23f, baseSize: 0.20f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = PollenLightColor.ToVector4(),
                outerColor = PollenDarkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 3f)),
                timeLeft = 120
            });
        }
        var goreType = ModContent.GoreType<GreenFallenLeaf>();
        for (var f = 0; f < 12; f++)
        {
            var vel = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.6f, 1f);
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
        }
        goreType = ModContent.GoreType<WhiteFallenPetal>();
        for (var f = 0; f < 12; f++)
        {
            var vel = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.6f, 1f);
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
        }
        for (var f = 0; f < 32; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center,
                Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = PollenLightColor,
                    outerColor = PollenDarkColor,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 2f)
                });
            dp.dampening = 0.05f;
        }
        var explodeSound = AssetReferences.Assets.Sounds.WetDeath.Asset with { PitchVariance = 0.8f };
        SoundEngine.PlaySound(explodeSound, Projectile.position);
    }
}

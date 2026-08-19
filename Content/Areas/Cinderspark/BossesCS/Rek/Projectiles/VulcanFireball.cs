using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class VulcanFireball : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 140;
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        if (Main.rand.NextBool(4))
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    break;
                case 1:
                    FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                    sp2.gravity = 0f;
                    sp2.fast = true;
                    sp2.dampening = 0.1f;
                    break;
            }
        }

        Projectile.velocity.Y += 0.08f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.05f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        RekFireballShader shader = ShaderContent.GetInstance<RekFireballShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * 3;
        shader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        shader.InnerColor = Color.Lerp(Color.Orange, Color.OrangeRed, 0.5f);
        shader.BloomColor = Color.Red;
        shader.Strength = 0.3f;
        var sbParams = SpritebatchParams.InWorldAndZoomed();
        sbParams.effect = shader.Effect;
        using(SpritebatchStarter.Begin(Main.spriteBatch, sbParams))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);

            drawer.color = Color.OrangeRed * 0.5f;
            drawer.color.A = 0;
            drawer.scale *= 1.3f;
            Main.spriteBatch.Draw(drawer);
        }
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
        for (float n = 0; n < 2f; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.OrangeRed;
            spawnParams.outerColor = Color.Red;
            spawnParams.scaleRange = new Vector2(0.1f, 3f);
            DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
        }

        SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
        sp.initialColor = Color.White * 0.14f;
    }
}

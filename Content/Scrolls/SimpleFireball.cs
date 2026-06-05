using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class SimpleFireball : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 18;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();

        Timer++;
        if(Timer == 1)
        {
            SoundStyle fireSound;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1");
                    break;
                case 1:
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot2");
                    break;
            }
            fireSound = fireSound with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(fireSound, Projectile.position);
        }

        Projectile.velocity.Y += 0.25f;
        if (Timer > 4)
            Projectile.tileCollide = true;
        FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(0.2f, 0.35f));
        dp.innerColor = Color.Goldenrod;
        dp.outerColor = Color.Red;
        dp.parent = Projectile;
        dp.gravity = 0f;
        dp.dampening = 0.05f;
        dp.fast = true;
        if (Main.rand.NextBool(5))
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

        if (Main.rand.NextBool(8))
        {
            FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
            sp.gravity = 0f;
            sp.fast = true;
            sp.dampening = 0.1f;
        }
    }


    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserColor = Color.Yellow;
        shader2.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader2.InnerColor = Color.Red;
        shader2.OuterColor = Color.OrangeRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Red;
        bloom.OuterColor = Color.OrangeRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }
    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.Red;
        Color trailColor = Color.Lerp(Color.DarkRed, Color.OrangeRed, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(40, 2, completionRatio);
    }
    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 1.3f;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
       // throw new NotImplementedException();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowDrawOrigin = glowMask.Size() / 2f;
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);
        // spriteBatch.RestartDefaults();


        glowMask = AssetManager.GlowMask.SpiralVortex.Value;
        glowDrawOrigin = glowMask.Size() / 2f;
        glowColor = Color.Red;
        glowColor.A = 0;
        spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);

        return false;
        //    return base.PreDraw(ref lightColor);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (this.OwnedByLocalClient())
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = -Projectile.oldVelocity;
                vel *= Main.rand.NextFloat(0.05f, 0.3f);
                Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1f, 3f));
            }


            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.OrangeRed, Color.DarkRed, 45, 0.15f);
            fx.Scale *= Main.rand.NextFloat(1f, 1.2f);
            float numDust = 16;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = -Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Red;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;
            }

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 vel = -(Projectile.oldPos[i] - Projectile.oldPos[i + 1]);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(25));
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(2, 7);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.innerColor = Color.OrangeRed;
                    spawnParams.outerColor = Color.DarkRed;
                    spawnParams.scaleRange *= 0.4f;
                    var dp = DustParticle.Spawn(Projectile.oldPos[i] + Projectile.Size * 0.5f, vel, spawnParams);
                    dp.fast = true;
                    dp.noTileCollide = true;
                    dp.dampening = 0.05f;
                    dp.gravity = 0;

                }
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity,
                ModContent.ProjectileType<EternalFlamePile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        return base.OnTileCollide(oldVelocity);
    }
}

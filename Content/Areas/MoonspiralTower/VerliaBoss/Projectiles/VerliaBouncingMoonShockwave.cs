using Stellamod.Assets;
using Stellamod.Core.Palettes;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;

public class VerliaBouncingMoonShockwave : ModProjectile
{
    private float Time => 120f;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 512;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = (int)Time;
    }

    public override void AI()
    {
        base.AI();
        if(Timer > 12)
        {
            Projectile.hostile = false;
        }
        Timer++;
        if (Timer == 1)
        {
            float numDust = 32;
            for (float f = 0; f < numDust; f++)
            {
                Vector2 spawnPos = Projectile.Center;
                spawnPos.X += Main.rand.NextFloat(-128, 128);
                spawnPos.Y += Main.rand.NextFloat(-2f, 2f);
                Vector2 velocity = -Vector2.UnitY * 2;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
                velocity *= Main.rand.NextFloat(1f, 15f);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.LightSkyBlue;
                var dp = DustParticle.Spawn(spawnPos, velocity, spawnParams);
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.Scale *= 2;
            }

            float numSparkles = 16;
            for (float f = 0; f < numSparkles; f++)
            {
                Vector2 spawnPos = Projectile.Center;
                spawnPos.X += Main.rand.NextFloat(-128, 128);
                spawnPos.Y += Main.rand.NextFloat(-2f, 2f);
                Vector2 velocity = -Vector2.UnitY * 2;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
                velocity *= Main.rand.NextFloat(1f, 15f);

                var dp = SparkleParticle.Spawn(spawnPos, velocity);
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.Scale *= 1.5f;
                dp.outerColor = Color.Blue;
                dp.flickering = true;
            }

            if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
            {
                SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                effectsPlayer.darknessCurve = MathHelper.Lerp(0.5f, 0f, EasingFunction.InExpo(Timer / Time));
            }
            SoundStyle impactSound = AssetRegistry.Sounds.Verlia.MoonDuoHitGround;
            SoundEngine.PlaySound(impactSound);

            ShakeModSystem.Shake = 16;
            FXUtil.ShakeCamera(Projectile.Center, 2048, 32);
            //     FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 32, 2, 32);
        }

    }
    public override bool PreDraw(ref Color lightColor)
    {
        VerliaShockwaveShader shockwaevShader = VerliaShockwaveShader.Instance;
        shockwaevShader.Time = -Timer * 0.02f + 0.8f;
        SpriteBatch sb = Main.spriteBatch;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 3.8f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(8f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += Projectile.height;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1.9f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(4f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += Projectile.height;
        Main.spriteBatch.Draw(sbDrawer);

        sb.RestartDefaults();

        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowLineDrawer.worldPosition.Y += Projectile.height;
        glowLineDrawer.scale.X *= MathHelper.Lerp(1f, 8f, EasingFunction.OutExpo(Timer / Time));
        glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

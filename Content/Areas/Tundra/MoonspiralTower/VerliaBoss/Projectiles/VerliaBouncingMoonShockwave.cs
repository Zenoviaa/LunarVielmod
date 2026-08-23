using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Palettes;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class ShockwavePlayer : ModPlayer
{
    public Vector2 shockwavePosition;
    public int rippleCount = 20;
    public int rippleSize = 5;
    public int rippleSpeed = 15;
    public float distortStrength = 300f;
    public float Bee = 220;
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Main.netMode == NetmodeID.Server)
            return;

        bool isActive = Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive();
        if (Bee > 0)
        {
            Bee--;
            if (!isActive)
            {
                Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", shockwavePosition).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).
                    UseTargetPosition(shockwavePosition);

            }

            if (isActive)
            {
                float progress = (180f - Bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(shockwavePosition);
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }
        else if (isActive)
        {
            Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
        }
    }
}
public class VerliaBouncingMoonShockwave : ModProjectile
{
    private float Time => 120f;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
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
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer > 18)
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
            if (Style == 0)
            {
                SoundStyle impactSound = AssetRegistry.Sounds.Verlia.MoonDuoHitGround;
                SoundEngine.PlaySound(impactSound);
            }
            else if (Style == 1)
            {
                SoundStyle impactSound = AssetRegistry.Sounds.Verlia.BigMoonExplosion;
                SoundEngine.PlaySound(impactSound);
            }
            else if (Style == 2)
            {
                SoundStyle impactSound = AssetRegistry.Sounds.Verlia.BigSwordHitDown;
                SoundEngine.PlaySound(impactSound);
            }
            if (Style == 1)
            {
                ShockwavePlayer shockwavePlayer = Main.LocalPlayer.GetModPlayer<ShockwavePlayer>();
                shockwavePlayer.Bee = 120;
                shockwavePlayer.shockwavePosition = Projectile.Center;
                shockwavePlayer.rippleSize = 5;
            }
            ShakeScreenPosition.Shake = 16;
            FXUtil.ShakeCamera(Projectile.Center, 2048, 32);
            //     FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 32, 2, 32);
        }

        if (Style == 1)
        {
            Main.LocalPlayer.GetModPlayer<ShockwavePlayer>().shockwavePosition = Projectile.Center;
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

        int height = 16;
        if(Style == 2)
        {
            height += 64;
        }
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 1.9f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(4f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color *= 0.5f;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);

     

        sb.RestartDefaults();

        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowLineDrawer.worldPosition.Y += height;
        glowLineDrawer.scale.X *= MathHelper.Lerp(1f, 8f, EasingFunction.OutExpo(Timer / Time));
        glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);


        if (Style == 2)
        {
            glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowLineDrawer.worldPosition.Y += height;
            glowLineDrawer.scale.X *= MathHelper.Lerp(6, 8f, EasingFunction.OutExpo(Timer / Time)) * 1.15f;
            glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
            glowLineDrawer.color = Color.Blue;
            glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
            glowLineDrawer.color.A = 0;
            Main.spriteBatch.Draw(glowLineDrawer);


            glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowLineDrawer.worldPosition.Y += height;
            glowLineDrawer.scale.X *= MathHelper.Lerp(6, 8f, EasingFunction.OutExpo(Timer / Time)) * 0.8f;
            glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.5f;
            glowLineDrawer.color = Color.White;
            glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
            glowLineDrawer.color.A = 0;
            Main.spriteBatch.Draw(glowLineDrawer);

        }
        if (Style == 1)
        {
            float outRatio = Timer / Time;

            string path = $"Stellamod/Content/Areas/MoonspiralTower/VerliaBoss/VerlianSigil";
            Asset<Texture2D> sigilTextureAsset = ModContent.Request<Texture2D>(path);

            SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(sigilTextureAsset, Projectile.Center);
            waveDrawer.rotation = 0;
            waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 2f, EasingFunction.InOutSine(outRatio));
            waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.QuadraticBump(outRatio));
            waveDrawer.color.A = 0;
            waveDrawer.worldPosition.Y -= 128;
            Main.spriteBatch.Draw(waveDrawer);
        }
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

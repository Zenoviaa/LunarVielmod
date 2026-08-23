using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss.Projectiles;

public class CariyaSwordFall : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 600;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        if (Timer > 26)
        {
            
            Projectile.hostile = false;
        }
        Timer++;
        if(Timer == 1)
        {
            SoundStyle downSlash = AssetRegistry.Sounds.Cariya.CarianDownslash with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(downSlash, Projectile.position);
        }
        if(Timer == 1)
        {
            if(Main.netMode != NetmodeID.Server)
            {
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Blue, 0.1f, 15f);
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 64);
            ShakeScreenPosition.Shake = 4;
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue, duration: 45);
            fx.Scale *= 3f;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue, duration: 45);
            fx2.Scale *= 1.8f;


            var fx3 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue, duration: 45);
            fx3.Scale *= 1.8f;
            fx3.VectorScale.X *= 8;
            fx3.VectorScale.Y *= 0.5f;

            for (float f = 0; f < 64; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(24, 24);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
                spawnParams.outerColor = Color.Blue;
                var d = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                d.dampening = 0.05f;
                d.gravity = 0;
                d.noTileCollide = true;
                d.Scale *= 1.5f;

            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Main.spriteBatch.Restart(effect: shearShader.Effect);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkTurquoise * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);
        Main.spriteBatch.RestartDefaults();


        float Time = 50f;
        float target = -Timer * 0.02f + 0.8f;
        VerliaShockwaveShader shockwaevShader = VerliaShockwaveShader.Instance;
        shockwaevShader.Time = MathHelper.Lerp(0, target, EasingFunction.InExpo(Timer / Time));

        SpriteBatch sb = Main.spriteBatch;
        sb.Restart(effect: shockwaevShader.Effect);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 5f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 1f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color = Color.Blue;
        sbDrawer.color.A = 0;

        int height = 16;
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);


        sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center);
        sbDrawer.BottomCenterOrigin();
        sbDrawer.scale.X *= MathHelper.Lerp(0f, 3f, EasingFunction.OutExpo(Timer / Time));
        sbDrawer.scale.Y += MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / Time));
        sbDrawer.scale.Y *= MathHelper.Lerp(0.2f, 2f, EasingFunction.QuadraticBump(Timer / Time));
        sbDrawer.color = Color.White;
        sbDrawer.color.A = 0;
        sbDrawer.worldPosition.Y += height;
        Main.spriteBatch.Draw(sbDrawer);



        sb.RestartDefaults();


        SpritebatchDrawer glowLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowLineDrawer.worldPosition.Y += height;
        glowLineDrawer.scale.X *= MathHelper.Lerp(1f, 4f, EasingFunction.OutExpo(Timer / Time));
        glowLineDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.2f;
        glowLineDrawer.color = Color.Blue;
        glowLineDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time));
        glowLineDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowLineDrawer);
        glowLineDrawer.scale.X *= 0.5f;
        glowLineDrawer.color = Color.White;
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

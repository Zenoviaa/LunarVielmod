using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class SpearBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private float AttackTime => 80;
    private float AttackProgress => EasingFunction.OutQuad(Timer / AttackTime);
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)AttackTime;
        Projectile.light = 0.78f;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        bool hasCollided = false;
        for(float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver4)
        {
            float lineWidth = 12;
            float collisionPoint = 0;
            Vector2 position = Projectile.Center;
            Vector2 previousPosition = position + Projectile.velocity.RotatedBy(f);
            hasCollided |= Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
            if (hasCollided)
                break;
        }

        return hasCollided;
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.TintScreen(Color.Red, 0.1f, timer: 60);
            shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 60);

            SoundStyle explosionSound = AssetRegistry.Sounds.Fire.Demoneatsyourmom with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(explosionSound);
            FXUtil.CreateRipple(Projectile.Center);
            ShakeScreenPosition.Shake = 6;

            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.LightGoldenrodYellow, Color.OrangeRed, 55, 450);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Red, Color.Black, duration: 25, baseSize: 0.2f);
            fx.Scale *= 3f;

            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.TintScreen(Color.Red, 0.12f, 5);
            var sound = AssetRegistry.Sounds.RekShockwave with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);

            var sound2 = new SoundStyle("Stellamod/Assets/Sounds/FireShockwave") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound2, Projectile.position);

            for(float r = 0; r < MathHelper.TwoPi; r += MathHelper.PiOver4)
            {
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = Projectile.velocity.RotatedBy(r);
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(5f, 45);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                    var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), vel);
                    dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                    dp.outerColor = Color.Red;
                    dp.noTileCollide = true;
                    dp.dampening = 0.05f;
                    dp.Scale *= Main.rand.NextFloat(1f, 1.5f);
                }

                for (float f = 0; f < 16; f++)
                {
                    Vector2 vel = Projectile.velocity.RotatedBy(r);
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(5f, 45);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Torch, vel, Scale: 2f);
                }

                for (float f = 0; f < 16; f++)
                {
                    Vector2 vel = Projectile.velocity.RotatedBy(r);
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(5f, 45);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Lava, vel, Scale: 2f);
                }
            }

        }
        FXUtil.ApplyContrast(MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / 45f)));
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (AttackProgress >= 0.95f)
            Projectile.Kill();
    }

    private void DrawTorchInner(SpriteBatch spriteBatch, Vector2 position, float rotation, float scale)
    {
        float fade = MathHelper.Lerp(1f, 0f, AttackProgress);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Lerp(Color.White, Color.OrangeRed, AttackProgress) * fade;
        drawer.color.A = 0;
        drawer.rotation = rotation;
        drawer.LeftCenterOrigin();
        drawer.worldPosition = position;
        drawer.scale *= MathHelper.SmoothStep(1f, 3f, AttackProgress);
        drawer.scale.Y *= MathHelper.SmoothStep(0, 1.5f, EasingFunction.OutExpo(AttackProgress));
        drawer.scale.X *= scale;
        spriteBatch.Draw(drawer);

        drawer.color = Color.DarkRed * fade;
        drawer.color.A = 0;
        drawer.scale *= 1.12f;
        spriteBatch.Draw(drawer);

        drawer.color = Color.DarkRed * fade;
        drawer.color.A = 0;
        drawer.scale *= 1.12f;
        drawer.scale.Y *= 0.8f;
        spriteBatch.Draw(drawer);
    }


    public override bool PreDraw(ref Color lightColor)
    {

        //DRAW THE TORCHH!!!!
        RekTorchShader torchShader = ShaderContent.GetInstance<RekTorchShader>();
        torchShader.Time = EasingFunction.OutExpo(AttackProgress);
        torchShader.Strength = MathHelper.Lerp(-0.5f, 0.5f, EasingFunction.OutSine(AttackProgress));
        torchShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        torchShader.InnerColor = Color.Yellow;
        torchShader.BloomColor = Color.Red;
        SpriteBatch spriteBatch = Main.spriteBatch;
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = torchShader.Effect };
        float length = TextureAssets.Projectile[Type].Value.Width;
        float scale = Projectile.velocity.Length() / length * 2f;

        void DrawTorches()
        {
            float lerpValue = Utils.GetLerpValue(0, AttackTime, Projectile.timeLeft);
            for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver4)
            {
                DrawTorchInner(spriteBatch, Projectile.Center, Projectile.rotation + f, scale);
            }
        }

        using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
        {
            DrawTorches();
        }

        torchShader.InnerColor = Color.White;
        torchShader.BloomColor = Color.Yellow;
        using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
        {
            DrawTorches();
        }

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

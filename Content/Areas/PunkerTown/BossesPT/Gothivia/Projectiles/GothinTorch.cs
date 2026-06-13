using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;


public class GothinTorch : ModProjectile,
    IDrawToRenderTarget
{
    private float Time
    {
        get
        {
            float t = 77;
            if (Variant == 1)
                t *= 1.35f;
            return t;
        }
    }
    private ref float Timer => ref Projectile.ai[0];
    private ref float NumDirections => ref Projectile.ai[1];
    private ref float Variant => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        if (NumDirections > 0)
        {
            for(int i = 0; i < NumDirections; i++)
            {
                float lineWidth = 12;
                float collisionPoint = 0;
                float rot = (float)i / (float)NumDirections;
                rot *= MathHelper.TwoPi;
                Vector2 newVel = Projectile.velocity.RotatedBy(rot);
                Vector2 pos = Projectile.Center;
                Vector2 attackPos = pos + newVel;
                bool colliding = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), pos, attackPos, lineWidth, ref collisionPoint);
                if (colliding)
                    return true;
            }
            return false;
        }
        else
        {
            float lineWidth = 12;
            if(Variant == 1)
            {
                lineWidth *= 20;
            }

            float collisionPoint = 0;
            Vector2 position = Projectile.Center;
            Vector2 previousPosition = position + Projectile.velocity;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
        }

    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
     
        Timer++;
        if(Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { PitchVariance = 0.5f }, Projectile.Center);
            SoundStyle fireballShoot = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(fireballShoot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
         
            if(Variant == 1)
            {
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.TintScreen(Color.OrangeRed, 0.1f, timer: 80);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.05f, timer: 80);
            }
            if(NumDirections > 0)
            {
                for(float r = 0; r < NumDirections; r++)
                {
                    float radians = (r / NumDirections) * MathHelper.TwoPi;
                    for (float f = 0; f < 8; f++)
                    {
                        Vector2 vel = Projectile.velocity;
                        vel = vel.SafeNormalize(Vector2.Zero);
                        vel *= Main.rand.NextFloat(5f, 100);
                        vel = vel.RotatedByRandom(MathHelper.ToRadians(24));
                        vel = vel.RotatedBy(radians);
                        var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), vel);
                        dp.gravity *= 0.5f;
                        dp.noTileCollide = true;
                        dp.dampening = 0.05f;
                        dp.Scale *= Main.rand.NextFloat(0.5f, 2f);
                    }
                }
            }
            else
            {
                for (float f = 0; f < 32; f++)
                {
                    Vector2 vel = Projectile.velocity;
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(5f, 100);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(24));
                    var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), vel);
                    dp.gravity *= 0.5f;
                    dp.noTileCollide = true;
                    dp.dampening = 0.05f;
                    dp.Scale *= Main.rand.NextFloat(0.5f, 2f);
                }
            }

        }

        if(Variant == 1 && Timer < 35)
        {
            ShakeScreenPosition.Shake = 4;
        }

        if(Variant == 1)
        {
            if(Timer >= 38)
            {
                Projectile.hostile = false;
            }
        }
        else
        {
            if (Timer >= 15)
            {
                Projectile.hostile = false;
            }
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    private void DrawBlowtorch(SpriteBatch spriteBatch, Vector2 sp)
    {
        float progress = Timer / Time;
        BlowTorchShader torchShader = ShaderContent.GetInstance<BlowTorchShader>();
        torchShader.Time = EasingFunction.OutExpo(progress);
        torchShader.FlameNoiseTexture = AssetManager.Noise.FlameVortexNoise;

        Color bloomColor = Color.Lerp(Color.Red, Color.Blue, EasingFunction.OutExpo(progress));
        torchShader.BloomColor = Color.Lerp(bloomColor, Color.Black, EasingFunction.InExpo(progress));
        torchShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutExpo(progress));

        //Drawing all the blowtorches in one projectile so it's optimized and not restarting the spritebatch 8 times
        //Also not eating up projectile slots
        //I'm so smart guys
        spriteBatch.Restart(effect: torchShader.Effect);
        if (NumDirections > 0)
        {
            for (int i = 0; i < NumDirections; i++)
            {
                float rot = (float)i / (float)NumDirections;
                rot *= MathHelper.TwoPi;
                Vector2 newVel = Projectile.velocity.RotatedBy(rot);
                DrawBlowtorchInner(spriteBatch, sp, newVel, progress);
            }
        }
        else
        {
            DrawBlowtorchInner(spriteBatch, sp, Projectile.velocity, progress);
        }
        spriteBatch.RestartDefaults();
    }

    private void DrawBlowtorchInner(SpriteBatch spriteBatch, Vector2 sp, Vector2 direction, float progress)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.Origin(0.1f, 0.5f);
        glowDrawer.scale.X *= MathHelper.Lerp(1.5f, 4.5f, EasingFunction.OutExpo(progress));
        glowDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.OutExpo(progress));
        if (Variant == 1)
            glowDrawer.scale *= 3;

        glowDrawer.color = Color.White;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = direction.ToRotation();

 
        spriteBatch.Draw(glowDrawer);

        glowDrawer.color = Color.DarkRed;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 5;
        spriteBatch.Draw(glowDrawer);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBlowtorch);
    }
}
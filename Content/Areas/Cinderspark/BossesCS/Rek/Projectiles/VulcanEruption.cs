using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Effects.RekFlames;
using Stellamod.Visual.Particles;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.RekBoss;
using static Terraria.GameContent.Animations.Actions.NPCs;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class VulcanEruption : ModProjectile
{
    private float _glowAlpha;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private int SegmentIndex => (int)Projectile.ai[1];
    private ref float Timer => ref Projectile.ai[2];

    private float TelegraphTime => 60;
    private float AttackTime => 30;

    private float TelegraphProgress => EasingFunction.Clamp(Timer / TelegraphTime);
    private float AttackProgress => EasingFunction.Clamp((Timer - TelegraphTime) / AttackTime);
    private RekSegment Segment
    {
        get
        {
            if(Parent.ModNPC is RekBoss rek)
            {
                return rek.Segments[SegmentIndex];
            }
            return default;
        }
    }

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
        Projectile.timeLeft = 90;
        Projectile.light = 0.78f;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 12;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = position + Projectile.velocity;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target) && Timer > TelegraphTime;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        Timer++;

        Segment.isBurning = true;
        if(Timer == TelegraphTime)
        {
            var sound = new SoundStyle("Stellamod/Assets/Sounds/RekFireballShoot") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);

            var sound2 = new SoundStyle("Stellamod/Assets/Sounds/FireShockwave") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound2, Projectile.position);

            FXUtil.ShakeCamera(Projectile.position, 1024, 8);
            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), vel);
                dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
                dp.outerColor = Color.Red;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            }
            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Torch, vel, Scale: 2f);
            }
            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 45);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 192) + new Vector2(0, -64), DustID.Lava, vel, Scale: 2f);
            }
        }

        if(Timer < TelegraphTime)
        {
            Projectile.Center = Segment.position;
            Projectile.rotation = Projectile.velocity.ToRotation();
            _glowAlpha = MathHelper.Lerp(_glowAlpha, 1f, 0.1f);
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
            if (Main.rand.NextBool(2))
            {

            }
        }
        else
        {
            float localTimer = Timer - TelegraphTime;
            _glowAlpha = MathHelper.Lerp(_glowAlpha, 0f, 0.1f);
        }

    }

    public override bool PreDraw(ref Color lightColor)
    {

        //DRAW THE TORCHH!!!!
        if(Timer >= TelegraphTime)
        {
            RekTorchShader torchShader = ShaderContent.GetInstance<RekTorchShader>();
            torchShader.Time = EasingFunction.OutExpo(AttackProgress);
            torchShader.Strength = MathHelper.Lerp(-0.5f, 0.5f, EasingFunction.OutSine(AttackProgress));
            torchShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
            torchShader.InnerColor = Color.Yellow;
            torchShader.BloomColor = Color.Red;
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = torchShader.Effect };
            using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
            {
                SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
                drawer.color = Color.Lerp(Color.White, Color.OrangeRed, AttackProgress);
                drawer.color.A = 0;
                drawer.LeftCenterOrigin();
                drawer.scale *= MathHelper.SmoothStep(1f, 3f, AttackProgress);
                drawer.scale.Y *= MathHelper.SmoothStep(0, 1.5f, EasingFunction.OutExpo(AttackProgress));
                drawer.scale.X *= 2.8f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                drawer.scale.Y *= 0.8f;
                spriteBatch.Draw(drawer);
            }

            torchShader.InnerColor = Color.White;
            torchShader.BloomColor = Color.Yellow;
            using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
            {
                SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
                drawer.color = Color.Lerp(Color.White, Color.OrangeRed, AttackProgress);
                drawer.color.A = 0;
                drawer.LeftCenterOrigin();
                drawer.scale *= MathHelper.SmoothStep(1f, 3f, AttackProgress);
                drawer.scale.Y *= MathHelper.SmoothStep(0, 1.5f, EasingFunction.OutExpo(AttackProgress));
                drawer.scale.X *= 2;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                drawer.scale.Y *= 0.8f;
                spriteBatch.Draw(drawer);
            }
        }
        else
        {
            RekTorchShader torchShader = ShaderContent.GetInstance<RekTorchShader>();
            torchShader.Time = EasingFunction.OutExpo(TelegraphProgress);
            torchShader.Strength = MathHelper.Lerp(0.5f, -0.5f, EasingFunction.OutSine(TelegraphProgress));
            torchShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
            torchShader.InnerColor = Color.Yellow;
            torchShader.BloomColor = Color.Red;
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = torchShader.Effect };
            using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
            {
                SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
                drawer.color = Color.Lerp(Color.White, Color.OrangeRed, TelegraphProgress);
                drawer.color.A = 0;
                drawer.LeftCenterOrigin();
                drawer.scale *= MathHelper.SmoothStep(1.2f, 0.5f, TelegraphProgress);
                drawer.scale.Y *= MathHelper.SmoothStep(0, 2.5f, EasingFunction.OutExpo(TelegraphProgress));
                drawer.scale.X *= 1.02f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                drawer.scale.Y *= 0.8f;
                spriteBatch.Draw(drawer);
            }

            torchShader.InnerColor = Color.White;
            torchShader.BloomColor = Color.Yellow;
            using (var start = SpritebatchStarter.Begin(spriteBatch, @params))
            {
                SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
                drawer.color = Color.Lerp(Color.White, Color.OrangeRed, AttackProgress);
                drawer.color.A = 0;
                drawer.LeftCenterOrigin();
                drawer.scale *= MathHelper.SmoothStep(1f, 3f, AttackProgress);
                drawer.scale.Y *= MathHelper.SmoothStep(0, 2.5f, EasingFunction.OutExpo(AttackProgress));
                drawer.scale.X *= 2;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                spriteBatch.Draw(drawer);

                drawer.color = Color.DarkRed;
                drawer.color.A = 0;
                drawer.scale *= 1.12f;
                drawer.scale.Y *= 0.8f;
                spriteBatch.Draw(drawer);
            }
        }

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
        var glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.scale *= 0.48f;
        glowDrawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12)) * ExtraMath.Osc(0.5f, 0.75f, speed: 8) * _glowAlpha;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls.Projectiles;

public class GoldenScarab : ModProjectile,
    IDrawToRenderTarget
{
    private Asset<Texture2D> _goldenAuraTextureAsset;
    private float _hitCount;
    private enum AIState
    {
        Summon,
        Chase,
        Bounce
    }
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private ref float BounceTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 8;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.light = 0.7f;
    }
    public override void AI()
    {
        base.AI();

        Projectile.scale = EasingFunction.InOutSine((float)Projectile.timeLeft / 60f);
        Main.projFrames[Type] = 8;
        switch (State)
        {
            case AIState.Summon:
                AI_Summon();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Bounce:
                AI_Bounce();
                break;
        }
        DrawHelper.AnimateTopToBottom(Projectile, 3);
    }

    private void AI_Summon()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle summonSound = new SoundStyle("Stellamod/Assets/Sounds/HeatFeather") with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(summonSound, Projectile.position);
        }
        Projectile.velocity.X *= 0.9f;
        Projectile.velocity.Y = MathHelper.Lerp(-5f, 0f, EasingFunction.InOutExpo(Timer / 30f));
        Projectile.rotation = 0;
        if(Timer >= 30)
        {
            SwitchState(AIState.Chase);
        }
    }

    private void AI_Chase()
    {
        Timer++;

        NPC nearest = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
        if(nearest != null)
        {
            float targetRotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver2);
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRotation, 0.1f);

        }
        else
        {
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0f, 0.1f);
        }

        if (Main.rand.NextBool(32))
        {
            var dp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY);
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.Scale *= 0.5f;
            dp.outerColor = Color.Goldenrod;
        }

        if(nearest == null)
        {
            Projectile.velocity.X *= 0.9f;
            float targetY = MathF.Sin(Timer * 0.05f) * 0.2f;
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, targetY, 0.1f);
            return;
        }

        if (nearest.CanBeChasedBy())
        {
            Vector2 velToTarget = (nearest.Center - Projectile.Center);
            velToTarget = velToTarget.SafeNormalize(Vector2.Zero) * 25;
            Projectile.velocity = Projectile.velocity.MoveTowards(velToTarget,MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 30f)));
        }
    }
    private void BoomEffect()
    {
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, 6, baseSize: 0.17f);
        for (int i = 0; i < 7; i++)
        {
            var dp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(15, 15));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.outerColor = Color.Goldenrod;
        }
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.DarkGoldenrod, 15, 64);
    }
    private void AI_Bounce()
    {
        Timer++;
        if(Timer == 1)
        {
            BoomEffect();
        }

        BounceTimer = EasingFunction.QuadraticBump(Timer / 15);
        Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
        Projectile.velocity *= 0.85f;
        float targetRotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver2);
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRotation, 0.1f);
        if (Timer >= 15)
        {
            SwitchState(AIState.Chase);
        }
    }

    private void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        Projectile.netUpdate = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if(State != AIState.Bounce)
        {
            Projectile.velocity *= -2;
            SwitchState(AIState.Bounce);
        }
        

        _hitCount++;
        if(_hitCount >= 5)
        {
            BoomEffect();
            Projectile.Kill();
        }
        SoundStyle hitSound = AssetRegistry.Sounds.Magic.HolyCast1 with { PitchVariance = 0.6f };
       SoundStyle hitSound2 = AssetRegistry.Sounds.Magic.HolyCast2 with { PitchVariance = 0.6f };
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                SoundEngine.PlaySound(hitSound, target.position);
                break;
            case 1:
                SoundEngine.PlaySound(hitSound2, target.position);
                break;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _goldenAuraTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Aura");
        SpritebatchDrawer auraDrawer = SpritebatchDrawer.FromTextureAsset(_goldenAuraTextureAsset, Projectile.Center);
        auraDrawer.color = Color.Goldenrod * ExtraMath.Osc(0.2f, 0.5f, speed: 3) * Projectile.scale;
        auraDrawer.color.A = 0;
        auraDrawer.scale *= 0.8f;
        for (float f = 0; f < MathHelper.TwoPi; f += 0.4f)
        {
            Vector2 offset = (f+Main.GlobalTimeWrappedHourly).ToRotationVector2() * ExtraMath.Osc(3f, 5f);
            auraDrawer.worldPosition = Projectile.Center + offset;
            auraDrawer.color = Color.Goldenrod * ExtraMath.Osc(0.2f, 0.5f, speed: 3) * Projectile.scale * 0.12f;
            auraDrawer.color.A = 0;
            Main.spriteBatch.Draw(auraDrawer);

        }

        SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        for(int k = 0; k < Projectile.oldPos.Length; k++)
        {
            afDrawer.worldPosition = Projectile.oldPos[k] + Projectile.Size * 0.5f;
            afDrawer.rotation = Projectile.oldRot[k];
            afDrawer.color = Color.White * 0.02f * MathHelper.Lerp(1f, 0f, (float)k / (float)Projectile.oldPos.Length);
            afDrawer.color.A = 0;
         
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        float scaleMult = MathHelper.Lerp(1f, 1.5f, BounceTimer);
        drawer.scale *= scaleMult * 0.8f;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Gold * ExtraMath.Osc(0.1f, 0.3f, speed: 2, Projectile.whoAmI);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White * MathHelper.Lerp(0f, 1f, BounceTimer);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
        //   return base.PreDraw(ref lightColor);
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Gold * 0.5f;
        shader2.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Gold * 0.5f;
        bloom.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.Goldenrod, Color.DarkGoldenrod, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(32, 2, completionRatio) * Projectile.scale;
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 1.4f * Projectile.scale;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        //  throw new NotImplementedException();
    }
}

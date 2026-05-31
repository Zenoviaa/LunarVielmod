using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class RoyalMagicBeam : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float InitialRadians => ref Projectile.ai[1];
    private float TargetRadiansOffset => MathHelper.ToRadians(75);
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);

    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), EndPoint, Projectile.Center, 48, ref collisionPoint))
            return true;

        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 80;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            FXUtil.CreateRipple(Projectile.Center);
            FXUtil.CreateRipple(Projectile.Center);
            for(float n =0; n < 32; n++)
            {
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(5, 85));
                dp.outerColor = Color.Pink;
                dp.dampening = 0.1f;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.Scale *= 1.3f;
            }
            for (float n = 0; n < 32; n++)
            {
                var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(5, 85));
                dp.outerColor = Color.Pink;
                dp.dampening = 0.1f;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.Scale *= 1.3f;
            }
            for(float n = 0; n < 10; n++)
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, EndPoint, Main.rand.NextFloat(0f, 1f));
                RoyalMagicStarParticle.Spawn(pos + Main.rand.NextVector2Circular(32, 32), Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 45f));
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            InitialRadians = Projectile.velocity.ToRotation();
        }

        float ratio = Timer / 80f;
        float ease1 = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        float ease2 = MathHelper.Lerp(1f, 0, EasingFunction.OutExpo(ratio));
        float ease3 = MathHelper.Lerp(ease1, ease2, EasingFunction.InOutSine(ratio));
        float newRadians = Utils.AngleLerp(InitialRadians, InitialRadians + TargetRadiansOffset, ease3);
        Projectile.velocity = newRadians.ToRotationVector2();


        ShakeScreenPosition.Shake = MathHelper.Lerp(6, 0, ratio);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    private void DrawRoyalBeam(SpriteBatch sb, Vector2 screenPos)
    {
        RoyalMagicBeamShader beamShader = ShaderContent.GetInstance<RoyalMagicBeamShader>();
        beamShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        beamShader.Tiling = new Vector2(3f, 1f);
        beamShader.BloomColor = Color.Lerp(Color.Blue, Color.Violet, EasingFunction.OutExpo(Timer / 80f));
        beamShader.Distortion = 0.2f;
        beamShader.Time = Main.GlobalTimeWrappedHourly * 4;
        sb.Restart(SpriteSortMode.Immediate, effect: beamShader.Effect);
        
        SpritebatchDrawer beamDrawer = SpritebatchDrawer.FromTextureAsset(TrailRegistry.BeamTrail, Projectile.Center);
        beamDrawer.rotation = Projectile.velocity.ToRotation();
        beamDrawer.LeftCenterOrigin();
        beamDrawer.scale.X *= 3;
        beamDrawer.scale.Y *= MathHelper.Lerp(0.5f, 1f, EasingFunction.OutExpo(Timer / 80f));
        beamDrawer.scale.Y *= MathHelper.Lerp(0.15f, 0f, EasingFunction.InExpo(Timer / 80f));
        beamDrawer.color = Color.White * 0.5f;
        beamDrawer.color.A = 0;
        sb.Draw(beamDrawer);


        beamDrawer.scale.Y *= 2.5f;
        beamDrawer.color = Color.White * 0.1f;
        beamDrawer.color.A = 0;
        sb.Draw(beamDrawer);

        sb.RestartDefaults();
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawRoyalBeam);
    }
}

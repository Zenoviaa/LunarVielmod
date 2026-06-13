using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class FireTornado : ModProjectile, 
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 96;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = Projectile.Center + Projectile.velocity; ;

        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint))
            return true;
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    private void DrawPixelatedTornado(SpriteBatch sb, Vector2 sp)
    {

        FireTornadoShader fireShader = ShaderContent.GetInstance<FireTornadoShader>();
        fireShader.Time = Main.GlobalTimeWrappedHourly * 0.1f;
        fireShader.Resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        fireShader.GradientTopColor = new Color(224, 187, 122);
        fireShader.GradientBottomColor = new Color(59, 19, 13);
        fireShader.FlameyTexture = AssetManager.Noise.FlamethrowerNoise.Value;
        fireShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        sb.Restart(effect: fireShader);

        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        drawer2.color = Color.White;

        drawer2.BottomCenterOrigin();
        drawer2.scale.Y *= 2;
        float ease2 = Timer / 60f;
        ease2 = EasingFunction.InOutExpo(ease2);
        drawer2.scale *= MathHelper.Lerp(4f, 1f, ease2);
        drawer2.color = Color.Lerp( Color.Transparent, drawer2.color, ease2);

        float time = Timer - 540;
        float ease = time / 60f;
        ease = EasingFunction.InOutSine(ease);
        drawer2.scale *= MathHelper.Lerp(1f, 2f, ease);
        drawer2.color = Color.Lerp(drawer2.color, Color.Transparent, ease);
        sb.Draw(drawer2);

        sb.RestartDefaults();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTornado);
 //       throw new System.NotImplementedException();
    }
}

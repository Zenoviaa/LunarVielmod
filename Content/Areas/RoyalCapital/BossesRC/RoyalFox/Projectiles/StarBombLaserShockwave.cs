using Stellamod.Common.Shaders;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;

public class StarBombLaserShockwave : ModProjectile
{
    private float Time => 45;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Scale => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        RoyalShockwaveCircleShader shockwave = ShaderContent.GetInstance<RoyalShockwaveCircleShader>();
        Main.spriteBatch.Restart(effect: shockwave.Effect);
        SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float yScale = MathHelper.Lerp(0.2f, 2.3f, EasingFunction.OutExpo(Timer / Time)) * Scale;
        circleDrawer.scale.Y *= yScale;

        Color color = Color.Lerp(Color.Blue, Color.Pink, Scale);
        color *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / Time)) * 0.8f;

        circleDrawer.color = color;
        Main.spriteBatch.Draw(circleDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

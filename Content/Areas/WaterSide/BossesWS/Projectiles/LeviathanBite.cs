using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class LeviathanBite : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 60;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            var dp = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.5f);
            dp.Scale *= 1.5f;
        }
        if(Timer > 30)
        {
            Projectile.hostile = true;
        }
     //   Projectile.Center = Parent.Center;
     //   Projectile.velocity = Parent.velocity;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float time = 60f;
        float ratio = Timer / time;
        float ease = EasingFunction.QuadraticBump(ratio);
        float ease2 = EasingFunction.InExpo(ratio / 0.5f);
        float ease3 = EasingFunction.InOutSine(ratio);
        float radiansOffset = MathHelper.ToRadians(135);


        Vector2 scale = Vector2.Lerp(Vector2.One * 1.8f, Vector2.One, ease3) * 2;
        Color biteColor = Color.Red;
        SpritebatchDrawer biteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        biteDrawer.color = Color.Lerp(Color.Black, biteColor, ease);
        biteDrawer.color.A = 0;
        biteDrawer.rotation -= MathHelper.Lerp(radiansOffset, 0f, ease2);
        biteDrawer.BottomLeftOrigin();
        biteDrawer.scale = scale;
        Main.spriteBatch.Draw(biteDrawer);

        biteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        biteDrawer.color = Color.Lerp(Color.Black, biteColor, ease);
        biteDrawer.color.A = 0;
        biteDrawer.rotation += MathHelper.Lerp(radiansOffset, 0f, ease2);
        biteDrawer.spriteEffects = SpriteEffects.FlipVertically;
        biteDrawer.BottomLeftOrigin();
        biteDrawer.drawOrigin.Y = biteDrawer.sourceRect.Value.Height - biteDrawer.drawOrigin.Y;
        biteDrawer.scale = scale;
        Main.spriteBatch.Draw(biteDrawer);
        return false;
    //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

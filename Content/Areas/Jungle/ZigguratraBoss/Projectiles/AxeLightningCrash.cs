using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;

public class AxeLightningCrash : ModProjectile
{
    Color LightGoldenColor => Color.Lerp(Color.Gold, Color.Black, 0.5f);
    Color DarkGoldenColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.5f);
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.height = 64;
        Projectile.width = 1280;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.light = 1f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
    }
    void MakeLightningSpark(Vector2 pos)
    {
        var vel = -Vector2.UnitY;
        vel *= 6;
        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

        var color = Color.Lerp(LightGoldenColor, DarkGoldenColor, Main.rand.NextFloat(0f, 1f));
        color = Color.Lerp(color, Color.White, 0.6f);
        Particles.LightningSpark.Spawn(new()
        {
            position = pos,
            velocity = vel,
            color = color,
            scale = Main.rand.NextFloat(0.7f, 1.5f) * 0.3f,
            timeLeft = 60
        });

    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            //Lightning crash vfx
            Particles.LightningImpact.Spawn(new()
            {
                position = Projectile.Center,
                timeLeft = 60,
                color = Color.LightGoldenrodYellow,
                velocity = -Vector2.UnitY
            });
            for(var i = 0; i < 16; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(80, 32);
                var sp = SmokeParticle.SpawnInAlphaLayer(pos, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
                sp.initialColor = Color.Lerp(Color.DarkGray, Color.Black, 0.6f);
                sp.fadeToColor = Color.Black;
                sp.behindLayer = true;
                sp.Scale *= 3;
            }
            for(var i = 0; i < 24; i++)
            {
                MakeLightningSpark(Projectile.Center + Main.rand.NextVector2Circular(64, 64));
            }
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

using Stellamod.Common;
using Stellamod.Common.Metaballs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class LavaBall : ModProjectile
{
    public override string Texture => ModType.NoTexture;
    ref float Timer => ref Projectile.ai[0];
    ref float Radius => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 54;
        Projectile.height = 54;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
        Projectile.hostile = false;

    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity.X *= 0.98f;
        if(Projectile.velocity.Y < 15)
            Projectile.velocity.Y += 0.3f;
        if (Timer >= 90)
            Projectile.hostile = false;

        var ratio = EasingFunction.InOutSine((float)Projectile.timeLeft / 60F);
        var metaball = new Vector3(DrawUtilities.WorldToScreenCoordinates(Projectile.Center), Radius * ratio);
        LavaMetaballRenderer.metaballs.Add(metaball);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
}

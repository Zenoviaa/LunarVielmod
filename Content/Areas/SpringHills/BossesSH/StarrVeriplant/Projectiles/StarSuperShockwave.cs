using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.StarrVeriplant.Projectiles;

public class StarSuperShockwave : ModProjectile
{
    private Vector2[] _shockwavePos;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 16 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Scale: 0.5f);
        }

        Projectile.velocity *= 1.01f;
    }


    public override bool PreDraw(ref Color lightColor)
    {


        return false;
    }
}

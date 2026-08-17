using Stellamod.Core.ProjectileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

//Use a custom alser texture for this one, alll spiraly and cool
//Maybe we can find one on the internet?
//Actually where do people even get their laser textures
public class LaserBeamBombLaser : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}


public class VulcanLaserBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
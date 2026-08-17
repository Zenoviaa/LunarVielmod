using Stellamod.Core.ProjectileHelpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class VolcanicSpearHead : ModProjectile
{
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

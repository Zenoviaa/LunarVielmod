using Stellamod.Core.ProjectileHelpers;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

//Like fenix's think but fiery and really cool
//Could probably make this shader with similar methods to gothivia?
//or just use the same shader tbh
//I'll look at how it works
public class FlameWheel : ModProjectile
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

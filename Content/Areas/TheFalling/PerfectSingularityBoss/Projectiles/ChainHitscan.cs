using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;

public class ChainHitscan : ModProjectile
{
    private enum ChainStyle
    {
        ChainWhip,
        ChainJail,
        ChainLinger
    }
    private Vector2 _originalPoint;
    private ref float Timer => ref Projectile.ai[0];
    private ChainStyle Style
    {
        get => (ChainStyle)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 128;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _originalPoint = Projectile.Center;
        }

    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (this.OwnedByLocalClient())
        {
            switch (Style)
            {
                case ChainStyle.ChainWhip:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), _originalPoint, (Projectile.Center - _originalPoint), 
                        ModContent.ProjectileType<PerfectChain>(), Projectile.damage, 1, Projectile.owner, 0, (float)Style);
                    break;
            }

        }
        return base.OnTileCollide(oldVelocity);
    }
    public override void OnKill(int timeLeft)
    {


    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
}

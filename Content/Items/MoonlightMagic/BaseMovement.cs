using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public abstract class BaseMovement : IAdvancedMagicAddon
    {
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;
        public abstract void AI();
    }
}

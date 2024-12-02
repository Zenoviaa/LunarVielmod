using Terraria;

namespace Stellamod.Items.MoonlightMagic
{
    internal interface IAdvancedMagicAddon
    {
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;
    }
}

using Terraria;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public interface IAdvancedMagicAddon
    {
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;
    }
}

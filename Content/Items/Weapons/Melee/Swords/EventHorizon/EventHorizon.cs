using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Melee.Swords.EventHorizon
{
    internal class EventHorizon : BaseSwingItem
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EventHorizonSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<EventHorizonStaminaSlash>();
        }
    }
}

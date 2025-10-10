using Stellamod.Core.XixianFlaskSystem;
using Terraria;

namespace Stellamod.Items.Insources
{
    public class HealthInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 30;
        }

        public void UseInsource(Player player)
        {
            player.Heal(50);
        }
    }
}

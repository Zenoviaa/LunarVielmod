using Stellamod.Common.XixianFlaskSystem;
using Terraria;

namespace Stellamod.Items.Insources
{
    public class HealthInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 30;
        }


        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.Heal(50);
        }
    }
}

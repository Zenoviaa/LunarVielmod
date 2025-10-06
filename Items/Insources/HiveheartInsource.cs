using Stellamod.Core.XixianFlaskSystem;
using Terraria.ID;

namespace Stellamod.Items.Insources
{
    public class HiveheartInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 15;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.AddBuff(BuffID.Honey, 60 * 10);
        }
    }
}

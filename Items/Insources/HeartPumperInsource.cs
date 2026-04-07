using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;

namespace Stellamod.Items.Insources
{
    public class HeartPumperInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 120;
        }


        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.Heal(150);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankBrooch>();
        }
    }
}

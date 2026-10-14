using Stellamod.Common.StaminaSystem;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;

namespace Stellamod.Items.Insources;

public class StaminaInsource : InsourceItem
{
    public override int GetAddedTime()
    {
        return 60 * 10;
    }
    public override void UseInsource(FlaskPlayer flaskPlayer)
    {
        base.UseInsource(flaskPlayer);
        flaskPlayer.Player.GetModPlayer<StaminaNDashPlayer>().DashCount++;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Mushroom, BlankBrooch>();
    }
}

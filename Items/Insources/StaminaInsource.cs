using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items.Accessories.Players;

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
        flaskPlayer.Player.GetModPlayer<DashPlayer>().DashCount++;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Mushroom, BlankBrooch>();
    }
}

using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.AccTF;

public class LuckyCardWinner : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<IgniterPlayer>().lucky = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GhastlySpirit, BlankAccessory>();
    }
}

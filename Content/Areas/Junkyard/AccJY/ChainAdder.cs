using Stellamod.Content.CommonMaterials;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.AccJY;

public class ChainAdder : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MeleeEffectsPlayer>().safunaiChainBonus++;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MechanizedSoul, BlankAccessory>();
    }
}

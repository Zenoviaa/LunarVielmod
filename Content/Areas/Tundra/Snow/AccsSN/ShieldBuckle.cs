using Stellamod.Common.StaminaSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN;

public class ShieldBuckle : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ice.Items.ShieldBuckle.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<StaminaNDashPlayer>().hasShieldBuckle = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankAccessory>();
    }
}

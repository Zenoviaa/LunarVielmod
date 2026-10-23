using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;

public class NPCSpawner : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.DebugWand.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.White;
        Item.useTime = 4;
        Item.useAnimation = 4;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = false;
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            if (player.altFunctionUse == 2)
            {
                ModContent.GetInstance<DevToolsUISystem>().ToggleNPCMenuUI();
            }
            else
            {
                NPCUtilities.SpawnNPCFromClient(Main.MouseWorld, DevTools.spawnNPCType);
            }
        
        }

        return true;
    }
}

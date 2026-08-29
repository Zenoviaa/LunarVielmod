using Stellamod.Content.CommonMaterials;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;

public class RollingStonesInfiniteEquip : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Item.IsTheSameAs += NoStonesAreTheSame;
    }

    private bool NoStonesAreTheSame(On_Item.orig_IsTheSameAs orig, Item self, Item compareItem)
    {
        if (self.type == ModContent.ItemType<RollingStones>() || compareItem.type == ModContent.ItemType<RollingStones>())
            return false;
        return orig(self, compareItem);
    }
}

public class RollingStones : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        LevelingPlayer levelingPlayer = player.GetModPlayer<LevelingPlayer>();
        for (int i = 0; i < levelingPlayer.stats.Length; i++)
        {
            levelingPlayer.statModifiers[i] += 2;
        }
    }
}

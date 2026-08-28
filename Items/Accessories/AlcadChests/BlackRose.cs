using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.AlcadChests;

public class BlackRosePlayer : ModPlayer
{
    public bool hasBlackRose;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasBlackRose = false;
    }
    public override void OnMissingMana(Item item, int neededMana)
    {
        base.OnMissingMana(item, neededMana);
        if (!hasBlackRose)
            return;

        Player.statMana += neededMana;
        Player.statLife -= neededMana;

        int combatText = CombatText.NewText(Player.getRect(), Color.Red, $"-{neededMana}", true);
        CombatText numText = Main.combatText[combatText];
        numText.lifeTime = 60;
        if (Player.statLife <= 0)
            Player.KillMe(new Terraria.DataStructures.PlayerDeathReason(), 0, 1);
    }
}
public class BlackRose : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 36;
        Item.rare = ItemRarityID.Green;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BlackRosePlayer>().hasBlackRose = true;
    }
}

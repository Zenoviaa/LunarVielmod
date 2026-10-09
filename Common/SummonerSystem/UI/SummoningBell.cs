using Stellamod.Common.RarityRendering;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem.UI;

public class SummoningBell : ModItem
{
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var bellPlayer = Main.LocalPlayer.GetModPlayer<BellPlayer>();
        if (!bellPlayer.HasUnlockedBell())
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "belllocked", LangText.Common("BellLock"))
            {
                OverrideColor = Color.Goldenrod
            };
            tooltips.Add(line);
        }
        else
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "belltip", LangText.Common("BellUse", LunarVeilKeybinds.BellKeybind.GetAssignedKeys()[0]))
            {
                OverrideColor = Color.LightGreen
            };
            tooltips.Add(line);

            float ticks = bellPlayer.GetCastingTime();
            float seconds = ticks / 60;
            string secondsString = seconds.ToString("#.#");
            line = new TooltipLine(Mod, "belltip", LangText.Common("TotalCastingTime", secondsString))
            {
                OverrideColor = Color.LightGreen
            };
            tooltips.Add(line);
        }
    }

    public override void UpdateInventory(Player player)
    {
        base.UpdateInventory(player);
        player.GetModPlayer<BellPlayer>().UnlockFlask();
        for (int i = 0; i < player.inventory.Length; i++)
        {
            Item item = player.inventory[i];
            if (item == Item)
            {
                player.inventory[i] = new Item();
                player.inventory[i].SetDefaults(ItemID.None);
            }
        }
    }

    public override bool OnPickup(Player player)
    {
        player.GetModPlayer<BellPlayer>().UnlockFlask();
        PopupText.NewText(PopupTextContext.SonarAlert, Item, 1, longText: true);
        return false;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.SpecialRarity[Type] = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = 1;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Player player = Main.player[Main.myPlayer];
        if (player.TryGetModPlayer<BellPlayer>(out var bellPlayer))
        {
            //Check that you have advanced brooches since these don't work without
            if (bellPlayer.CanSummon())
            {
                //Give backglow to show that the effect is active
                DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 200, 124));
            }
            else
            {
                float sizeLimit = 28;
                //Draw the item icon but gray and transparent to show that the effect is not active
                Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                return false;
            }
        }

        return true;
    }
}

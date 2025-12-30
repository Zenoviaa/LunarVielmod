using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.XixianFlaskSystem
{
    public abstract class InsourceItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            InsourceHelper.AddCooldownLine(Mod, tooltips, GetAddedTime());
        }
        public virtual int GetAddedTime()
        {
            return 60 * 15;
        }
        public virtual void PreUseInsource(FlaskPlayer flaskPlayer)
        {
            flaskPlayer.insourceTime += GetAddedTime();
        }
        public virtual void UseInsource(FlaskPlayer flaskPlayer) { }
        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
            player.GetModPlayer<FlaskPlayer>().UnlockInsource(Item);
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item == Item)
                {
                    player.inventory[i] = new Item();
                    player.inventory[i].SetDefaults(0);
                    PopupText.NewText(PopupTextContext.SonarAlert, Item, 1, longText: true);
                    break;
                }
            }
        }

        public override bool OnPickup(Player player)
        {
            player.GetModPlayer<FlaskPlayer>().UnlockInsource(Item);
            PopupText.NewText(PopupTextContext.SonarAlert, Item, 1, longText: true);
            return false;
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.UI.XixianFlaskSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public abstract class BaseInsource : ModItem
    {
        public int InsourceHealValue;
        public int InsourceCannotUseDuration;
        public int InsourcePotionSickness;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Insource", Helpers.LangText.Common("Insource"))
            {
                OverrideColor = new Color(100, 278, 203)
            };
            tooltips.Add(line);
        }

        public virtual void TriggerEffect(Player player) { }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 3, 3, 40);
            Item.rare = ItemRarityID.Green;
        }
    }

    public class XixianFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "xixian", "Right click in your inventory to put an insource in the flask, then drink it! It acts like an infinite potion!")
            {
                OverrideColor = new Color(308, 71, 255)
            };
            tooltips.Add(line);
        }
     

        public override void SetDefaults()
        {
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.value = Item.buyPrice(0, 3, 3, 40);
            Item.rare = ItemRarityID.Green;
            Item.consumable = false;
            Item.potion = true;
            Item.UseSound = SoundID.Item2;
            Item.healLife = 10;
          
        
        }

        public override void RightClick(Player player)
        {
            base.RightClick(player);
            XixianFlaskUISystem uiSystem = ModContent.GetInstance<XixianFlaskUISystem>();
            uiSystem.ToggleUI();
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

       
        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            base.GetHealLife(player, quickHeal, ref healValue);
            FlaskPlayer flaskPlayer = player.GetModPlayer<FlaskPlayer>();
            if (flaskPlayer.Insource.ModItem is BaseInsource insource)
            {
                healValue = insource.InsourceHealValue;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.player[Main.myPlayer];

            //Check that this item is equipped

            //Check that you have advanced brooches since these don't work without
            if (!player.HasBuff<CannotUseFlask>())
            {
                //Give backglow to show that the effect is active
                DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 200, 124));
            }

            if (player.HasBuff<CannotUseFlask>() || player.HasBuff(BuffID.PotionSickness))
            {
                float sizeLimit = 28;
                //Draw the item icon but gray and transparent to show that the effect is not active
                Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                return false;
            }


            return true;
        }

        public override bool CanUseItem(Player player)
        {
            FlaskPlayer flaskPlayer = player.GetModPlayer<FlaskPlayer>();
            if (flaskPlayer.Insource.ModItem is BaseInsource insource)
            {
               
            }
                
            if (player.HasBuff<CannotUseFlask>() || player.HasBuff(BuffID.PotionSickness))
            {
                return false;
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            FlaskPlayer flaskPlayer = player.GetModPlayer<FlaskPlayer>();
            if (flaskPlayer.Insource.ModItem is BaseInsource insource)
            {
                int healValue = insource.InsourceHealValue;
                player.statLife += healValue;
                insource.TriggerEffect(player);
                player.ClearBuff(BuffID.PotionSickness);
                player.AddBuff(BuffID.PotionSickness, insource.InsourcePotionSickness);
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), insource.InsourceCannotUseDuration);
            }
            return base.UseItem(player);
        }
    }
}
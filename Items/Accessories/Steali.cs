using Stellamod.Common.StaminaSystem;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
    public class Steali : AbstractDashItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
            dashPlayer.DashVelocity += 7;
            dashPlayer.DashRegenerationBonus += 0.05f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankAccessory>();
        }
    }
}
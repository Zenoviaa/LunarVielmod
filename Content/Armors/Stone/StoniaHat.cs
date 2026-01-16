using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Stone
{
    [AutoloadEquip(EquipType.Head)]
    public class StoniaHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<StoniaHat, StoniaChestplate, StoniaBoots>();
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.criticalStrikeDamage += 0.5f;
            armorStatsPlayer.defenseBonus += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StoniaChestplate>() && legs.type == ModContent.ItemType<StoniaBoots>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.noFallDmg = true;
            player.pickSpeed -= 0.25f;
            player.maxFallSpeed *= 3f;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class StoniaChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.defenseBonus += 3;
            armorStatsPlayer.generalEndurance += 0.05f;
            armorStatsPlayer.accessorySlots += 1;
        }
    } 
  
    [AutoloadEquip(EquipType.Legs)]
    public class StoniaBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.criticalStrikeChance += 0.05f;
            armorStatsPlayer.defenseBonus += 1;
        }
    }
}
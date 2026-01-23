using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Leather
{
    public class LeatherPlayer : ModPlayer
    {
        public bool hasLeatherSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasLeatherSetBonus= false;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (hasLeatherSetBonus)
            {
                CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
                crossbowPlayer.magicCircleColor = Color.White;
                crossbowPlayer.magicCircleTextureAsset = AssetManager.GlowMask.MagicCircle2;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class LeatherHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<LeatherHead, LeatherBody, LeatherLegs>();
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.criticalStrikeDamage += 0.25f;
            stats.defenseBonus += 2;
            stats.accessorySlots += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LeatherBody>() && legs.type == ModContent.ItemType<LeatherLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<LeatherPlayer>().hasLeatherSetBonus = true;
        
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class LeatherBody : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
        }


        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 3;
            stats.rangedDamage += 0.05f;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LeatherLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.rangedBowChargeTime += 0.3f;
            stats.defenseBonus += 1;
            stats.accessorySlots += 1;
        }
    }
}

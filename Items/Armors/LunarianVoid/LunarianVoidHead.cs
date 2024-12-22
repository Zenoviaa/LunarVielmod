using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.LunarianVoid
{
    [AutoloadEquip(EquipType.Head)]
    public class LunarianVoidHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Astrasilk Hat");
            // Tooltip.SetDefault("Increases Mana Regen by 4%");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {

            player.GetCritChance(DamageClass.Throwing) += 10f;
            player.GetDamage(DamageClass.Throwing) *= 1.25f;
            player.statLifeMax2 += 20;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LunarianVoidBody>() && legs.type == ModContent.ItemType<LunarianVoidLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().Leather = true;
        }
    }
}

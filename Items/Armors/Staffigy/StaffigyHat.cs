using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Armors.Scrappy;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Staffigy
{
   

    [AutoloadEquip(EquipType.Head)]
    public class StaffigyHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StaffigyRobe>()
                && legs.type == ModContent.ItemType<StaffigyPants>();
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Become greatly empowered for a short time when low on health!\nJust one last breath...");
            player.statManaMax2 += 20;


        }


    }
}

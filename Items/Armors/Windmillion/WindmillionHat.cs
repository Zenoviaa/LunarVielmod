using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;
using Stellamod.Helpers;

namespace Stellamod.Items.Armors.Windmillion
{
    [AutoloadEquip(EquipType.Head)]
    public class WindmillionHat : ModItem
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
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {

            player.GetCritChance(DamageClass.Throwing) += 10f;
            player.GetDamage(DamageClass.Throwing) *= 1.1f;
            player.statLifeMax2 += 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WindmillionRobe>() && legs.type == ModContent.ItemType<WindmillionBoots>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {//30% Increased throwing attack speed!
            //Highly increased throwing weapon knowckback
            player.setBonus = LangText.SetBonus(this);//"I am wind in a million" + "\n30% Increased throwing attack speed!" + "\nHighly increased throwing weapon knowckback");
            player.GetAttackSpeed(DamageClass.Throwing) += 0.3f;
            player.GetKnockback(DamageClass.Throwing) += 0.3f;
        }

       
    }
}

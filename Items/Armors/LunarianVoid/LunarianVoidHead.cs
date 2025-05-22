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

        public override void AddRecipes() 
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}

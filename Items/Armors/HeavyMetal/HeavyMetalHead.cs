using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.HeavyMetal
{
    [AutoloadEquip(EquipType.Head)]
    public class HeavyMetalHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Heavy Metal Hat");
			// Tooltip.SetDefault("Increases throwing critical strike chance by 4%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

    

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.maxMinions += 1;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().HMArmor = true;
      
            player.setBonus = LangText.SetBonus(this);//"2 Gintze Guards come to fight for you"  + "\n+1 Summons!");  // This is the setbonus tooltip
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}

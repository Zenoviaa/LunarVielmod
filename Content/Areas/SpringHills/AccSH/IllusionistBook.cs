using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class IllusionistBook : ModItem
    {
        public override string Texture => AssetReferences.Assets.Textures.Item.IllusionistBook.KEY;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AdvancedMagicPlayer>().chargeTimeBonus += 0.1f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Ivythorn>());
        }
    }
}
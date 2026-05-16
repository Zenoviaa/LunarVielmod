using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.AccFB
{
    public class LeatherGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }
}

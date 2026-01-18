using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class BasicBaseball : BaseJugglerItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToCombatTool(0.01f, 0.04f, 1);
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BasicBaseballProj>();
            Item.shootSpeed = 24;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(), material: ModContent.ItemType<Mushroom>());
        }
    }
    public class BasicBaseballProj : BaseJugglerProjectile
    {

    }
}

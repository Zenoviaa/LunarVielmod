using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL
{
    public class HuntingSwingHookLine : GrappleLine
    {

    }

    public class HuntingSwingHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<HuntingSwingHookLine>();
            Item.shootSpeed = 15;
            Item.autoReuse = false;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(
                mold: ModContent.ItemType<BlankGun>(),
                material: ModContent.ItemType<GintzlMetal>());
        }
    }
}

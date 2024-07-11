using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class VacuumDrill : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 30;
            Item.mana = 8;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = SoundID.DD2_LightningAuraZap;

            // Weapon Properties
            Item.value = Item.sellPrice(gold: 2);
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 19;
            Item.knockBack = 0;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<VacuumDrillHold>();
            Item.shootSpeed = 15f;
            Item.channel = true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>());
            recipe.AddIngredient(ModContent.ItemType<BrokenTech>(), 5);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
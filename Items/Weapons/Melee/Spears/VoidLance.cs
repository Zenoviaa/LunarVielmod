using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Weapons.Spears;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;

namespace Stellamod.Items.Weapons.Melee.Spears
{
    internal class VoidLance : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<VoidLanceSpear>();
            Item.shootSpeed = 35f;
  
            Item.useAnimation = 20;
            Item.useTime = 28;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<VoidLanceWave>(), damage, knockback, player.whoAmI);
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TheIrradiaspear>(), 1);
            recipe.AddIngredient(ItemType<SingulariumBar>(), 16);
            recipe.AddIngredient(ItemID.DarkLance, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}

using Stellamod.Projectiles.Weapons.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Weapons.Spears;
using Stellamod.Projectiles.Weapons.Magic;
using Stellamod.NPCs.Bosses.DreadMire;

namespace Stellamod.Items.Weapons.Mage
{
    internal class VoidsGrasp : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Void's Grasp");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 25;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<VoidHandSpawn>();
            Item.shootSpeed = 10f;
            Item.mana = 15;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ShadeHandTome>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }


    }
}

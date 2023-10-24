using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Projectiles.Spears;
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
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Bow;
using Terraria.Audio;
using Terraria.DataStructures;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Swords;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class M61X : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("M.3.8-F30");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 10;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<ArchariliteHeatBlast>();
            Item.shootSpeed = 40f;
            Item.mana = 4;
            Item.UseSound = SoundID.Item72;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FlintlockPistol, 1);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }




    }
}

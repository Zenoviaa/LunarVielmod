using Microsoft.Xna.Framework;
using Mono.Cecil;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.damage = 15;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<ArchariliteHeatBlast>();
            Item.shootSpeed = 40f;
            Item.mana = 4;
            Item.UseSound = SoundID.Item72;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().ArchariliteSC)
            {
                Item.useTime = 5;
                Item.damage = 21;
                Item.knockBack = 4;
                Item.useAnimation = 5;
                Item.mana = 7;
                Item.shootSpeed = 30f;
                type = ModContent.ProjectileType<ArchariliteHeatBlastSC>();
            }
            else
            {
                Item.damage = 15;
                Item.knockBack = 14;
                Item.useAnimation = 10;
                Item.useTime = 10;
                Item.shootSpeed = 40f;
                type = ModContent.ProjectileType<ArchariliteHeatBlast>();
            }

        }


    }
}

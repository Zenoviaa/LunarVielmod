using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Nails;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Shields
{
    public class SteamedNail : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 1;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Doorlauncher"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Burning with flowery rage!");
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 1f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }
        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 100;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 10000;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SteamedNailProj>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
            Item.crit = 52;
            Item.accessory = true;
            Item.defense = 8;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            combowombo++;
            int dir = AttackCounter;
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);
            if (combowombo == 3 || combowombo == 4)
            {
                Item.knockBack = 9;
            }
            else
            {
                Item.knockBack = 4;
            }
            if (combowombo == 4)
            {
                Item.shoot = ModContent.ProjectileType<SteamedNailProj>();
                combowombo = 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<SteamedNailProj2>();
            }
            return false;

        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ItemID.Bone, 22);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 5);
            recipe.AddIngredient(ItemID.HallowedBar, 5);


            recipe.Register();
        }
    }
}
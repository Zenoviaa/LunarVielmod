using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Ranged
{
    internal class Crysalizer : ClassSwapItem
    {
        public int WinterboundArrow;

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 121;
            Item.mana = 8;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightRed;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 26f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
            recipe.AddIngredient(ModContent.ItemType<IceWalker>(), 1);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            WinterboundArrow += 1;
            if (WinterboundArrow >= 3)
            {
                WinterboundArrow = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow1>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            if (WinterboundArrow == 2)
            {

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow2>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            if (WinterboundArrow == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow3>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }


            return false;
        }
    }
}

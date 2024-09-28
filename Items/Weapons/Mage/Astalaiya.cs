using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Astalaiya : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 9;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public int Star;
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
     
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<AstalaiyaStar1>();
            Item.shootSpeed = 10f;
            Item.mana = 5;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 14);

            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ItemID.ManaCrystal, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Star += 1;
            if (Star >= 3)
            {
                Star = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar1>();
            }
            if (Star == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya2"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar3>();
            }
            if (Star == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar2>();
            }
        }



    }
}

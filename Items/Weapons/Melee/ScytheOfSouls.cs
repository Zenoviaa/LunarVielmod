using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Melee
{
    class ScytheOfSouls : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Magic;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 23;
            Item.useAnimation = 20;
            Item.useTime = 20;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinder Braker");
        }

        public override void SetDefaults()
        {
            Item.damage = 37;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 3, 20, 14);
            Item.rare = ItemRarityID.LightRed;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<ScytheOfSoulsProj>();
            Item.shootSpeed = 8f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddIngredient(ItemType<EldritchSoul>(), 8);
            recipe.AddIngredient(ItemID.Sickle, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
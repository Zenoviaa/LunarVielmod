using Stellamod.Items.Materials;
using Stellamod.Projectiles.Ammo;
using Stellamod.Projectiles.Arrows;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Ammo
{
    internal class LiliumArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 12; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<FlowerArrow>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 16f; // The speed of the projectile.
            Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
      

    }
}

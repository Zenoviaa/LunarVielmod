using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Irrasprayer : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Ranged;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;

            // Weapon Properties
            Item.DamageType = DamageClass.Magic;
            Item.damage = 40;
            Item.knockBack = 4;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<IrrasprayProj>();
            Item.shootSpeed = 6f;
            // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
            Item.value = Item.sellPrice(gold: 25);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<VirulentPlating>());
        }
    }
}

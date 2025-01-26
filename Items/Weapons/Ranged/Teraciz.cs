using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Teraciz : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Summon;
        public override void SetClassSwappedDefaults()
        {
            Item.mana = 8;
            Item.damage = 10;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 19;
            Item.knockBack = 0;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<MeatBullet>();
            Item.shootSpeed = 15f;
            Item.value = 5000;
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<MeatBullet3>(), ModContent.ProjectileType<MeatBullet2>() });
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<MiracleThread>());
        }
    }
}
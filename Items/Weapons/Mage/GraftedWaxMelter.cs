
using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
    public class GraftedWaxMelter : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Ranged;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 0;
            Item.damage = 42;
        }

        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Spinny Winny damage the binny");

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Max", "(A) Honestly could wipe out many wide varieties of enemies!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);





            base.ModifyTooltips(tooltips);



        }

        public override void SetDefaults()
        {

            Item.damage = 39; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.width = 20; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 18; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 18; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 3; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Orange; // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item42; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<CandleShotProj1>();
            Item.shootSpeed = 18f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.mana = 8;

            Item.autoReuse = true;


        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 100);
            recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 20);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 10);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 10);

            recipe.Register();
        }
    }
}
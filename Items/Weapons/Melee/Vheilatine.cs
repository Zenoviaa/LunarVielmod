
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Slashers.Vheilatine;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class Vheilatine : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Magic;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 13;
            Item.mana = 22;
        }
        public int AttackCounter = 1;
        public int combowombo = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Auroran"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Sends shockwaves through the air" +
                "\nHitting enemies with sword will increase speed!" +
                "\nDivergency Inspired!"); */
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "Vaheil", "(S) Great Damage scaling with swings!")
            {
                OverrideColor = new Color(108, 201, 255)

            };
            tooltips.Add(line);

            base.ModifyTooltips(tooltips);

        }
        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VheilatineProj>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<VheilatineProj>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 8)
            {
                type = ModContent.ProjectileType<VheilatineProj>();
                SoundEngine.PlaySound(SoundID.Item34, player.position);
            }
        }

         public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        
            int dir = AttackCounter;
            if (player.direction == 1)
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
            }
            else
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

            }
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 10);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 10);
            recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 12);
            recipe.AddIngredient(ItemID.FallenStar, 9);
            recipe.Register();
        }
    }
}
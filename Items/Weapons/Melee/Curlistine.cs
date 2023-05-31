
using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class Curlistine : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Spinny Winny damage the binny");

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Curlistine", "(A) Very High Damage Scaling with frost explosions!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);









        }

        public override void SetDefaults()
        {

            Item.damage = 19; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Melee;
            Item.width = 20; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 30; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 11; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Blue; // the color that the Item's name will be in-game
            Item.UseSound = SoundID.Item101; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<CurlistineProj>();
            Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
         
            Item.autoReuse = true;


        }
       
    }
}
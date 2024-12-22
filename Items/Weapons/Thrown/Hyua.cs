
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Slashers.Hyua;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class Hyua : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 38;
            Item.mana = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "Daeknasd", "(B) Very good throwing weapon that sticks around!")
            {
                OverrideColor = new Color(108, 201, 255)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 0;
            Item.height = 0;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 700;
            Item.noMelee = true;
            Item.shootSpeed = 2;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RingedAlcd>();
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
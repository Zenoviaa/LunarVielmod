
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.Hyua;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class Hyua : ModItem
    {
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
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 700;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HyuaProj>();
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.buyPrice(0, 0, 7, 0);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<HyuaProj>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 8)
            {
                type = ModContent.ProjectileType<HyuaProj>();
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
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RingedAlcd>(), damage * 1, knockback, player.whoAmI, 1, dir);
            return false;
        }

      
    }
}
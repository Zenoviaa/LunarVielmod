﻿
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Slashers;
using Stellamod.Projectiles.Slashers.ThefirstAurora;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using Stellamod.Projectiles.Safunai.Alcarish;
using System.Collections.Generic;
using System.IO;


namespace Stellamod.Items.Weapons.Melee.Greatswords
{
    public class TheFirstAurora : ModItem
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

            line = new TooltipLine(Mod, "Alcarishasd", "Greatsword Weapon Type")
            {
                OverrideColor = ColorFunctions.GreatswordWeaponType
            };
            tooltips.Add(line);




        }
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuroraSProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<AuroraSProj>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 4)
            {
                type = ModContent.ProjectileType<AuroraSProj>();
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

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Swinger") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);
           // Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AuroranBullet>(), damage * 2, knockback, player.whoAmI, 1, dir);
          //  Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<AuroranBullet2>(), damage * 2, knockback, player.whoAmI, 1, dir);
           // Projectile.NewProjectile(source, position, velocity * 1.2f, ModContent.ProjectileType<AuroranBullet3>(), damage * 2, knockback, player.whoAmI, 1, dir);
            return false;
        }

        
    }
}
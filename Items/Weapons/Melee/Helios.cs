
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Slashers.Helios;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class Helios : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gutinier"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Classful weapon!" +
                "\nDivergency Inspired!"); */
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "Helios", "(S+) Godlike High Damage Scaling with the power of the sun!")
            {
                OverrideColor = new Color(255, 223, 82)

            };
            tooltips.Add(line);



        }
        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Generic;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StarSheith");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<HeliosProj>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ModContent.RarityType<GothiviaSpecialRarity>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<HeliosProj>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 1)
            {
                type = ModContent.ProjectileType<HeliosProj>();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb"), player.position);
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
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LightSpandProg>(), damage * 1, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity * 0.5f, ModContent.ProjectileType<LightSpandProg>(), damage * 1, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity * 1.2f, ModContent.ProjectileType<LightSpandProg>(), damage * 2, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity * 0.7f, ModContent.ProjectileType<LightSpandProg>(), damage * 3, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity * 0.2f, ModContent.ProjectileType<LightSpandProg>(), damage * 2, knockback, player.whoAmI, 1, dir);
            Projectile.NewProjectile(source, position, velocity * 1f, ModContent.ProjectileType<HeliosP>(), damage * 1, knockback, player.whoAmI, 1, dir);

            return false;
        }

       
    }
}
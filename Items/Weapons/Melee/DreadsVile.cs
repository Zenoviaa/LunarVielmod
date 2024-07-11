using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Nails;
using Stellamod.Projectiles.Steins;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class DreadsVile : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 1;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Stein"))
            {
                OverrideColor = ColorFunctions.SteinWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Doorlauncher"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Electrical nail, thats weird");
        }
        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Generic;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 100;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.noMelee = false;
            Item.rare = ModContent.RarityType<GothiviaSpecialRarity>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VileFist>();
            Item.shootSpeed = 20f;
            Item.crit = 22;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 1f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            combowombo++;
            int dir = AttackCounter;
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);

            if (combowombo == 3 || combowombo == 4)
            {
                Item.knockBack = 9;
            }
            else
            {
                Item.knockBack = 4;
            }

            if (combowombo == 4)
            {
                Item.shoot = ModContent.ProjectileType<VileFist>();
                combowombo = 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<VileFist>();
            }
            return false;
        }





    }
}
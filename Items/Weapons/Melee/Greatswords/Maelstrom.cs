
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

using Stellamod.Projectiles.Slashers.Maelstrom;

namespace Stellamod.Items.Weapons.Melee.Greatswords
{
    public class Maelstrom : ModItem
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

            line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Greatsword"))
            {
                OverrideColor = ColorFunctions.GreatswordWeaponType
            };
            tooltips.Add(line);




        }
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MaelstromProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<MaelstromProj>();
                damage = Item.damage;
            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 4)
            {
                type = ModContent.ProjectileType<MaelstromProj2>();
                damage = 60;
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
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);

            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 4)
            {
                float numberProjectiles = 5;
                float rotation = MathHelper.ToRadians(20);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<MaelstromShotProj>(), damage, Item.knockBack, player.whoAmI);
                }

                player.GetModPlayer<MyPlayer>().SwordCombo = 0;
                player.GetModPlayer<MyPlayer>().SwordComboR = 0;
            } else
            {
                player.GetModPlayer<MyPlayer>().SwordCombo++;
                player.GetModPlayer<MyPlayer>().SwordComboR = 240;
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwingyMael") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
            return false;
        }


    }
}
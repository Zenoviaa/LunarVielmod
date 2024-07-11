
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.DelgrimsHammer;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Greatswords
{
    public class DelgrimsHammer : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 0;
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
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 20;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DelgrimsHammerProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 27, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<DelgrimsHammerProj>();

            }
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 4)
            {
                type = ModContent.ProjectileType<DelgrimsHammerProj>();
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
            
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwingyAr") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);
            return false;
        }
    }
}
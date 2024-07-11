using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.DelgrimsHammer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Slashers.Maelstrom;
using Stellamod.Projectiles.Slashers.Chillrend;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Melee.Greatswords
{
    internal class Chillrend : ModItem
    {
        private int _combo;
        public int AttackCounter = 1;
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
            Item.damage = 152;
            Item.DamageType = DamageClass.Melee;
            Item.width = 90;
            Item.height = 108;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 20;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ChillrendProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 27, 50, 0);
            Item.rare = ItemRarityID.Lime;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            _combo++;
            if (_combo >= 0)
            {
                type = ModContent.ProjectileType<ChillrendProj>();

            }
            if (_combo >= 6)
            {
                type = ModContent.ProjectileType<ChillrendProj2>();
                SoundEngine.PlaySound(SoundID.Item34, player.position);
                _combo = 0;
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



            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwingyWind") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);
            return false;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 30);
            recipe.AddIngredient(ItemID.Ectoplasm, 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class VialedInsource : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Insource",  Helpers.LangText.Common("Insource"))
            {
                OverrideColor = new Color(100, 278, 203)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {

            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.value = Item.buyPrice(0, 3, 3, 40);
            Item.rare = ItemRarityID.Green;
            Item.consumable = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");

        }


        public override bool? UseItem(Player player)
        {
            FlaskPlayer FlaskPlayer = player.GetModPlayer<FlaskPlayer>();
            if (FlaskPlayer.hasVialedInsource)
            {
                FlaskPlayer.hasVialedInsource = false;

                return true;
            }
            if (!FlaskPlayer.hasVialedInsource)
            {
                FlaskPlayer.hasVialedInsource = true;
                FlaskPlayer.hasVitalityInsource = false;
                FlaskPlayer.hasFloweredInsource = false;
                FlaskPlayer.hasHealthyInsource = false;
                FlaskPlayer.hasEpsidonInsource = false;

                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
                ModContent.ProjectileType<VialedInsourceProj>(), 0, 1f, player.whoAmI);


                return true;
            }

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 10);
            recipe.AddIngredient(ItemID.Daybloom, 5);
            recipe.AddIngredient(ItemID.Moonglow, 5);
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }

    }
}
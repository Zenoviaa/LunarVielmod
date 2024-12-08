using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseJugglerItem : ClassSwapItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.damage = 19;
            Item.DamageType = DamageClass.Throwing;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd", Helpers.LangText.Common("Juggler"))
            {
                OverrideColor = ColorFunctions.JugglerWeaponType
            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Alcarishasd", Helpers.LangText.Common("JugglerHelp"))
            {
                OverrideColor = Color.LightGray
            };
            tooltips.Add(line);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                //Gonna make it kill all balls but idk how
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}

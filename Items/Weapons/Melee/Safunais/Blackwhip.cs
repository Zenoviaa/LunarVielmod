using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Projectiles.Safunai.Blackwhip;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunais
{
    public class Blackwhip : BaseSafunaiItem
    {
        public override DamageClass AlternateClass => DamageClass.Generic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 80;
            Item.mana = 0;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Haldada", Helpers.LangText.Common("Safunai"))
            {
                OverrideColor = new Color(308, 71, 99)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Halhadda", "(B) Good Damage Scaling (Alcarish Blobs) On Hit!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<DarkwhipProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 160;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}

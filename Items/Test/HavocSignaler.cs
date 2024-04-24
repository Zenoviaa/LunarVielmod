using Stellamod.Projectiles.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;

namespace Stellamod.Items.Test
{
    internal class HavocSignaler : ModItem
    {
        private int _useIndex;
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var rightClickLine = new TooltipLine(Mod, "r", "Use this item to signal Havoc for an attack\nRight click to swap attacks");
            tooltips.Add(rightClickLine);
            TooltipLine line;
            switch (_useIndex)
            {
                default:
                case 0:
                    line = new TooltipLine(Mod, "Alcarishxxa", "Attack: Charge")
                    {
                        OverrideColor = new Color(244, 119, 255)
                    };
                    break;
                case 1:
                    line = new TooltipLine(Mod, "Alcarishxxa", "Attack: Mini Laser")
                    {
                        OverrideColor = new Color(244, 119, 255)
                    };
                    break;
                case 2:
                    line = new TooltipLine(Mod, "Alcarishxxa", "Attack: Big Laser")
                    {
                        OverrideColor = new Color(244, 119, 255)
                    };
                    break;
            }

            tooltips.Add(line);
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if(player.altFunctionUse == 2)
            {
                _useIndex++;
                if (_useIndex >= 3)
                {
                    _useIndex = 0;
                }
                return true;
            }
            else
            {
                switch (_useIndex)
                {
                    case 0:
                        HavocSignal.NewSignal(Havoc.ActionState.Charge);
                        break;
                    case 1:
                        HavocSignal.NewSignal(Havoc.ActionState.Laser);
                        break;
                    case 2:
                        HavocSignal.NewSignal(Havoc.ActionState.Laser_Big);
                        break;
                }
                return true;
            }
        }
    }
}

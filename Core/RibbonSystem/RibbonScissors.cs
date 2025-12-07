using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.RibbonSystem
{
    public class RibbonScissors : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item42;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int mouseX = (int)(Main.MouseWorld.X / 16);
                int mouseY = (int)(Main.MouseWorld.Y / 16);

                //Just some position clamping so it's not connecting floating points and it looks a bit better
                mouseX *= 16;
                mouseY *= 16;
                RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
                ribbonRenderer.TryBreakRibbon(new Vector2(mouseX, mouseY));
            }


            return true;
        }
    }
}

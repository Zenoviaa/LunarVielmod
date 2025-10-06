using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.XixianFlaskSystem
{
    public class InsourceUnlocker : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Point16 point = new Point16(x, y);
            if (player.altFunctionUse == 2)
            {
                //Right click 
                //Cycle
                player.GetModPlayer<FlaskPlayer>().ResetProgress();
            }
            else
            {
                player.GetModPlayer<FlaskPlayer>().GrantAllProgress();

            }
            return true;
        }
    }
}

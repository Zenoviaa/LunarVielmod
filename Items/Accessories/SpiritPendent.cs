
using Microsoft.Xna.Framework;
using Stellamod.Common.Lights;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SpiritPendent : ModItem
    {
        private float _timer;
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            _timer++;
            SpecialEffectsPlayer specialEffectsPlayer = player.GetModPlayer<SpecialEffectsPlayer>();
            specialEffectsPlayer.hasSpiritPendant = true;
            if (_timer % 32 == 0 && !hideVisual)
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.FireworkFountain_Blue, Scale: 0.5f);
            }

            Lighting.AddLight(player.position, Color.LightSkyBlue.ToVector3() * 2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<ConvulgingMater>());
        }
    }
}

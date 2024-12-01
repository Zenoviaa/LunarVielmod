using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class CrystalStar : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Expert;
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
            soundStyle.PitchVariance = 0.15f;
            Item.UseSound = soundStyle;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<CauldronPlayer>().CrystalStarCount++;
            for (int i = 0; i < 32; i++)
            {
                float progress = (float)i / 32f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 velocity = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(player.Center, DustID.BoneTorch, velocity);
            }
            return true;
        }
    }
}

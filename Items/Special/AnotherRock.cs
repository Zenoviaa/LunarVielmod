using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special
{
    internal class AnotherRock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;

        }

        public override bool? UseItem(Player player)
        {
            Vector2 teleportPosition = TeleportSystem.StoneGolemAltarWorld;
            player.Teleport(teleportPosition);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
            SoundEngine.PlaySound(SoundID.Item6, player.position);
            return true;
        }
    }
}

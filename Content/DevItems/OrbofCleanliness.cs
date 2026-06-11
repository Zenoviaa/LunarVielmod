using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.DevItems;

public class OrbofCleanliness : ModItem
{
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

    public override bool? UseItem(Player player)
    {
        if (MultiplayerHelper.IsHost)
        {
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            colosseumSystem.Reset();
        }
        else
        {
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ResetColosseum).Send(-1);
        }
        player.GetModPlayer<LevelingPlayer>().ResetStats();

        DownedBossSystem.ResetFlags();
        DownedBossTracker.ResetFlags();
        DownedBossRewardPlayer rewardPlayer = player.GetModPlayer<DownedBossRewardPlayer>();
        rewardPlayer.ResetFlags();

        return true;
    }
}

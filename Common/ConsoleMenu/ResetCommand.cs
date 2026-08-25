using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.PlayerLevelingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ConsoleMenu;

public class ResetCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "reset";
    }

    public override Arguments GetArguments()
    {
        Arguments arguments0 = new Arguments();
        arguments0.potentialArguments = new()
        {
            "level",
            "boss",
            "gintze"
        };

        return arguments0;
    }

    public override bool Invoke(params string[] args)
    {
        if (args.Length <= 0)
            return false;
        Player player = Main.LocalPlayer;
        switch (args[1])
        {
            case "level":

                player.GetModPlayer<LevelingPlayer>().ResetStats();
                return true;
            case "boss":
                DownedBossSystem.ResetFlags();
                DownedBossTracker.ResetFlags();
                DownedBossRewardPlayer rewardPlayer = player.GetModPlayer<DownedBossRewardPlayer>();
                rewardPlayer.ResetFlags();
                return true;
            case "gintze":
                if (MultiplayerHelper.IsHost)
                {
                    ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
                    colosseumSystem.Reset();
                }
                else
                {
                    Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ResetColosseum).Send(-1);
                }
                return true;
        }

        return false;
    }
}

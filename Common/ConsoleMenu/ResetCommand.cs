using Stellamod.Common.ArmorShop;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.QuestSystem;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WaypointSystem;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Items;
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
            "gintze",
            "quests",
            "cauldron",
            "armor",
            "waypoints",
            "flags",
            "wand",
            "insource"
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
            case "wand":
                player.GetModPlayer<AdvancedMagicPlayer>().ResetProgress();
                return true;
            case "insource":
                player.GetModPlayer<FlaskPlayer>().ResetProgress();
                player.GetModPlayer<BellPlayer>().ResetProgress();
                return true;
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
            case "quests":
                {
                    QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
                    questPlayer.ActiveQuests.Clear();
                    questPlayer.CompletedQuests.Clear();
                    questPlayer.RewardQuests.Clear();
                    questPlayer.RecalculateUI = true;
                }
                return true;
            case "cauldron":
                {
                    CauldronPlayer cauldronPlayer = player.GetModPlayer<CauldronPlayer>();
                    cauldronPlayer.Crafts.Clear();
                }
                return true;
            case "armor":
                {
                    player.GetModPlayer<ArmorShopPlayer>().PurchasedArmors.Clear();
                }
                return true;
            case "waypoints":
                {
                    OrganWaypointTracker tracker = ModContent.GetInstance<OrganWaypointTracker>();
                    tracker.ResetWaypoints();
                }
                return true;
            case "flags":
                {
                    GameFlags.ResetFlags();
                }
                return true;
        }

        return false;
    }
}

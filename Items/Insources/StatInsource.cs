using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.PlayerLevelingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources;

public class StatInsourceBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}

public class StatInsourcePlayer : ModPlayer
{
    public int durationToUse;
    public override void ResetEffects()
    {
        base.ResetEffects();
    }
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if (Player.HasBuff<StatInsourceBuff>())
        {
            for (int i = 0; i < 7; i++)
            {
                var levelingPlayer = Player.GetModPlayer<LevelingPlayer>();
                levelingPlayer.statModifiers[i] += 8;
            }
        }

        if (durationToUse > 0)
        {
            Player.AddBuff(ModContent.BuffType<StatInsourceBuff>(), durationToUse);
            durationToUse = 0;
        }
    }
}

public class StatInsource : InsourceItem
{
    public override int GetAddedTime()
    {
        return 60 * 120;
    }

    public override void UseInsource(FlaskPlayer flaskPlayer)
    {
        base.UseInsource(flaskPlayer);
        Player player = flaskPlayer.Player;
        StatInsourcePlayer statInsourcePlayer = player.GetModPlayer<StatInsourcePlayer>();
        statInsourcePlayer.durationToUse += 60 * 20;
    }
}
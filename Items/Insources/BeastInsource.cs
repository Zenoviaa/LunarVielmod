using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources;
public class BeastInsourcePlayer : ModPlayer
{
    public int stacks;
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!Player.HasBuff<BeastInsourceBuff>())
        {
            stacks = 0;
        }
        else
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                var fx = FXUtil.GlowStretch(Player.Center + vel.SafeNormalize(Vector2.Zero) * 32, vel);
                fx.OuterGlowColor = Color.Red;
                fx.VectorScale *= 0.4f;
            }
        }
    }
    public override void OnHurt(Player.HurtInfo info)
    {
        base.OnHurt(info);
        stacks = 0;
    }
}

public class BeastInsourceBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        BeastInsourcePlayer berserkPlayer = player.GetModPlayer<BeastInsourcePlayer>();
        player.GetDamage(DamageClass.Generic) += 0.1f * berserkPlayer.stacks;
        if (berserkPlayer.stacks <= 0)
            player.DelBuff(buffIndex);
    }
}

public class BeastInsource : InsourceItem
{
    public override int GetAddedTime()
    {
        return 60 * 15;
    }

    public override void UseInsource(FlaskPlayer flaskPlayer)
    {
        base.UseInsource(flaskPlayer);
        Player player = flaskPlayer.Player;
        BeastInsourcePlayer berserkingPlayer = player.GetModPlayer<BeastInsourcePlayer>();
        berserkingPlayer.stacks++;
        player.AddBuff(ModContent.BuffType<BeastInsourceBuff>(), 2);
    }
}
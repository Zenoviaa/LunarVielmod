using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources;

public class FireResistancePlayer : ModPlayer
{
    public int timeLeft;
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (timeLeft > 0)
            timeLeft--;
        if (timeLeft > 0)
        {
            Player.AddBuff(ModContent.BuffType<FireResistanceInsourceBuff>(), 2);
            Player.lavaImmune = true;
            Player.ClearBuff(BuffID.OnFire);
            Player.ClearBuff(BuffID.OnFire3);
            Player.ClearBuff(BuffID.ShadowFlame);
            Player.ClearBuff(BuffID.CursedInferno);
            Player.ClearBuff(BuffID.Frostburn);
            Player.ClearBuff(BuffID.Frostburn2);
        }
    }
}

public class FireResistanceInsourceBuff : ModBuff
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

public class FireResistanceInsource : InsourceItem
{
    public override int GetAddedTime()
    {
        return 60 * 45;
    }

    public override void UseInsource(FlaskPlayer flaskPlayer)
    {
        base.UseInsource(flaskPlayer);
        Player player = flaskPlayer.Player;
        FireResistancePlayer fireResistancePlayer = player.GetModPlayer<FireResistancePlayer>();
        fireResistancePlayer.timeLeft += 30 * 60;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankBrooch>();
    }
}
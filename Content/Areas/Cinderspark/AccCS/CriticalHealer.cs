using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class CriticalHealer : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<CriticalHealerPlayer>().hasCriticalHealer = true;
    }
}
public class CriticalHealerPlayer : ModPlayer
{
    public bool hasCriticalHealer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasCriticalHealer = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (hit.Crit && hasCriticalHealer)
        {
            Player.Heal(4);
        }
    }
}
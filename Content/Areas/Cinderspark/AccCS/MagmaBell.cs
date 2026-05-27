using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class MagmaBellPlayer : ModPlayer
{
    public bool hasMagmaBell;
    public float regenTimer;
    public float waterHurtTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasMagmaBell = false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (hasMagmaBell)
        {
            Player.lavaImmune = true;
        }

        if (!hasMagmaBell)
        {
            waterHurtTimer = 0;
            return;
        }

        if (Player.lavaWet)
        {
            regenTimer++;
            if (regenTimer >= 60)
            {
                regenTimer = 0;
                Player.Heal(1);
            }
        }

        if (Player.wet && !Player.lavaWet)
        {
            if (waterHurtTimer <= 0)
            {
                Player.Hurt(new PlayerDeathReason(), 60, 1);
                waterHurtTimer = 30;
            }
            else
            {
                waterHurtTimer--;
            }
        }
    }
}

public class MagmaBell : ModItem
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
        player.GetModPlayer<MagmaBellPlayer>().hasMagmaBell = true;
    }
}

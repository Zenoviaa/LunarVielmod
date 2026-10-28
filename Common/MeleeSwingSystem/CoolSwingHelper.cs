using Stellamod.Common.StaminaSystem;
using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.DataStructures;


namespace Stellamod.Common.MeleeSwingSystem;

public static class CoolSwingHelper
{

    /// <summary>
    /// Performs a cool swing!
    /// </summary>
    /// <param name="player"></param>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="type"></param>
    /// <param name="damage"></param>
    /// <param name="knockback"></param>
    public static void DoACoolSwing(
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
        int combo = comboPlayer.ComboCounter;
        int dir = comboPlayer.ComboDirection;
        Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
            player.whoAmI, ai1: dir, ai2: combo);
        comboPlayer.IncreaseCombo();
    }
    public static void DoAStaminaSwing(
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
        SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
        int combo = comboPlayer.StaminaComboCounter;
        int dir = comboPlayer.ComboDirection;

        Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
            player.whoAmI, ai1: dir, ai2: combo);
        comboPlayer.IncreaseCombo();


    }

    public static void DoBasicOrStaminaSwing(Item item, Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            if(item.TryGetGlobalItem(out CoolSwingGlobalItem cs))
            {
                player.GetModPlayer<StaminaNDashPlayer>().Consume(cs.meleeSwingStaminaCost);
                player.GetModPlayer<StaminaNDashPlayer>().StaminaEffects(player);
                DoAStaminaSwing(player, source, position, velocity, item.StaminaShoot, (int)(damage * item.StaminaDamageMultiplier), knockback);
                PixelPrimitiveCircleFactory.CreateGenericInBoom(player.Center, Color.White, Color.White, 24, 128);
            }

        }
        else
        {
            DoACoolSwing(player, source, position, velocity, type, damage, knockback);
        }
    }
}

using Stellamod.Common.StaminaSystem;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Common.MeleeSwingSystem;

public class CoolSwingGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public bool isMeleeSwingItem;
    public int meleeSwingStaminaCost;
    public int meleeStaminaShoot;
    public float meleeSwingStaminaDamageMultiplier;
    public MeleeWeaponType meleeWeaponType;
    public override bool CanShoot(Item item, Player player)
    {
        if (player.altFunctionUse == 2 && isMeleeSwingItem)
        {
            StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            return dashPlayer.CanConsume(meleeSwingStaminaCost);
        }
        return base.CanShoot(item, player);
    }
    public override bool AltFunctionUse(Item item, Player player)
    {
        if (isMeleeSwingItem)
            return true;
        return base.AltFunctionUse(item, player);
    }



}

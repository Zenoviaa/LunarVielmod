using Stellamod.Common.WeaponUpgrade;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;


public class DragoncrestMoldPlayer : ModPlayer
{
    public bool hasDragonCrestMold;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasDragonCrestMold = false;
    }

    public bool IsActive(Item item)
    {
        if (!hasDragonCrestMold)
            return false;
        if (!item.TryGetGlobalItem<WeaponUpgradeGlobalItem>(out var result))
            return false;
        if (result.weaponLevel > 0)
            return false;


        return true;
    }
    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        base.ModifyWeaponDamage(item, ref damage);
        if (!IsActive(Player.HeldItem))
            return;
        damage += 0.05f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!IsActive(Player.HeldItem))
            return;
        target.AddBuff(BuffID.OnFire, 100);
    }
}


public class DragoncrestMold : ModItem
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
        player.GetModPlayer<DragoncrestMoldPlayer>().hasDragonCrestMold = true;

    }
}
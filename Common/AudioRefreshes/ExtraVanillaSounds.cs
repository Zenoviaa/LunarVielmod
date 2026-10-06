using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.AudioRefreshes;

public class ExtraVanillaSounds : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Player.OpenChest += PlayChestOpenSound;
        On_ItemSlot.LeftClick_ItemArray_int_int += PlayEquipVanitySound;
    }

    private void PlayEquipVanitySound(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        orig(inv, context, slot);
        bool flag = Main.mouseLeftRelease && Main.mouseLeft;
        if ((inv[slot].vanity || inv[slot].wornArmor) && flag)
        {
            AudioRefreshesHelper.PlayEquipVanitySound();
        }
    }


    private void PlayChestOpenSound(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest)
    {
        orig(self, x, y, newChest);
        AudioRefreshesHelper.PlayChestOpenSound(self.position);
    }
}

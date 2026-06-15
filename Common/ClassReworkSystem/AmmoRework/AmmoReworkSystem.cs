using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework;

public class AmmoReworkSystem : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Player.ChooseAmmo += ChooseAmmo;

    }

    private Item ChooseAmmo(On_Player.orig_ChooseAmmo orig, Player self, Item weapon)
    {
        Item item = orig(self, weapon);
        ClassReworkPlayer reworkPlayer = self.GetModPlayer<ClassReworkPlayer>();
        if (ItemLoader.CanChooseAmmo(weapon, reworkPlayer.QuiverAmmoItem, self))
        {
            item = reworkPlayer.QuiverAmmoItem;
            item.stack++;
        }
        return item;
    }
}

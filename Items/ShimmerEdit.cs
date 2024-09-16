using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items
{
    internal class ShimmerEdit : ModSystem
    {
        public override void Load()
        {
            base.Load();
            On_Item.CanShimmer += CanShimmer;
        }

        public override void Unload()
        {
            base.Unload();
            On_Item.CanShimmer -= CanShimmer;
        }

        private bool CanShimmer(On_Item.orig_CanShimmer orig, Item self)
        {
            //Cannot shimmer until goth is dead
            if (self.type == ItemID.RodofDiscord && !DownedBossSystem.downedGothBoss)
                return false;
            return orig(self);
        }
    }
}

using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts
{
    public class Solution : ModDust
    {
        public override void SetStaticDefaults()
        {
            UpdateType = DustID.PureSpray;
        }
    }
}
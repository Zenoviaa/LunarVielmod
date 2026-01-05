using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.NPCHelpers
{
    public class NPCSets : ModSystem
    {
        public override void SetupContent()
        {
            Heavy = ItemID.Sets.Factory.CreateBoolSet();
            CannotBeBubbled = ItemID.Sets.Factory.CreateBoolSet();
            ResistedByFlamecrestShield = ItemID.Sets.Factory.CreateBoolSet();
            base.SetupContent();

        }
        public static bool[] Heavy;
        public static bool[] ResistedByFlamecrestShield;
        public static bool[] CannotBeBubbled;
    }

}

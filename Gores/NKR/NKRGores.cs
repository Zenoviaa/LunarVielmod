using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Gores.NKR
{
    public abstract class NKRGore : ModGore
    {
        /*
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            base.OnSpawn(gore, source);
            gore.
            gore.type = GoreID.
        }
        */
    }
    public class NoxianFront : NKRGore { }
    public class NoxianFuelCan : NKRGore { }
    public class NoxianScrew : NKRGore { }
    public class NoxianSeat : NKRGore { }
    public class NoxianStep : NKRGore { }
    public class NoxianTire : NKRGore { }
}

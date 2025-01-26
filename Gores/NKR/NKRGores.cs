using Terraria.ModLoader;

namespace Stellamod.Gores.NKR
{
    internal abstract class NKRGore : ModGore
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
    internal class NoxianFront : NKRGore { }
    internal class NoxianFuelCan : NKRGore { }
    internal class NoxianScrew : NKRGore { }
    internal class NoxianSeat : NKRGore { }
    internal class NoxianStep : NKRGore { }
    internal class NoxianTire : NKRGore { }
}

using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
    internal abstract class RibbonGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 5;
            gore.frame = (byte)Main.rand.Next(5);
            gore.frameCounter = (byte)Main.rand.Next(5);
            gore.timeLeft = 805;
            UpdateType = 910;
        }

        public override bool Update(Gore gore)
        {
            gore.velocity *= 0.98f;
            return base.Update(gore);
        }
    }

    internal class RibbonRed : RibbonGore { }

    internal class RibbonYellow : RibbonGore { }

    internal class RibbonBlue : RibbonGore { }

    internal class RibbonWhite : RibbonGore { }

    internal class RibbonPink : RibbonGore { }
}

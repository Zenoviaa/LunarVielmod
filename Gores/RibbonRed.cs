using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
    internal class RibbonRed : ModGore
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
            gore.velocity.X *= 0.98f;
            return base.Update(gore);
        }
    }
}

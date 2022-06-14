using Stellamod.WorldG;
using System;
using Terraria.ModLoader;

namespace Stellamod
{
    public class Stellamod : Mod
    {
        public static object Instance { get; internal set; }

        public static implicit operator Stellamod(StellaWorld v)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Helpers
{
    internal interface IComboSystem
    {
        int[] ComboProjectiles { get; }
        string[] ComboProjectilesIcons { get; }
        float[] ComboProjectilesDamageMultiplers { get; }
        string FullCharge { get; }
        string EmptyCharge { get; }
        Vector3 ColorStart { get; }
        Vector3 ColorEnd { get; }
    }
}
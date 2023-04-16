using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Helpers
{
    internal interface IComboProjectile
    {
        int MaxCharges { get; }
        int CurCharges { get; }
        int projectileChargeLoopTime { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Core.Utilities;

/// <summary>
/// A vector4 with named parameters for position and velocity
/// </summary>
/// <param name="position"></param>
/// <param name="velocity"></param>
public record struct PositionVelocity(Vector2 position, Vector2 velocity)
{

}

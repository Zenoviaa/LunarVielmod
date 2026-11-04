using Terraria;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Wraps around a float and represents an angle
/// </summary>
public record struct Angle
{
    public static readonly Angle Right = new Angle(Vector2.UnitX.ToRotation());
    public static readonly Angle Left = new Angle(-Vector2.UnitX.ToRotation());
    public static readonly Angle Up = new Angle(-Vector2.UnitY.ToRotation());
    public static readonly Angle Down = new Angle(Vector2.UnitY.ToRotation());
    public static readonly Angle[] CardinalAngles = new Angle[4]
    {
        Left, Right, Up, Down
    };
    public Angle(Vector2 vector)
    {
        this.radians = vector.ToRotation();
    }

    public Angle(float radians)
    {
        this.radians = radians;
    }

    public float radians;
    public float Degrees
    {
        get
        {
            return MathHelper.ToDegrees(radians);
        }
        set
        {
            radians = MathHelper.ToRadians(value);
        }
    }
    
    public static implicit operator float(Angle a) => a.radians;
}

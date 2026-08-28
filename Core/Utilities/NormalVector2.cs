using System;
using Terraria;

namespace Stellamod.Core.Utilities;

public struct NormalVector2 : IEquatable<NormalVector2>
{
    public NormalVector2(in Vector2 originalVelocity)
    {
        Vector2 normalizedVector = originalVelocity.SafeNormalize(Vector2.Zero);
        X = normalizedVector.X;
        Y = normalizedVector.Y;
    }

    public float X;
    public float Y;

    /// <summary>
    /// Compares whether current instance is equal to specified <see cref="Object"/>.
    /// </summary>
    /// <param name="obj">The <see cref="Object"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public override bool Equals(object obj)
    {
        return (obj is NormalVector2) && Equals((NormalVector2)obj);
    }

    /// <summary>
    /// Compares whether current instance is equal to specified <see cref="Vector2"/>.
    /// </summary>
    /// <param name="other">The <see cref="Vector2"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public bool Equals(NormalVector2 other)
    {
        return (X == other.X &&
                Y == other.Y);
    }

    /// <summary>
    /// Gets the hash code of this <see cref="Vector2"/>.
    /// </summary>
    /// <returns>Hash code of this <see cref="Vector2"/>.</returns>
    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }


    #region Public Static Operators

    /// <summary>
    /// Inverts values in the specified <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/> on the right of the sub sign.</param>
    /// <returns>Result of the inversion.</returns>
    public static NormalVector2 operator -(NormalVector2 value)
    {
        value.X = -value.X;
        value.Y = -value.Y;
        return value;
    }

    /// <summary>
    /// Compares whether two <see cref="Vector2"/> instances are equal.
    /// </summary>
    /// <param name="value1"><see cref="Vector2"/> instance on the left of the equal sign.</param>
    /// <param name="value2"><see cref="Vector2"/> instance on the right of the equal sign.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public static bool operator ==(NormalVector2 value1, NormalVector2 value2)
    {
        return (value1.X == value2.X &&
                value1.Y == value2.Y);
    }

    /// <summary>
    /// Compares whether two <see cref="Vector2"/> instances are equal.
    /// </summary>
    /// <param name="value1"><see cref="Vector2"/> instance on the left of the equal sign.</param>
    /// <param name="value2"><see cref="Vector2"/> instance on the right of the equal sign.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public static bool operator !=(NormalVector2 value1, NormalVector2 value2)
    {
        return !(value1 == value2);
    }

 

    /// <summary>
    /// Multiplies the components of two vectors by each other.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the mul sign.</param>
    /// <param name="value2">Source <see cref="Vector2"/> on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication.</returns>
    public static Vector2 operator *(NormalVector2 value1, Vector2 value2)
    {
        value2.X *= value1.X;
        value2.Y *= value1.Y;
        return value2;
    }

    /// <summary>
    /// Multiplies the components of vector by a scalar.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/> on the left of the mul sign.</param>
    /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication with a scalar.</returns>
    public static Vector2 operator *(NormalVector2 value, float scaleFactor)
    {
        Vector2 result = value;
        result.X *= scaleFactor;
        result.Y *= scaleFactor;
        return result;
    }

    /// <summary>
    /// Multiplies the components of vector by a scalar.
    /// </summary>
    /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
    /// <param name="value">Source <see cref="Vector2"/> on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication with a scalar.</returns>
    public static Vector2 operator *(float scaleFactor, NormalVector2 value)
    {
        Vector2 result = value;
        result.X *= scaleFactor;
        result.Y *= scaleFactor;
        return result;
    }

    #endregion


    public static implicit operator Vector2(NormalVector2 normalVector) => new Vector2(normalVector.X, normalVector.Y);
}

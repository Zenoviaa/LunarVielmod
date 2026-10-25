using System;
using System.Runtime.InteropServices;

namespace Stellamod.Core.Utilities;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexPosition : IVertexType
{
    public Vector3 Position;
    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    static VertexPosition()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
    }

    public VertexPosition(Vector3 position)
    {
        Position = position;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "{{Position:" + Position.ToString();
    }

    public static bool operator ==(VertexPosition left, VertexPosition right)
    {
        if (left.Position == right.Position)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(VertexPosition left, VertexPosition right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return this == (VertexPosition)obj;
    }
}

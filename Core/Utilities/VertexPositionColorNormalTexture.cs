using System;
using System.Runtime.InteropServices;

namespace Stellamod.Core.Utilities;

/*
To make this tornado, I think to get the 3D'ness of it, the easiest way would be to make and spin a cylinder mesh,
and scroll/blend a scrolling texture along that mesh/
So we'ed need to create multiple quads
FURTHERMORE, the curves and movement tornados can do is REALLY cool
I don't want to make a perfectly uniform tornado as that looks a bit stiff, I wanna make the best tornado everrr


Let's break this down into steps
1. Create several quads around a central point, it should divide the size of the tornado to see how many bands to make

The reason we need so many bands is for the distortion and movement that the tornado needs to do, it's not uniform
If we curve it correctly and make the top lazily follow the bottom, it'll give it a very dynamic form

2. Scroll a texture along the mesh and make the back sides of the quads darken, we could also cull 
counter clockwise faces it depends on whatever looks better, but I think I want counterclockwise faces to just be darker
if push comes to shove we just render it twice, just the second time culls the opposite side and the 2nd one is darker/rendered first

*/
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexPositionColorNormalTexture : IVertexType
{
    public Vector3 Position;

    public Vector3 Normal;

    public Vector2 TextureCoordinate;
    public Color Color;
    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    static VertexPositionColorNormalTexture()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0));
    }

    public VertexPositionColorNormalTexture(Vector3 position, Color color,Vector2 textureCoordinate,  Vector3 normal )
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
        Color = color;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "{{Position:" + Position.ToString() + " Normal:" + Normal.ToString() + " TextureCoordinate:" + TextureCoordinate.ToString() + "}}";
    }

    public static bool operator ==(VertexPositionColorNormalTexture left, VertexPositionColorNormalTexture right)
    {
        if (left.Position == right.Position && left.Normal == right.Normal)
        {
            return left.TextureCoordinate == right.TextureCoordinate;
        }

        return false;
    }

    public static bool operator !=(VertexPositionColorNormalTexture left, VertexPositionColorNormalTexture right)
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

        return this == (VertexPositionColorNormalTexture)obj;
    }
}

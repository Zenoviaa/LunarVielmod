namespace Stellamod.Common.Particles;

//Gonna include the glow colors as an extra thingy

public struct BitDustInstanceData : IVertexType
{
    private Vector4 _color;
    private Vector4 _innerColor;
    private Vector4 _outerColor;
    private Vector4 _transformation;
    private Vector3 _tilingOffsetRotation;
    public BitDustInstanceData()
    {

    }

    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
        new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Color, 2),
        new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2)
    );

    VertexDeclaration IVertexType.VertexDeclaration
    {
        get { return VertexDeclaration; }
    }

    public Vector4 Color
    {
        get { return _color; }
        set { _color = value; }
    }
    public Vector4 InnerColor
    {
        get { return _innerColor; }
        set { _innerColor = value; }
    }
    public Vector4 OuterColor
    {
        get { return _outerColor; }
        set { _outerColor = value; }
    }
    public Vector4 Transformation
    {
        get { return _transformation; }
        set { _transformation = value; }
    }
    public Vector3 TilingOffsetRotation
    {
        get
        {
            return _tilingOffsetRotation;
        }
        set
        {
            _tilingOffsetRotation = value;
        }
    }
}

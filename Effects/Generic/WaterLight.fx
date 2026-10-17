matrix transformMatrix;
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct HeightPixelShaderOutput
{
    float4 Height : SV_Target0;
    float4 Light : SV_Target1;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    //We have to multiply the position by the matrix so it appears in the correct spot
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

HeightPixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    //Lighting pos should already be converted to screen space by the cpu
    //So we can just do this
    float2 coords = input.TextureCoordinates;
    float d = coords.x + coords.y;
    d /= 2.0;
    HeightPixelShaderOutput o;
    o.Height = float4(d, d, d, d) * input.Color;
    o.Light = o.Height;
    return o;
}


technique SpriteDrawing
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};
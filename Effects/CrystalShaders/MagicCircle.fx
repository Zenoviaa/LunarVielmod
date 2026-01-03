
matrix transformMatrix;
texture spriteTexture;
sampler2D spriteTex = sampler_state
{
    texture = <spriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};
float4 tilingOffset;


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

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

// The X coordinate is the trail completion, the Y coordinate is the elevation on the point of the trail.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Calculate distorting noise
    float2 offset = tilingOffset.rg;
    float2 tiling = tilingOffset.ba;
    float2 coords = offset + input.TextureCoordinates.xy * tiling;
    //Additive drawing
    float3 rgb = tex2D(spriteTex, coords).rgb * input.Color.rgb;
    return float4(rgb, 0.0);
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

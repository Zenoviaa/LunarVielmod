matrix transformMatrix;
float time;
float2 tiling;
float3 innerColor;
float3 outerColor;

texture laserTexture;
sampler2D laserTex = sampler_state
{
    texture = <laserTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};

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

float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float2 offset = float2(time * -0.05, 0.0);
    float2 laserCoords = coords * tiling + offset;
    float n = tex2D(laserTex, laserCoords);
    
    float3 glowingColor = lerp(outerColor, innerColor, n);
    float3 finalColor = glowingColor * n;
    return float4(finalColor, 0.0) * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
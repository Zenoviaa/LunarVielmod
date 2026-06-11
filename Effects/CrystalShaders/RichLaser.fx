matrix transformMatrix;
float time;
float2 tiling;

float3 laserColor;
float3 bloomInnerColor;
float3 bloomOuterColor;
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

texture bloomTexture;
sampler2D bloomTex = sampler_state
{
    texture = <bloomTexture>;
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
    float laserSample = tex2D(laserTex, coords + offset);
  
    float3 innerCol = laserSample * laserColor;
    
    float2 offset2 = float2(time * -0.025, 0.0);
    float bloomSample = tex2D(bloomTex, coords + offset2);
    
    //If we want we can have a gradient on the bloom
    float3 bloomMultiplyCol = lerp(bloomOuterColor, bloomInnerColor, QuadraticBump(bloomSample));
    float3 bloomCol = bloomMultiplyCol * bloomSample * input.Color.rgb;
    
    //0.0 alpha for additive draw
    float3 combinedCol = innerCol + bloomCol;
    //combinedCol = lerp(float3(0.0, 0.0, 0.0), combinedCol, input.Color.a);
    return float4(combinedCol, 0.0);

}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
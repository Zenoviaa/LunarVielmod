matrix transformMatrix;
float time;
float3 bloomColor;
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


float OutCubic(float t)
{
    return 1.0 - pow(1.0 - t, 3.0);
}
float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
  
float InExpo(float t)
{
    const float p = 10.0;
    return t == 0.0 ? 0.0 : pow(2.0, p * t - p);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float2 laserCoords = coords + float2(time * -0.05, 0.0);
    float n = tex2D(laserTex, laserCoords).r;
    float3 glowingColor = lerp(bloomColor, float3(1.0, 1.0, 1.0), n) * QuadraticBump(coords.y);
    float4 finalColor = float4(glowingColor, 1.0) * input.Color;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
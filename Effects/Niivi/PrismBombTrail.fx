#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float distortionStrength;

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


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //first calculate some coords and oise
    float2 coords = input.TextureCoordinates.xy;
    float2 noiseCoords = coords + float2(time, 0.0);
    float noise = tex2D(noiseSampler, noiseCoords);
    float2 noiseOffset = float2(0.0, sin(noise * 3.14) * distortionStrength );
    float2 offsetCoords = coords + noiseOffset;
    float4 laserColor = tex2D(spriteSampler, offsetCoords + float2(-time * 2.0, 0.0));
    
    float4 outColor = float4(1.0, 1.0, 1.0, 1.0);
    outColor.r += sin(coords.x * 4.0 + time);
    outColor.g += cos(coords.y * 4.0 + time);
    outColor.b += sin(coords.x * 8.0 + time);
    
    
    
    laserColor += QuadraticBump(coords.y) * 0.2;
    laserColor *= input.Color;
    laserColor *= outColor;
    laserColor += coords.x * laserColor.r * 3.0;
    laserColor += sin(time * -24.0 + coords.y * 18.0) * laserColor.r;
    laserColor = floor(laserColor * 2.0) / 2.0;
    laserColor.a = 0.0;
    return laserColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
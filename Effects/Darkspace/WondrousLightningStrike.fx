#include "../Helpers/Math.fxh"
matrix transformMatrix;

texture spriteTexture;
sampler2D spriteSampler = sampler_state
{
    texture = <spriteTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};

texture noiseTexture;
sampler2D noiseSampler = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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

    float pixelated = 128.0;
    float2 noiseCoords = coords + float2(time, 0.0);
    float noise = tex2D(noiseSampler, noiseCoords * 0.2);
    float2 noiseOffset = float2(0.0, sin(noise * 3.14) * distortionStrength);
    float2 offsetCoords = coords + noiseOffset;
    float4 laserColor = tex2D(spriteSampler, offsetCoords + float2(-time, sin(-time * 12.0 + coords.x * 8.0) * 0.15));
    
    float4 outColor = float4(1.0, 0.0, 0.0, 1.0);
    outColor.rgb = ApplyHue(outColor.rgb, time * -8.0 + coords.x * 4.0);

  

    laserColor += QuadraticBump(coords.y) * 0.2;

    laserColor *= outColor;
    laserColor *= lerp(1.0, 3.0, coords.x);

    laserColor = floor(laserColor * 8.0) / 8.0;
    laserColor *= 1.0 - coords.x;
    laserColor *= QuadraticBump(coords.y);
    laserColor += pow(QuadraticBump(offsetCoords.y), 16.0);
    laserColor *= 2.0;
    laserColor *= input.Color;
    laserColor.a = 0.0;
    return laserColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
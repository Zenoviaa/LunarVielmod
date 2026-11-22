#include "Helpers/Math.fxh"
#define PS_SHADERMODEL ps_3_0

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D ClampTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = clamp;
    AddressV = clamp;
};


Texture2D WaterTexture;
sampler2D WaterTextureSampler = sampler_state
{
    Texture = <WaterTexture>;
    AddressU = clamp;
    AddressV = clamp;
};


Texture2D NoiseTexture;
sampler2D NoiseTextureSampler = sampler_state
{
    Texture = <NoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

float3 InnerColor;
float3 BloomColor;

float3 StartGradient;
float3 EndGradient;


float2 ScreenOffset;
float2 Tiling;
float Time;
float Quantize;
float Distortion;
float NormalDistortionStrength;

Texture2D NormalNoiseTexture;
sampler2D NormalNoiseSampler = sampler_state
{
    Texture = <NormalNoiseTexture>;
};

Texture2D HeightMapTexture;
sampler2D HeightMapSampler = sampler_state
{
    Texture = <HeightMapTexture>;
};
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float reflectionDistance;
float2 reflectionTexelSize;
float reflectionPower;
float4 outlineColor;
float2 outlineTexelSize;


float4 CombineALLPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    
    
    float4 baseWaterColor = tex2D(SpriteTextureSampler, coords);
    
    
    float4 fancyWaterColor = tex2D(NormalNoiseSampler, coords);
    float osc = sin(Time + coords.x * 5.0) * 0.676;
    fancyWaterColor.rgb = ApplyHue(fancyWaterColor.rgb, osc);
    float4 finalColor = fancyWaterColor * baseWaterColor.a;
    return finalColor * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL CombineALLPS();
    }
};

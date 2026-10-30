#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0, float4 pos : SV_Position) : COLOR0
{
    float pixelated = 128.0;
    coords = floor(coords * pixelated) / pixelated;
    
    //WHOAAA PRISMATIC!!!
    float4 color = float4(1.0, 1.0, 1.0, 1.0);
    color.r += sin(coords.x * 6.28 + time * 32.0);
    color.g += cos(coords.y * 6.0 + time);
    color.b += cos(coords.x * 6.28 + time);
   // color *= QuadraticBump(coords.x);
    color += coords.y * 0.5;
    color += tintColor.a * coords.y;
    color *= coords.y;
    
    float range = 0.3;
    color *= saturate(coords.x / range);
    color *= saturate((1.0 - coords.x) / range);
    color *= saturate((1.0 - coords.y) / 0.1);
    //color = floor(color * 8.0) / 8.0;

    color *= tintColor;
    return color * 1.6;
    //return coords.y * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
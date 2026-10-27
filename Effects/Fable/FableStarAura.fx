#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler starSampler : register(s1);
float2 spriteSize;
float2 noiseTexelSize;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{ 
    float t = time;
    
    float mask = tex2D(spriteSampler, coords);
    float2 starCoords = coords * spriteSize * noiseTexelSize;
    starCoords += float2(t * 0.4, t * 0.4);
 //   starCoords.y += sin(time * 0.4 + coords.x * 18.0) * 0.02;
    float4 starColor = tex2D(starSampler, starCoords);

    starColor += (sin(time * 8.0 + length(coords) * 18.0) * 0.5 + 0.5) * 2.5 * starColor.r;
    float4 myStarColor = starColor;
    coords.y += sin(time * 4.0 + coords.x * 8.0) * 0.05;
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    float b = QuadraticBump(saturate((0.5 - l) / 0.2) + sin(t * 4.0) * 0.01);
    b = pow(b, 16.0);
    b *= 32.0;
    starColor += b;


    return starColor * mask * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
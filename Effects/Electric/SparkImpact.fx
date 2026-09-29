#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float time;


float NaiveLum(float3 color)
{
    float l = 0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b;
    return l;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords);
    float osc = sin(time * 24.0) * 0.5 + 0.5;
    float lum = NaiveLum(spriteColor.rgb);
    if (lum > 0.9)
    {
        return spriteColor * tintColor * osc;
    }
    else
    {
        return spriteColor * tintColor;
    }
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
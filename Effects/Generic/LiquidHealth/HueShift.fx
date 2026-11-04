#include "../../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float hueShift;



float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords);
    spriteColor.rgb = ApplyHue(spriteColor.rgb, hueShift);
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
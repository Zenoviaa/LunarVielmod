#include "PotionLiquidBase.fxh"

PSOutput PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0)
{
    PSOutput output;
    float4 metaballColor = LiquidColor(coords, tintColor);
    output.Liquid = metaballColor * 0.9;
    output.Mask = float4(metaballColor.r, metaballColor.r, metaballColor.r, metaballColor.r);
    return output;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

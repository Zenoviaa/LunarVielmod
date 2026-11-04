#include "PotionLiquidBase.fxh"

PSOutput NoMaskPixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0)
{
    PSOutput output;
    float4 metaballColor = LiquidColor(coords, tintColor);
    output.Liquid = metaballColor * 0.9;
    output.Mask = 0.0;
    return output;
}


technique Technique1
{
    pass NoMaskPass
    {
        PixelShader = compile ps_3_0 NoMaskPixelShaderFunction();
    }
}

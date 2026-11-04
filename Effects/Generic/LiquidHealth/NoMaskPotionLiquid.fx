#include "LiquidBase.fxh"

technique Technique1
{
    pass NoMaskPass
    {
        PixelShader = compile ps_3_0 NoMaskPixelShaderFunction();
    }
}

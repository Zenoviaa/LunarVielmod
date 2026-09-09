sampler spriteSampler : register(s0);


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = float4(1.0, 1.0, 1.0, 1.0);
    float2 direction = coords - float2(0.5, 0.5);
    
    float len = length(direction);
    float fadeOut = saturate(len / 0.5);
    float fadeOut2 = saturate((len - 0.3) / 0.2);
    fadeOut2 = 1.0 - fadeOut2;
    spriteColor *= fadeOut;
    spriteColor *= fadeOut2;
    spriteColor = floor(spriteColor * 24.0) / 24.0;
    return spriteColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
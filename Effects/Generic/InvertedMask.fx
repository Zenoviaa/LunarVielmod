sampler spriteSampler : register(s0);
sampler maskSampler : register(s1);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords) * tintColor;
    float invertedMask = 1.0 - tex2D(maskSampler, coords).a;
    return spriteColor * invertedMask;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
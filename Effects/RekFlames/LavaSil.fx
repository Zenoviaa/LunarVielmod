sampler spriteSampler : register(s0);
sampler maskSampler : register(s1);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords);
    float4 maskColor = tex2D(maskSampler, coords);
    return spriteColor.a * sampleColor * maskColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
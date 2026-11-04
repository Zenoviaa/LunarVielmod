sampler maskSampler : register(s0);
sampler mixSampler;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 maskColor = tex2D(maskSampler, coords);
    float4 mixColor = tex2D(mixSampler, coords);
    return maskColor * mixColor * 8.0;
}


technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

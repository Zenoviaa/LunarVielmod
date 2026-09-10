sampler spriteSampler : register(s0);
float2 texelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords) * sampleColor;
    if (spriteColor.a > 0.0)
        return spriteColor;
    
    
    float4 leftColor = tex2D(spriteSampler, coords + float2(-texelSize.x, 0.0)) * sampleColor;
    float4 rightColor = tex2D(spriteSampler, coords + float2(texelSize.x, 0.0)) * sampleColor;

    float x = max(leftColor.a, rightColor.a);
    return float4(1.0, 1.0, 1.0, 1.0) * x;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
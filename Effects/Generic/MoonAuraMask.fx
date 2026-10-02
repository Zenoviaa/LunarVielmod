sampler2D spriteSampler : register(s0);
sampler2D maskSampler : register(s1);
sampler2D noiseSampler : register(s2);

float time;
float distortionStrength;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 originalColor = tex2D(spriteSampler, coords);
    float4 maskColor = tex2D(maskSampler, coords);
    if(maskColor.r <= 0.0)
        return originalColor;
    
    float strengthAtPoint = lerp(1.0, 0.0, maskColor.r);
    strengthAtPoint *= distortionStrength;
    float2 scrollingCoords = frac(coords + float2(time * -0.05, time * -0.025));
    float noise = tex2D(noiseSampler, scrollingCoords).r;
    float2 distortionOffset = float2(cos(noise * 3.14), sin(noise * 3.14)) * strengthAtPoint;
    float4 distortedColor = tex2D(spriteSampler, coords + distortionOffset);
    float4 finalTint = lerp(float4(1.0, 1.0, 1.0, 1.0), tintColor, maskColor.r);
    distortedColor *= finalTint;
    return distortedColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
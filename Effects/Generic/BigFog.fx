sampler spriteSampler : register(s0);
float time;

float4 SampleFog(float2 coords, float t)
{
    float2 offsetCoords1 = coords + float2(t * -0.025, 0.0);
    float2 offsetCoords2 = coords + float2(t * -0.04, 0.75);
    
    offsetCoords1 = frac(offsetCoords1);
    offsetCoords2 = frac(offsetCoords2);
    
    float4 fogColor1 = tex2D(spriteSampler, offsetCoords1);
    float4 fogColor2 = tex2D(spriteSampler, offsetCoords2);
    float4 mixedFog = fogColor1 + fogColor2;
    mixedFog /= 1.5;
    return mixedFog;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = SampleFog(coords, time + tintColor.a * 30.0);
    float2 diff = coords - float2(0.5, 0.5);
    float ratio = 1.0 - saturate(length(diff) / 0.5);
    float4 fogColor = spriteColor * ratio;
    return fogColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float3 innerColor;
float3 outerColor;       
float threshold;
float InExpo(float t)
{
    const float p = 10.0;
    return t == 0 ? 0 : pow(2, p * t - p);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float col = tex2D(uImage0, uv);
    
    //Then we're gonna interp colors and fade it out on the edges
   // totalContribution = saturate(totalContribution);
    float denom = 1.0 - threshold;
    denom *= 0.79;
    float contribution = saturate(col - threshold) / denom;
    float3 metaballColorInner = lerp(outerColor, innerColor, InExpo(contribution));
    float3 metaballColorOuter = lerp(float3(0.2, 0.2, 0.2), float3(0.0, 0.0, 0.0), InExpo(contribution));
    float3 metaballColor = lerp(metaballColorOuter, metaballColorInner, contribution);
    float4 finalColor = float4(metaballColor, 1.0) * sampleColor * contribution;
    return finalColor * 1.5;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};
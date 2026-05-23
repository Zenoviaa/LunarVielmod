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
float time;
float yGradient;

float SampleNoise(float2 coords, float2 offset)
{
    float2 uv = coords;
    uv += offset;
    uv += float2(0.0, time * 0.05);
    uv = frac(uv);
    float innerNoise = tex2D(uImage0, uv).r;
    return innerNoise;
}

float SampleNoiseOutline(float2 coords, float2 offset)
{
    float2 uv = coords;
    uv += offset;
    uv += float2(0.0, time * 0.05);
    uv = frac(uv);
    float innerNoise = tex2D(uImage1, uv).r;
    return innerNoise;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float n = SampleNoise(coords, float2(time * 0.025, 0.0));
    float n2 = SampleNoise(coords, float2(time * -0.025 + 0.3, 0.2));
    float combinedNoise = saturate((n + n2) / 2.0);

    float outlineN = SampleNoiseOutline(coords, float2(time * 0.025, 0.0));
    float outlineN2 = SampleNoiseOutline(coords, float2(time * -0.025 + 0.3, 0.2));
    float combinedOutline = saturate((outlineN + outlineN2) / 2.0);
    
    return sampleColor * combinedNoise;
    //+combinedOutline * float4(0.5, 0.5, 0.0, 0.0);

}


technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};